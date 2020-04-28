using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Constants;
using Slavestefan.Aphrodite.Web.MessageHandler;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class Bot : BackgroundService
    {
        private readonly Discord.WebSocket.DiscordSocketClient _client;
        private readonly Random _rng = new Random();
        private readonly IServiceProvider _serviceProvider;
        private readonly string _botToken;
        private bool _antiSatinMode;
        private readonly CommandService _commands;
        private readonly Dictionary<ulong, (PostingService Service, IServiceScope Scope, CancellationTokenSource Token)> _postingServices = new Dictionary<ulong, (PostingService, IServiceScope, CancellationTokenSource)>();
        private Timer _changeGameTimer;
        private Timer _keepAlive;
        private DrivebyHandler _drivebyHandler;
        private CancellationToken _stopExecutionToken;

        public Bot(IServiceProvider serviceProvider, string botToken)
        {
            _client = new Discord.WebSocket.DiscordSocketClient();
            _serviceProvider = serviceProvider;
            _botToken = botToken;
            _commands = _serviceProvider.GetService<CommandService>();
        }

        private DiscordSocketClient Client
        {
            get
            {
                int i = 0;
                while (_client.ConnectionState == Discord.ConnectionState.Connecting && i < 100)
                {
                    ++i;
                    Thread.Sleep(1000);
                }

                if (_client.ConnectionState == Discord.ConnectionState.Connecting)
                {
                    throw new InvalidOperationException("Discord client not connected: Timeout while trying to connect");
                }

                if (_client.ConnectionState != Discord.ConnectionState.Connected|| _client.ConnectionState == Discord.ConnectionState.Disconnecting)
                {
                    throw new InvalidOperationException("Discord client disconnected");
                }

                return _client;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            _antiSatinMode = await scope.ServiceProvider.GetRequiredService<BotConfigService>().GetBoolValue(BotConfigKeys.MessWithSatinKey) ?? false;
            _drivebyHandler = new DrivebyHandler(scope.ServiceProvider.GetRequiredService<ILogger<DrivebyHandler>>());
            scope.Dispose();

            _stopExecutionToken = cancellationToken;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                services: _serviceProvider);

            _keepAlive = new Timer(KeepAlive, null, 1 * 60 * 1000, 1 * 60 * 1000);
            _changeGameTimer = new Timer(SetGame, null, 30*60*1000, 29 * 60 * 1000);

            while (!_stopExecutionToken.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(500), _stopExecutionToken);
            }
        }

        private Task StartPosting()
        {
            try
            {
                var postingService = _serviceProvider.GetService<PostingServiceHost>();
                postingService.StartupPosting();
            }
            catch (Exception ex)
            {
                using var scope = _serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Bot>>();
                logger.LogError("Error resuming autoposting at startup: " + ex);
            }

            return Task.CompletedTask;
        }

        private async void KeepAlive(object state)
        {
            using var client = new HttpClient();
            try
            {
                await client.GetAsync("https://picautopost20191212060808.azurewebsites.net/", _stopExecutionToken);
            }
            catch
            {
                // We don't actually care about any errors in here. Don't question it, the line above saves me 40€ a month.
            }
        }

        private Task Client_Log(Discord.LogMessage arg)
        {
            Debug.WriteLine(arg);
            return Task.CompletedTask;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += SetGame;
            _client.Ready += StartPosting;
            _client.Log += Client_Log;
            _client.MessageReceived += Client_MessageReceived;
            await _client.LoginAsync(Discord.TokenType.Bot, _botToken);
            await _client.StartAsync();
            await base.StartAsync(cancellationToken);
        }

        private async Task MessWithSatin(SocketMessage message)
        {
            var index = _rng.Next(0, Constants.Phrases.SatinReactions.Length);
            await message.Channel.SendMessageAsync($"```{Phrases.SatinReactions[index]}```");
        }

        //TODO: Clean this mess up
        private async Task Client_MessageReceived(SocketMessage message)
        {
            // Check if user is blocked.
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PicAutoPostContext>();
            if (dbContext.Users.Any(x => x.DiscordId == message.Author.Id && x.Status == Model.Users.UserStatus.Blocked))
            {
                return;
            }

            // Mess with Satin
            if (message.Author.Id == Users.PrincessSatin && _antiSatinMode && message.Content.ToLower().Contains("aphrodite"))
            {
                await MessWithSatin(message);   
                return;
            }

            // Driveby Handler TODO: User generic message handlers.
            if (_drivebyHandler.WantsToHandle(message))
            {
                await _drivebyHandler.Handle(message);
                return;
            }

            // TODO: Put this in 
            if (!(message is SocketUserMessage msg) || !msg.Content.StartsWith("!ap"))
            {
                return;
            }

            // TODO: Make a proper command for static options.
            if (msg.Content.ToLower() == "!ap antisatinmode")
            {
                _antiSatinMode = !_antiSatinMode;
                await msg.Channel.SendMessageAsync($"Satin mode {(_antiSatinMode ? "engaged" : "disengaged")}");
                await scope.ServiceProvider.GetRequiredService<BotConfigService>().SetBoolValue(BotConfigKeys.MessWithSatinKey, _antiSatinMode);
                return;
            }

            // Finally some code that actually belongs in this method.
            var context = new SocketCommandContext(_client, msg);

            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: 4,
                services: _serviceProvider);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _keepAlive?.Dispose();
            _changeGameTimer?.Dispose();


            await Client.LogoutAsync();
        }

        private async void SetGame(object state)
            => await SetGame();

        public async Task SetGame()
        {
            switch (_rng.Next(1,4))
            {
                case 1:
                    await Client.SetGameAsync("with lockees", null, Discord.ActivityType.Playing);
                    break;
                case 2:
                    await Client.SetGameAsync("to the screams of lockees", null, Discord.ActivityType.Listening);
                    break;
                case 3:
                    await Client.SetGameAsync("lockees squirm", null, Discord.ActivityType.Watching);
                    break;
            }
        }

        [Obsolete("Use SendMessageAsEmbed instead")]
        internal async Task SendMessage(string message, ulong channelId, Embed embed = null)
        {
            if (!(_client.GetChannel(channelId) is ISocketMessageChannel channel))
            {
                throw new ArgumentException($"Channel {channelId} not found");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                await channel.SendMessageAsync(string.Empty, embed: embed);
            }
            else
            {
                if (!message.StartsWith("```"))
                {
                    message = "```" + message;
                }

                if (!message.EndsWith("```"))
                {
                    message = message + "```";
                }

                await channel.SendMessageAsync(message, embed: embed);
            }
        }

        internal async Task SendMessageAsEmbed(string message, ulong channelId)
        {
            if (!(_client.GetChannel(channelId) is ISocketMessageChannel channel))
            {
                throw new ArgumentException($"Channel {channelId} not found");
            }

            var embed = new EmbedBuilder
            {
                Description = message,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder
                    {
                        Name = string.Empty,
                        Value = message
                    }
                }
            };

            await channel.SendMessageAsync(string.Empty, embed: embed.Build());
        }

        internal async Task SendRawMessage(string message, ulong channelId, Embed embed = null)
        {
            if (!(_client.GetChannel(channelId) is ISocketMessageChannel channel))
            {
                throw new ArgumentException($"Channel {channelId} not found");
            }

            await channel.SendMessageAsync(message, embed: embed);
        }

        public string GetChannelNameFromSnowflake(ulong channelId)
        {
            var channel = _client.GetChannel(channelId) as SocketGuildChannel;
            if (channel == null)
            {
                throw new ArgumentException("Channel not found or not a guild channel");
            }

            return channel.Name;
        }

        public ulong? GuesstimateUser(string name, ulong channelId)
        {
            if (!(_client.GetChannel(channelId) is SocketGuildChannel channel))
            {
                throw new ArgumentException($"Channel {channelId} not found");
            }

            var users = channel.Users.Where(x => x.Username.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (users.Count == 1)
            {
                return users[0].Id;
            }
            else
            {
                return null;
            }
        }
    }
}
