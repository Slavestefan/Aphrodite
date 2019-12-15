using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Slavestefan.PicAutoPost.Model;

namespace Slavestefan.PicAutoPost.Bot
{
    public class Bot : IHostedService
    {
        private readonly Discord.WebSocket.DiscordSocketClient _client;
        private readonly PicAutoPostContext _context;

        public Bot(PicAutoPostContext context)
        {
            _client = new Discord.WebSocket.DiscordSocketClient();
            _context = context;
            Task.Run(Start);
        }

        public async Task Start()
        {
            await _client.LoginAsync(Discord.TokenType.Bot, "NjU0NTIzNjUzMzY2NjExOTg0.XfG9rg.C-tmUL-D4hzOozYgUiHnf4XC-6w");
            await _client.StartAsync();
            _client.Ready += _client_Ready;
        }

        private Task _client_Ready()
        {
            var channel = _client.GetChannel(571110713037291521) as SocketTextChannel;
            channel?.SendMessageAsync("This is a test");
            return Task.CompletedTask;
        }
    }
}
