

using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Net;
using Microsoft.Extensions.Logging;

namespace Slavestefan.Aphrodite.Web.MessageHandler
{
    public abstract class MessageHandlerBase : IDiscordMessageHandler
    {
        private readonly ILogger<MessageHandlerBase> _logger;

        protected MessageHandlerBase(ILogger<MessageHandlerBase> logger)
        {
            _logger = logger;
        }

        public abstract bool WantsToHandle(SocketMessage message);

        public virtual async Task<bool> Handle(SocketMessage message)
        {
            try
            {
                return await HandleWithExceptionHandling(message);
            }
            catch (Exception ex)
            {
                if (ex is HttpException httpex && httpex.HttpCode == System.Net.HttpStatusCode.Forbidden 
                    || ex.InnerException is HttpException httpexInner && httpexInner.HttpCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogInformation($"Couldn't handle message, likely blocked by user {message.Author.Id}");
                    return false;
                }

                _logger.LogError($"Error handling message by {this.GetType().Name}" + ex);
                return false;
            }
        }

        protected abstract Task<bool> HandleWithExceptionHandling(SocketMessage message);
    }
}