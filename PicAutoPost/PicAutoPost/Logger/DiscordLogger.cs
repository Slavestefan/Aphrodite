using System;
using System.Collections.Concurrent;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Logger
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

            if (eventId == 20101  // EF 
                || eventId == 10403)
            {
                return;
            }

            var embedBuilder = new EmbedBuilder
            {
                Fields = new System.Collections.Generic.List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "EventId",
                        Value = eventId.Id
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Timestamp",
                        Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:ffff")
                    }
                }
            };
            var output = formatter(state, exception).Replace("`", string.Empty);
            var sendMessage = _bot.SendMessage($"{logLevel.ToString()} - {eventId.Id} - {formatter(state, exception)} - {exception}", _config.ChannelId);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
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
        public static DiscordLoggerConfiguration Config;
        private readonly IServiceProvider _services;
        private readonly ConcurrentDictionary<string, DiscordLogger> _loggers = new ConcurrentDictionary<string, DiscordLogger>();

        public DiscordLoggerProvider(DiscordLoggerConfiguration config, IServiceProvider services)
        {
            Config = config;
            _services = services;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new DiscordLogger(_services.GetService<Bot>(), Config));
        }
    }
}