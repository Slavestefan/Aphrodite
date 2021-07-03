using System;
using System.Threading.Tasks;
using Discord.Commands;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Random")]
    public class RandomModule : AphroditeModuleBase
    {
        private readonly RandomService _rng;

        public RandomModule(RandomService rng, PicAutoPostContext context, BotConfigService configService) : base(context, configService)
        {
            _rng = rng;
        }

        [Command("Number")]
        public async Task Number(int max)
            => await Number(1, max);

        [Command("Number")]
        public async Task Number(int min, int max)
        {
            await ReplySimpleEmbedAsync(_rng.Rng.Next(min, max +1).ToString());
        }
    }
}