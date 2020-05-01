using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Web.Modules
{
    //TODO: Combine into picture Module
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
            try
            {
                _logger.LogDebug("Add Pic called by user " + Context.Message.Author.Id);

                if (!(Context.Message.Embeds.Any() || Context.Message.Attachments.Any()))
                {
                    await ReplyAsync("```No picture found```");
                    return;
                }

                var user = TypedDbContext.Users.AsQueryable().FirstOrDefault(x => x.DiscordId == Context.Message.Author.Id);
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

                List<Picture> picList = new List<Picture>();
                foreach (var embed in Context.Message.Embeds)
                {
                    picList.Add(await ExtractImage(embed.Url, user));
                }

                foreach (var attachment in Context.Message.Attachments)
                {
                    picList.Add(await ExtractImage(attachment.Url, user));
                }

                picList = picList.GroupBy(x => new { x.Hash, x.User.IdUser }).Select(x => x.First()).ToList();

                var duplicates = picList.Where(x => TypedDbContext.Pictures.Include(y => y.User).Any(p => p.Hash == x.Hash && p.User.IdUser == x.User.IdUser)).ToList();
                foreach (var dup in duplicates)
                {
                    picList.Remove(dup);
                }

                TypedDbContext.Pictures.AddRange(picList);

                var output = $"{picList.Count} picture{(picList.Count != 1 ? "s" : "")} added. {(duplicates.Count == 0 ? string.Empty : $"Skipped {duplicates.Count} duplicates")}";
                TypedDbContext.SaveChanges();
                await ReplyAsync("```" + output + "```");
                _logger.LogInformation(output + $" By user {Context.Message.Author.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while adding pics: " + ex.ToString());
            }
        }

        private async Task<Picture> ExtractImage(string url, User user)
        {
            using var sha2 = SHA256.Create();
            var pic = new Picture
            {
                Location = new Uri(url),
                User = user,
            };

            var response = await HttpClient.GetAsync(pic.Location);
            pic.Raw = await response.Content.ReadAsByteArrayAsync();
            pic.Hash = sha2.ComputeHash(pic.Raw);
            return pic;
        }
    }
}