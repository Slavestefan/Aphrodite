using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Users;
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

        public DebugModule(PicAutoPostContext context, Bot bot, PostingServiceHost postingServiceHost, ILogger<DebugModule>  logger) : base(context)
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
            await ReplyAsync($"Running version {assembly.GetName().Version} build date {new FileInfo(assembly.Location).CreationTimeUtc:yyyy-MM-dd} targeting Framework {AppContext.TargetFrameworkName} ");
        }

        [Command("Block")]
        public async Task BlockUser(ulong userSnowflake, bool block = true)
        {
            var user = TypedDbContext.Users.FirstOrDefault(x => x.DiscordId == userSnowflake);

            if (user == null)
            {
                user = new User()
                {
                    DiscordId = userSnowflake
                };

                TypedDbContext.Users.Add(user);
            }

            user.Status = Slavestefan.Aphrodite.Model.Users.UserStatus.Blocked;
            TypedDbContext.SaveChanges();
            await ReplyAsync($"Successfully blocked user {userSnowflake}");
        }

        [Command("Dump")]
        [RequireUserPrecondition(new[] { Constants.Users.Slavestefan })]
        public async Task Dump(ulong channelSnowflake, int messageCount)
        {

            ISocketMessageChannel channel = null;
            try
            {
                channel = (ISocketMessageChannel)_bot.Client.GetChannel(channelSnowflake);
                if (channel == null)
                {
                    await Context.Message.Author.SendMessageAsync("Channel not found");
                    return;
                }

                var msgs = await channel.GetMessagesAsync(messageCount).FlattenAsync();
                var sb = new StringBuilder();
                foreach (var msg in msgs)
                {
                    sb.Append(msg.CreatedAt.ToString("yyyyMMddHHmmss"));
                    sb.Append(" ");
                    sb.Append(msg.Author.Username);
                    sb.Append(": ");
                    sb.AppendLine(msg.Content);
                }

                File.WriteAllText(channelSnowflake + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", sb.ToString());
            }
            catch (Exception ex)
            {
                await Context.Message.Author.SendMessageAsync(("Exception while searching for channel" + ex).Substring(0, 200));
            }
        }

        [Command("Echo")]
        public async Task Echo([Remainder] string content)
        {
            await base.ReplySimpleEmbedAsync(content);
            
        }
    }
}