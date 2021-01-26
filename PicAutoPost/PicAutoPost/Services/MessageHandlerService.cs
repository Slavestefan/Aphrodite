using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Web.MessageHandler;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class MessageHandlerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ReadOnlyCollection<IDiscordMessageHandler> _handlers;

        public MessageHandlerService(IServiceProvider serviceProvider, ILogger<MessageHandlerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _handlers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(x => typeof(IDiscordMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(InstantiateHandler).ToList().AsReadOnly();
        }

        public async Task<bool> HandleMessage(SocketMessage message)
        {
            foreach (var handler in _handlers)
            {
                if (handler.WantsToHandle(message))
                {
                    if (await handler.Handle(message))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private IDiscordMessageHandler InstantiateHandler(Type handlerType)
        {
            if (!typeof(IDiscordMessageHandler).IsAssignableFrom(handlerType) || handlerType.IsInterface || handlerType.IsAbstract)
            {
                throw new ArgumentException(nameof(handlerType));
            }
            var ctors = handlerType.GetConstructors().Where(x => x.IsPublic).ToList();
            var scope = _serviceProvider.CreateScope();
            if (ctors.Count() > 1)
            {
                throw new Exception("Message handlers cannot have more than one ctor. Offending handler:" + handlerType.Name);
            }

            var ctor = ctors[0];
            var parameters = ctor.GetParameters();
            var paraInstances = new List<object>();
            foreach (var p in parameters)
            {
                var type = p.ParameterType;
                if (type == typeof(IServiceProvider))
                {
                    paraInstances.Add(_serviceProvider);
                    continue;
                }

                paraInstances.Add(scope.ServiceProvider.GetRequiredService(type));
            }

            return (IDiscordMessageHandler) ctor.Invoke(paraInstances.ToArray());
        }
    }
}
