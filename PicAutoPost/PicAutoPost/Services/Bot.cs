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
using PicAutoPost.Constants;
using Slavestefan.PicAutoPost.Model;

namespace PicAutoPost.Services
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
            Debug.WriteLine("Executing");
            _client.Ready += SetGame;
            _client.Ready += StartPosting;
            _client.Log += Client_Log;
            _client.MessageReceived += Client_MessageReceived;
            _stopExecutionToken = cancellationToken;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                services: _serviceProvider);

            _keepAlive = new Timer(KeepAlive, null, 5 * 60 * 1000, 5 * 60 * 1000);
            _changeGameTimer = new Timer(SetGame, null, 30*60*1000, 29 * 60 * 1000);

            while (!_stopExecutionToken.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(500), _stopExecutionToken);
            }
        }

        private Task StartPosting()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PicAutoPostContext>();
            var configurations = dbContext.Configurations.Where(x => x.IsRunning).ToList();

            foreach (var config in configurations)
            {
                StartPostingService(config.ChannelId);
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
                // We don't actually care about any errors in here.
            }
        }

        private Task Client_Log(Discord.LogMessage arg)
        {
            Debug.WriteLine(arg);
            return Task.CompletedTask;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.LoginAsync(Discord.TokenType.Bot, _botToken);
            await _client.StartAsync();
            await base.StartAsync(cancellationToken);
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == 474751258834501633 && _antiSatinMode && message.Content.ToLower().Contains("aphrodite"))
            {
                await message.Channel.SendMessageAsync("```Shoo Satin!```");
                return;
            }

            if (!(message is SocketUserMessage msg) || !msg.Content.StartsWith("!ap"))
            {
                return;
            }
            
            if (!(msg.Author.Id == Users.EmpressKatie || msg.Author.Id == Users.Slavestefan))
            {
                await msg.Channel.SendMessageAsync("You don't have the permission to use me ;)");
                return;
            }

            if (msg.Content.ToLower() == "!ap antisatinmode")
            {
                _antiSatinMode = !_antiSatinMode;
                await msg.Channel.SendMessageAsync($"Satin mode {(_antiSatinMode ? "engaged" : "disengaged")}");
                return;
            }

            var context = new SocketCommandContext(_client, msg);

            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: 4,
                services: _serviceProvider);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _keepAlive.Dispose();
            _changeGameTimer.Dispose();
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

        internal async Task SendMessage(string message, ulong channelId, Embed embed = null)
        {
            var channel = _client.GetChannel(channelId) as ISocketMessageChannel;
            if (channel == null)
            {
                throw new ArgumentException($"Channel {channelId} not found");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                await channel.SendMessageAsync(string.Empty, embed: embed);
            }
            else
            {
                await channel.SendMessageAsync($"```{message}```", embed: embed);
            }
        }

        internal void StartPostingService(ulong channelId)
        {
            (PostingService Service, IServiceScope Scope, CancellationTokenSource TokenSource) tuple;
            if (_postingServices.ContainsKey(channelId))
            {
                tuple = _postingServices[channelId];
                if (tuple.Service.IsRunning)
                {
                    return;
                }

                tuple.TokenSource = new CancellationTokenSource();
            }
            else
            {
                tuple.Scope = _serviceProvider.CreateScope();
                tuple.Service = tuple.Scope.ServiceProvider.GetRequiredService<PostingService>();
                tuple.TokenSource = new CancellationTokenSource();
                _postingServices.Add(channelId, tuple);
            }

            tuple.Service.Start(channelId, tuple.TokenSource.Token);
        }

        internal void StopPostingService(ulong channelId)
        {
            if (!_postingServices.ContainsKey(channelId))
            {
                return;
            }

            var (service, scope, tokenSource) = _postingServices[channelId];
            tokenSource.Cancel();
            service.Stop();
            scope.Dispose();
            _postingServices.Remove(channelId);
        }

        internal List<(ulong, bool, TimeSpan)> GetPostingServiceStatus()
        {
            return _postingServices.Select(x => (x.Key, x.Value.Service.IsRunning, x.Value.Service.GetTimeUntilNextPost())).ToList();
        }
    }
}
