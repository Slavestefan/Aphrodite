using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using PicAutoPost.Helpers;
using PicAutoPost.Services;
using Slavestefan.PicAutoPost.Model;

namespace PicAutoPost.Modules
{
    public class PostModule : DbModuleBase<PicAutoPostContext>, IDisposable
    {
        private readonly RandomService _rng;

        public PostModule(RandomService rng, IServiceProvider services) : base(services)
        {
            _rng = rng;
        }

        [Command("Post")]
        public async Task Post([Remainder] string foo = "")
        {
            var pics = TypedDbContext.Pictures.Include(x => x.User).Where(x => x.User.DiscordId == Context.Message.Author.Id);
            var skip = _rng.Rng.Next(0, pics.Count());
            await Context.Message.Channel.SendMessageAsync(embed: Converter.ToEmbed(pics.Skip(skip).Take(1).First()));
        }
    }
}