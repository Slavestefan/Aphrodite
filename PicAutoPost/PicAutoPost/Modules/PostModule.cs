using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Helpers;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class PostModule : DbModuleBase<PicAutoPostContext>
    {
        private readonly RandomService _rng;

        public PostModule(RandomService rng, PicAutoPostContext context) : base(context)
        {
            _rng = rng;
        }

        [Command("Post")]
        public async Task Post(int count = 1)
        {
            var pics = TypedDbContext.Pictures.Include(x => x.User).Where(x => x.User.DiscordId == Context.Message.Author.Id);
            for (var i = 0; i < count; i++)
            {
                var skip = _rng.Rng.Next(0, pics.Count());
                await Context.Message.Channel.SendMessageAsync(embed: Converter.ToEmbed(pics.Skip(skip).Take(1).First()));
            }
        }
    }
}