using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Helpers;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [RequireUserPrecondition(new [] { Constants.Users.EmpressKatie, Constants.Users.Slavestefan})]
    public class StartStopModule : DbModuleBase<PicAutoPostContext>
    {
        private readonly PostingServiceHost _postingHost;
        private readonly ILogger<StartStopModule> _logger;

        public StartStopModule(IServiceProvider services, PostingServiceHost postingHost, ILogger<StartStopModule> logger) : base(services)
        {
            _postingHost = postingHost;
            _logger = logger;
        }

        [Priority(10)]
        [Command("Start")]
        public async Task Start(ulong? channelId = null)
        {
            _logger.LogDebug($"Start ");
            channelId ??= Context.Channel.Id;
            var config = TypedDbContext.Configurations.FirstOrDefault(x => x.ChannelId == channelId && x.UserId == Context.Message.Author.Id);

            if (config?.IsRunning == true)
            {
                await Context.Channel.SendMessageAsync($"```I'm already active in this channel. My last post was at {config.LastPost:g}```");
                return;
            }

            if (config == null)
            {
                await Context.Channel.SendMessageAsync($"```Use the setup command to configure me for this channel first.```");
                return;
            }

            config.IsRunning = true;
            TypedDbContext.SaveChanges();
            await ReplyAsync("```Autoposting started```");
            _logger.LogInformation($"Autoposting started via command in channel {channelId} by user {Context.Message.Author.Id}");
            _postingHost.StartPostingService(config);
        }

        [Priority(20)]
        [Command("Setup")]
        public async Task Setup(int minInterval = 30, int maxInterval = 60, int minPostCount = 1, int maxPostCount = 3, ulong? channelId = null)
        {
            _logger.LogDebug($"Setup Command received in channel {Context.Channel.Id} by user {Context.Message.Author.Id}. Parameters: {minInterval} {maxInterval} {minPostCount} {maxPostCount} {channelId}");
            channelId ??= Context.Channel.Id;
            var config = await TypedDbContext.Configurations.FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == Context.Message.Author.Id);

            if (config == null)
            {
                config = new PostConfiguration
                {
                    ChannelId = Context.Channel.Id,
                    IdConfiguration = Guid.NewGuid(),
                    IsRunning = false,
                    MinPostingIntervalInMinutes = minInterval,
                    MaxPostingIntervalInMinutes = maxInterval,
                    MinPostPerInterval = minPostCount,
                    MaxPostPerInterval = maxPostCount,
                    UserId = Context.Message.Author.Id,
                };

                TypedDbContext.Configurations.Add(config);
            }
            else
            {
                config.MinPostingIntervalInMinutes = minInterval;
                config.MaxPostingIntervalInMinutes = maxInterval;
                config.MinPostPerInterval = minPostCount;
                config.MaxPostPerInterval = maxPostCount;
            }

            TypedDbContext.SaveChanges();
            await Context.Channel.SendMessageAsync("```Setup successful. Use !ap start to turn me on```");
            _logger.LogInformation($"Autoposting setup via command in channel {channelId} by user {Context.Message.Author.Id}. Values: {minInterval} {maxInterval} {minPostCount} {maxPostCount}");
        }

        [Command("Stop")]
        public async Task Stop(ulong? channelId = null)
        {

            channelId ??= Context.Channel.Id;
            var config = await TypedDbContext.Configurations.FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == Context.Message.Author.Id);
            if (config == null)
            {
                await ReplyAsync("```I'm currently not running here for you.```");
                return;
            }

            config.IsRunning = false;
            TypedDbContext.SaveChanges();

            _postingHost.StopPostingService(Context.Channel.Id);
            await ReplyAsync("```Autoposting stopped```");
        }

        [Command("Sith on")]
        public async Task ToggleSith(ulong? channelId = null)
        {
            channelId ??= Context.Channel.Id;

            var config = await TypedDbContext.Configurations.FirstOrDefaultAsync(x => x.ChannelId == channelId);

            if (config == null)
            {
                await Context.Channel.SendMessageAsync($"```Use the setup command to configure me for this channel first.```");
                return;
            }

            if (config.IsRunning)
            {
                await Context.Channel.SendMessageAsync($"```Use \"!ap start {channelId}\" to start me first```");
                return;
            }
        }
    }
}