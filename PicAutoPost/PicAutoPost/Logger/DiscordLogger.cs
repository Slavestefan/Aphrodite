using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PicAutoPost.Services;

namespace PicAutoPost.Logger
{
    public class DiscordLogger : ILogger
    {
        private readonly Bot _bot;
        private readonly DiscordLoggerConfiguration _config;
        


        public DiscordLogger(Bot bot, DiscordLoggerConfiguration config)
        {
            _bot = bot;
            _config = config;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var sendMessage = _bot.SendMessage($"{logLevel.ToString()} - {eventId.Id} - {formatter(state, exception)}", _config.ChannelId);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class DiscordLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public ulong ChannelId { get; set; }        
    }

    public class DiscordLoggerProvider : ILoggerProvider
    {
        private readonly DiscordLoggerConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly ConcurrentDictionary<string, DiscordLogger> _loggers = new ConcurrentDictionary<string, DiscordLogger>();

        public DiscordLoggerProvider(DiscordLoggerConfiguration config, IServiceProvider services)
        {
            _config = config;
            _services = services;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new DiscordLogger(_services.GetService<Bot>(), _config));
        }
    }
}