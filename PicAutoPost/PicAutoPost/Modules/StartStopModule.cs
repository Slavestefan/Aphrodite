using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class StartStopModule : DbModuleBase<PicAutoPostContext>
    {
        private readonly Bot _bot;

        public StartStopModule(IServiceProvider services, Bot bot) : base(services)
        {
            _bot = bot;
        }

        [Priority(10)]
        [Command("Start")]
        public async Task Start()
        {
            var config = TypedDbContext.Configurations.FirstOrDefault(x => x.ChannelId == Context.Channel.Id && x.UserId == Context.Message.Author.Id);

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
            _bot.StartPostingService(Context.Channel.Id);
        }

        [Priority(20)]
        [Command("Setup")]
        public async Task Setup(int minInterval = 30, int maxInterval = 60, int minPostCount = 1, int maxPostCount = 3)
        {
            var config = await TypedDbContext.Configurations.FirstOrDefaultAsync(x => x.ChannelId == Context.Channel.Id && x.UserId == Context.Message.Author.Id);

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
        }

        [Command("Stop")]
        public async Task Stop()
        {
            _bot.StopPostingService(Context.Channel.Id);
            await ReplyAsync("```Autoposting stopped```");
        }
    }
}