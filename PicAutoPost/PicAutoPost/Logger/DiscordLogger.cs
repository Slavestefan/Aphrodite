using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Discord;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Logger
{
    public class DiscordLogger : ILogger
    {
        private readonly Bot _bot;
        private readonly DiscordLoggerConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DiscordLogger(Bot bot, DiscordLoggerConfiguration config, IWebHostEnvironment hostingEnvironment)
        {
            _bot = bot;
            _config = config;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (eventId == 20101  // EF 
                || eventId == 10403
                || eventId == 20001 // EF Connection open
                || eventId == 20002 // EF Connection close
                || eventId == 20003) // EF Connection closed
            {
                return;
            }

            var message = formatter(state, exception).Replace("`", string.Empty);
            string path = null;
            if (message.Length > 1024)
            {
                path = @$"logs\StackTrace-{DateTime.UtcNow:yyyyMMddHHmmssfff}.txt";
                File.WriteAllText(_hostingEnvironment.WebRootPath +"\\" + path, message);
                message = message.Substring(0, 1024);
            }

            var embed = new EmbedBuilder
            {
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder {Name = "UTC Timestamp", Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff"), IsInline = true},
                    new EmbedFieldBuilder {Name = "LogLevel", Value = logLevel.ToString(), IsInline = true},
                    new EmbedFieldBuilder {Name = "Event", Value = $"{eventId.Id} - {eventId.Name}", IsInline = true},
                    new EmbedFieldBuilder {Name = "Message", Value = message, IsInline = true},
                },
                Color = GetColorByLogLevel(logLevel),
            };
            
            if (path != null)
            {
                embed.Fields.Add(new EmbedFieldBuilder()
                {
                    Name = "FullMessage", Value = Constants.Urls.BaseAppUrl +"/" +  path.Replace("\\", "/")
                });
            }

            var sendMessage = _bot.SendRawMessage(logLevel >= LogLevel.Warning ? $"Hey! Listen! <@{Constants.Users.Slavestefan}>" : string.Empty, _config.ChannelId, embed.Build());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
        }

        private Color GetColorByLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return new Color(0, 0, 255);
                case LogLevel.Information:
                    return new Color(0, 255, 0);
                case LogLevel.Warning:
                    return new Color(0, 255, 255);
                case LogLevel.Error:
                case LogLevel.Critical:
                    return new Color(255, 0, 0);
                case LogLevel.None:
                    return new Color(0, 0, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class DiscordLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public int EventId { get; set; } = 0;
        public ulong ChannelId { get; set; }        
    }

    public class DiscordLoggerProvider : ILoggerProvider
    {
        public static DiscordLoggerConfiguration Config;
        private readonly IServiceProvider _services;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConcurrentDictionary<string, DiscordLogger> _loggers = new ConcurrentDictionary<string, DiscordLogger>();

        public DiscordLoggerProvider(DiscordLoggerConfiguration config, IServiceProvider services, IWebHostEnvironment hostingEnvironment)
        {
            Config = config;
            _services = services;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new DiscordLogger(_services.GetService<Bot>(), Config, _hostingEnvironment));
        }
    }
}