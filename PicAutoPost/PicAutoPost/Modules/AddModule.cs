using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class AddModule : DbModuleBase<PicAutoPostContext>, IDisposable
    {
        private readonly ILogger<DbModuleBase<PicAutoPostContext>> _logger;
        private HttpClient _client;

        public AddModule(IServiceProvider services, ILogger<AddModule> logger) : base(services)
        {
            _logger = logger;
        }

        private HttpClient HttpClient
            => _client ??= new HttpClient();

        [Command("Add")]
        public async Task AddPic()
        {
            _logger.LogDebug("Add Pic called by user " + Context.Message.Author.Id);
            if (!(Context.Message.Embeds.Any() || Context.Message.Attachments.Any()))
            {
                await ReplyAsync("```No picture found```");
                return;
            }

            var user = TypedDbContext.Users.FirstOrDefault(x => x.DiscordId == Context.Message.Author.Id);
            if (user == null)
            {
                user = new User()
                {
                    DiscordId = Context.Message.Author.Id,
                    IdUser = Guid.NewGuid(),
                    Username = Context.Message.Author.Username
                };
                TypedDbContext.Users.Add(user);
            }

            var count = 0;
            foreach (var embed in Context.Message.Embeds)
            {
                TypedDbContext.Pictures.Add(await ExtractImage(embed.Url, user));
                count++;
            }

            foreach (var attachment in Context.Message.Attachments)
            {
                TypedDbContext.Pictures.Add(await ExtractImage(attachment.Url, user));
                count++;
            }

            TypedDbContext.SaveChanges();
            await ReplyAsync($"```{count} picture{(count > 1 ? "s" : "")} added```");
            _logger.LogInformation($"```{count} picture{(count > 1 ? "s" : "")} added by user {Context.Message.Author.Id}```");
        }

        private async Task<Picture> ExtractImage(string url, User user)
        {
            var pic = new Picture
            {
                Location = new Uri(url),
                User = user,
            };

            var response = await HttpClient.GetAsync(pic.Location);
            pic.Raw = await response.Content.ReadAsByteArrayAsync();

            return pic;
        }
    }
}