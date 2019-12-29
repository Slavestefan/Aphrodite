using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Helpers;
using Slavestefan.Aphrodite.Web.Logger;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Debug")]
    [RequireUserPrecondition(new[] { Constants.Users.EmpressKatie, Constants.Users.Slavestefan})]
    public class DebugModule : DbModuleBase<PicAutoPostContext>
    {
        private readonly Bot _bot;
        private readonly PostingServiceHost _postingServiceHost;

        public DebugModule(IServiceProvider services, Bot bot, PostingServiceHost postingServiceHost) : base(services)
        {
            _bot = bot;
            _postingServiceHost = postingServiceHost;
        }

        [Command("Status")]
        public async Task Status()
        {
            var postingServiceStatus = _postingServiceHost.GetPostingServiceStatus();

            await ReplyAsync($"```Currently running {postingServiceStatus.Count} services: \n{string.Join(Environment.NewLine, postingServiceStatus.Select(x => $"ChannelId: {x.Item1}, ChannelName: {_bot.GetChannelNameFromSnowflake(x.Item1)}, Running: {x.Item2}, Timer: about {x.Item3.TotalMinutes} Minutes"))}```")
                    .ContinueWith(x => ReplyAsync($"```{TypedDbContext.Pictures.Count()} pictures in store.```"))
                    .ContinueWith(x => ReplyAsync($"```LogLevel: {DiscordLoggerProvider.Config.LogLevel}```"));
        }

        [Command("LogLevel")]
        public async Task SetLogLevel(int level)
        {
            DiscordLoggerProvider.Config.LogLevel = (LogLevel)level;
            await ReplyAsync($"```Log level set to {DiscordLoggerProvider.Config.LogLevel}```");
        }
    }
}