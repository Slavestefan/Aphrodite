

using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Common;
using Slavestefan.Aphrodite.Web.Constants;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.MessageHandler
{
    public class SatinHandler : MessageHandlerBase
    {
        private readonly RandomService _rng;

        public SatinHandler(ILogger<MessageHandlerBase> logger, RandomService rng) : base(logger)
        {
            _rng = rng;
        }

        public override bool WantsToHandle(SocketMessage message)
            => WantsToHandle(message as IUserMessage);

        public bool WantsToHandle(IUserMessage message)
        {
            if (message == null)
            {
                return false;
            }

            if (message.Author.Id == Constants.Users.PrincessSatin && message.Content.Contains("Aphrodite"))
            {
                return true;
            }

            return false;
        }

        protected override async Task<bool> HandleWithExceptionHandling(SocketMessage message)
        {
            var result = Phrases.SatinReactions.GetRandom();
            await message.Channel.SendMessageAsync(result);
            if (_rng.Rng.Next(1,5) == 1)
            {
                await message.Channel.SendMessageAsync(Phrases.SatinRoll1);
                await message.Channel.SendMessageAsync(Phrases.SatinRoll2);
            }

            return true;
        }
    }
}