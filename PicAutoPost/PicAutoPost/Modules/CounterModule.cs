using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model.Tracker;
using Slavestefan.Aphrodite.Model.Users;
using Slavestefan.Aphrodite.Web.Helpers;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Counter")]
    public class CounterModule : AphroditeModuleBase
    {
        private readonly ILogger<CounterModule> _logger;
        private readonly RelationshipService _relationshipService;

        public CounterModule(IServiceProvider services, ILogger<CounterModule> logger) : base(services)
        {
            _logger = logger;
            _relationshipService = new RelationshipService(TypedDbContext);
        }

        [Command("Create")]
        public async Task Create(string name, int amount, string targetUser = null, bool isHidden = false)
        {
            try
            {
                User user;
                if (targetUser != null)
                {
                    user = base.GetUserFromSnowflakeOrUsernameOrMention(targetUser);
                    if (!await _relationshipService.ConfirmRelationship(Context.Message.Author.Id, user.DiscordId, OwnerSlaveRelationshipTypes.Tracker))
                    {
                        await ReplySimpleEmbedAsync("You don't have permission to create a counter for that user. You need to own them first");
                        return;
                    }
                }
                else
                {
                    user = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username);
                }

                if (TypedDbContext.Counters.Any(x => x.Name == name))
                {
                    await ReplySimpleEmbedAsync("Counter with that name already exists, please choose another name");
                    return;
                }

                var counter = new Counter
                {
                    Name = name,
                    TotalAmount = amount,
                    User = user,
                    CompletedAmount = 0,
                    IsHidden = isHidden
                };

                TypedDbContext.Counters.Add(counter);
                TypedDbContext.SaveChanges();
                await ReplySimpleEmbedAsync($"Created counter with ID {counter.IdCounter}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Command("List")]
        public async Task ListCounters()
        {
            try
            {
                var users = _relationshipService.GetSlaves(Context.Message.Author.Id);
                users = users.Concat(TypedDbContext.Users.AsQueryable().Where(x => x.DiscordId == Context.Message.Author.Id));
                var counters = TypedDbContext.Counters.Include(x => x.User).Where(x => users.Any(u => u.IdUser == x.User.IdUser));
                var embedBuilder = new EmbedBuilder()
                {
                    Description = "Counters for " + Context.Message.Author.Mention,
                };

                foreach (var counter in counters)
                {
                    var embedField = new EmbedFieldBuilder()
                    {
                        Name = counter.IdCounter.ToString(),
                        Value = $"{counter.Name}{(counter.User.DiscordId != Context.Message.Author.Id ? " (" + counter.User.DiscordId.ToMention() + ")" : string.Empty)}",
                    };
                    embedBuilder.AddField(embedField);
                }

                await ReplySimpleEmbedAsync(embedBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Command("Info")]
        public async Task Info([Remainder] string nameOrId)
        {
            try
            {
                var isGuid = Guid.TryParse(nameOrId, out var guid);
                var counter = TypedDbContext.Counters.Include(x => x.User).FirstOrDefault(x => x.Name == nameOrId || (isGuid && x.IdCounter == guid));
                if (counter == null || !await HasCounterPermission(Context.Message.Author.Id, counter))
                {
                    await ReplySimpleEmbedAsync("Unknown counter. Please use the name or GUID");
                    return;
                }

                EmbedBuilder embed = new EmbedBuilder
                {
                    Description = "Info for counter: " + counter.IdCounter,
                    Fields =
                    {
                        new EmbedFieldBuilder {Name = "Name", Value = counter.Name},
                    },
                };

                if (counter.IsHidden && counter.User.DiscordId == Context.Message.Author.Id)
                {
                    embed.AddField(new EmbedFieldBuilder
                    {
                        Name = "Progress",
                        Value = counter.CompletedAmount >= counter.TotalAmount ? "Completed" : "Incomplete. Progress is hidden"
                    });
                }
                else
                {
                    embed = new EmbedBuilder
                    {
                        Description = "Info for counter: " + counter.IdCounter,
                        Fields =
                        {
                            new EmbedFieldBuilder { Name = "Progress", Value = $"{counter.CompletedAmount}/{counter.TotalAmount}"},
                            new EmbedFieldBuilder { Name = "Hidden from User", Value = counter.IsHidden}
                        }
                    };
                }

                await ReplySimpleEmbedAsync(embed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Command("Progress")]
        public async Task Progress(string nameOrId, int progress = 1)
        {
            try
            {
                var isGuid = Guid.TryParse(nameOrId, out var guid);
                var counter = TypedDbContext.Counters.Include(x => x.User).FirstOrDefault(x => x.Name == nameOrId || (isGuid && x.IdCounter == guid));

                if (counter == null || !await HasCounterPermission(Context.Message.Author.Id, counter))
                {
                    await ReplySimpleEmbedAsync("Unknown counter. Please use the name or GUID");
                    return;
                }

                if (counter.User.DiscordId != Context.Message.Author.Id)
                {
                    await ReplySimpleEmbedAsync("You cannot progress your sub's counter. If you'd like to manipulate the goal use !ap counter changeGoal");
                    return;
                }

                counter.CompletedAmount += progress;
                var counterHistory = new CounterHistory
                {
                    AmountChanged = progress,
                    ByUser = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username),
                    ChangeType = ChangeType.Progress,
                    Counter = counter
                };

                TypedDbContext.CounterHistory.Add(counterHistory);
                TypedDbContext.SaveChanges();

                if (counter.CompletedAmount >= counter.TotalAmount)
                {
                    await ReplySimpleEmbedAsync($"Goal reached: {counter.CompletedAmount}/{counter.TotalAmount}");
                    return;
                }

                if (!counter.IsHidden)
                {
                    await ReplySimpleEmbedAsync($"Progress updated: {counter.CompletedAmount}/{counter.TotalAmount}");
                }
                else
                {
                    await ReplySimpleEmbedAsync($"Progress updated. Keep going, you haven't reached the goal yet");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Command("ChangeGoal")]
        public async Task ChangeGoal(string nameOrId, int amount)
        {
            try
            {
                var isGuid = Guid.TryParse(nameOrId, out var guid);
                var counter = TypedDbContext.Counters.Include(x => x.User).FirstOrDefault(x => x.Name == nameOrId || (isGuid && x.IdCounter == guid));

                if (counter == null || !await HasCounterPermission(Context.Message.Author.Id, counter))
                {
                    await ReplySimpleEmbedAsync("Unknown counter. Please use the name or GUID");
                    return;
                }

                var owner = _relationshipService.GetOwner(counter.User.DiscordId);
                if (owner != null && counter.User.DiscordId == Context.Message.Author.Id)
                {
                    await ReplySimpleEmbedAsync("Only your owner can change the goal on your counters");
                    return;
                }

                counter.TotalAmount += amount;
                var history = new CounterHistory
                {
                    AmountChanged = amount,
                    ByUser = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username),
                    ChangeType = ChangeType.Goal,
                    Counter = counter,
                    Timestamp = DateTime.Now
                };
                TypedDbContext.CounterHistory.Add(history);
                TypedDbContext.SaveChanges();

                await ReplySimpleEmbedAsync($"Goal updated, new goal is {counter.TotalAmount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        //[Command("History")]
        //public async Task History(string nameOrId)
        //{
        //    try
        //    {
        //        var isGuid = Guid.TryParse(nameOrId, out var guid);
        //        var counter = TypedDbContext.Counters.Include(x => x.User).FirstOrDefault(x => x.Name == nameOrId || (isGuid && x.IdCounter == guid));

        //        if (counter == null || !await HasCounterPermission(Context.Message.Author.Id, counter))
        //        {
        //            await ReplySimpleEmbedAsync("Unknown counter. Please use the name or GUID");
        //            return;
        //        }
                
        //        var embedFields = TypedDbContext.CounterHistory.Include(x => x.Counter).Where(x => x.Counter.IdCounter == counter.IdCounter).Select(x => new EmbedFieldBuilder
        //        {
        //            Name = 
        //        })
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //    }
        //}

        private async Task<bool> HasCounterPermission(ulong userSnowflake, Counter counter)
        {
            if (counter == null)
            {
                return false;
            }

            if (counter.User.DiscordId == userSnowflake)
            {
                return true;
            }

            var relationship = await _relationshipService.GetRelationship(userSnowflake, counter.User.DiscordId);
            if (relationship.Status.HasFlag(OwnerSlaveRelationshipTypes.Tracker))
            {
                return true;
            }

            return false;
        }
    }
}