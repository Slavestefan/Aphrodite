using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Web.MessageHandler;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class MessageHandlerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ReadOnlyCollection<Type> _handlers;

        public MessageHandlerService(IServiceProvider serviceProvider, ILogger<MessageHandlerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _handlers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(x => x.IsAssignableFrom(typeof(IDiscordMessageHandler))).ToList().AsReadOnly();
        }

        //private IDiscordMessageHandler InstantiateHandler(Type handlerType)
        //{
        //    var ctors = handlerType.GetConstructors();
        //    _serviceProvider.CreateScope();
        //}
    }
}
