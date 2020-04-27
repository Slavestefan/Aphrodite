using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private readonly ILogger<DebugModule> _logger;

        public DebugModule(IServiceProvider services, Bot bot, PostingServiceHost postingServiceHost, ILogger<DebugModule>  logger) : base(services)
        {
            _bot = bot;
            _postingServiceHost = postingServiceHost;
            _logger = logger;
        }

        [Command("Status")]
        public async Task Status()
        {
            var postingServiceStatus = _postingServiceHost.GetPostingServiceStatus();

            await ReplyAsync($"```Currently running {postingServiceStatus.Count} services: \n{string.Join(Environment.NewLine, postingServiceStatus.Select(x => $"ChannelId: {x.Item1}, ChannelName: {_bot.GetChannelNameFromSnowflake(x.Item1)}, Running: {x.Item2}, Timer: about {x.Item3.TotalMinutes} Minutes"))}```")
                    .ContinueWith(x => ReplyAsync($"```{TypedDbContext.Pictures.AsQueryable().Count()} pictures in store.```"))
                    .ContinueWith(x => ReplyAsync($"```LogLevel: {DiscordLoggerProvider.Config.LogLevel}```"));
        }

        [Command("LogLevel")]
        public async Task SetLogLevel(int level)
        {
            DiscordLoggerProvider.Config.LogLevel = (LogLevel)level;
            await ReplyAsync($"```Log level set to {DiscordLoggerProvider.Config.LogLevel}```");
            _logger.LogInformation($"Log level changed to {DiscordLoggerProvider.Config.LogLevel} by {Context.User.Id}");
        }

        [Command("Version")]
        public async Task GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            await ReplyAsync($"Running version {assembly.GetName().Version} build date {new FileInfo(assembly.Location).CreationTimeUtc:yyyy-MM-dd} running in CLR Version {assembly.ImageRuntimeVersion} ");
        }
    }
}