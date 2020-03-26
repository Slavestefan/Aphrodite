

using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Slavestefan.Aphrodite.Web.MessageHandler
{
    public abstract class MessageHandlerBase
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
                _logger.LogError($"Error handling message by {this.GetType().Name}" + ex);
                return false;
            }
        }

        protected abstract Task<bool> HandleWithExceptionHandling(SocketMessage message);
    }
}