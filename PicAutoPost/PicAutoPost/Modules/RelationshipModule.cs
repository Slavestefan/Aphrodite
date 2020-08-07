

using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model.Users;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class RelationshipModule : AphroditeModuleBase
    {
        private readonly Bot _bot;
        private readonly ILogger<RelationshipModule> _logger;
        private readonly RelationshipService _relationshipService;

        public RelationshipModule(IServiceProvider services, Bot bot, ILogger<RelationshipModule> logger) : base(services)
        {
            _bot = bot;
            _logger = logger;
            _relationshipService = new RelationshipService(TypedDbContext);
        }

        [Command("Owner Request")]
        public async Task RequestOwner([Remainder] string userSnowflakeOrMention)
        {
            try
            {
                User owner = null;
                if (!ulong.TryParse(userSnowflakeOrMention, out var ownerSnowflake))
                {
                    var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

                    if (mentionedUser == null)
                    {
                        await ReplySimpleEmbedAsync("You must specify a user to be your owner either by mentioning them (using @) or using their snowflake ID");
                        return;
                    }
                    ownerSnowflake = mentionedUser.Id;
                }

                if (ownerSnowflake == 655372144510894091 || ownerSnowflake == 654523653366611984)
                {
                    await ReplySimpleEmbedAsync("You could never handle me");
                    return;
                }

                var ownerDiscordUser = _bot.GetUserFromSnowflake(ownerSnowflake);
                owner = await TypedDbContext.GetOrCreateUserAsync(ownerSnowflake, ownerDiscordUser.Username);
                var slave = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username);
                try
                {
                    await _relationshipService.AddOwnerRequest(owner, slave, OwnerSlaveRelationshipTypes.FullControl);
                }
                catch (InvalidOperationException ex)
                {
                    await ReplySimpleEmbedAsync(ex.Message);
                    return;
                }
            
                await _bot.DmUserAsync(ownerSnowflake,
                    $"User {Context.Message.Author.Mention} requests to be owned by you. To confirm reply with {Environment.NewLine} !ap Owner Confirm {slave.Username} {Environment.NewLine}");
                await ReplySimpleEmbedAsync($"Request for ownership sent to {ownerDiscordUser.Mention}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Command("Owner Confirm")]
        public async Task ConfirmOwner([Remainder] string slaveSnowflakeOrUsernameOrMention)
        {
            if (!ulong.TryParse(slaveSnowflakeOrUsernameOrMention, out var slaveSnowflake))
            {
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                if (mentionedUser == null)
                {
                    slaveSnowflake = TypedDbContext.Users.FirstOrDefault(x => x.Username == slaveSnowflakeOrUsernameOrMention)?.DiscordId ?? 0;
                    if (slaveSnowflake == 0)
                    {
                        await ReplySimpleEmbedAsync("You must specify a user to confirm ownership of");
                        return;
                    }
                }
                else
                {
                    slaveSnowflake = mentionedUser.Id;
                }
            }

            var slave = await TypedDbContext.GetOrCreateUserAsync(slaveSnowflake, null);
            var slaveDiscordUser = _bot.GetUserFromSnowflake(slaveSnowflake);
            var relationship = await _relationshipService.GetRelationship(Context.Message.Author.Id, slaveSnowflake);
            if (relationship == null)
            {
                await ReplySimpleEmbedAsync($"There is no request pending for {slaveDiscordUser.Mention}");
                return;
            }

            if (relationship.Status == Status.Confirmed)
            {
                await ReplySimpleEmbedAsync($"You already own {slaveDiscordUser.Mention}");
                return;
            }

            if (relationship.Status != Status.PendingOwnerConfirmation)
            {
                await ReplySimpleEmbedAsync($"Invalid status to confirm ownership");
                return;
            }

            relationship.Status = Status.Confirmed;
            TypedDbContext.SaveChanges();
            await ReplySimpleEmbedAsync($"Request confirmed. You now own {slaveDiscordUser.Mention}");
            await _bot.DmUserAsync(slave.DiscordId, $"{Context.Message.Author.Mention} has accepted your request of ownership.");
        }
    }
}