

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class BotConfigService
    {
        private readonly PicAutoPostContext _context;
        private readonly ILogger<BotConfigService> _logger;

        public BotConfigService(PicAutoPostContext context, ILogger<BotConfigService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GetStringValue(string key, ulong channelId = 0)
            => (await GetConfig(key, channelId))?.ValueString;
        public async Task<int?> GetIntValue(string key, ulong channelId = 0)
            => (await GetConfig(key, channelId))?.ValueInt;
        public async Task<bool?> GetBoolValue(string key, ulong channelId = 0)
            => (await GetConfig(key, channelId))?.ValueBool;
        public async Task<ulong?> GetUlongValue(string key, ulong channelId = 0)
            => (await GetConfig(key, channelId))?.ValueUlong;

        public async Task SetStringValue(string key, string value, ulong channelId = 0)
        {
            var config = await GetConfig(key, channelId);
            string oldValue = null;
            if (config == null)
            {
                config = new BotConfiguration
                {
                    ChannelId = channelId,
                    Key = key,
                    ValueString = value
                };

                _context.BotConfigurations.Add(config);
            }
            else
            {
                oldValue = config.ValueString;
            }

            config.ValueString = value;
            _context.SaveChanges();
            _logger.LogDebug($"Changed config {key} for channelId {channelId} from {oldValue} to {value}");
        }

        public async Task SetUlongValue(string key, ulong value, ulong channelId = 0)
        {
            var config = await GetConfig(key, channelId);
            ulong? oldValue = null;
            if (config == null)
            {
                config = new BotConfiguration
                {
                    ChannelId = channelId,
                    Key = key,
                    ValueUlong = value
                };

                _context.BotConfigurations.Add(config);
            }
            else
            {
                oldValue = config.ValueUlong;
            }

            config.ValueUlong = value;
            _context.SaveChanges();
            _logger.LogDebug($"Changed config {key} for channelId {channelId} from {oldValue} to {value}");
        }

        public async Task SetIntValue(string key, int value, ulong channelId = 0)
        {
            var config = await GetConfig(key, channelId);
            int? oldValue = null;
            if (config == null)
            {
                config = new BotConfiguration
                {
                    ChannelId = channelId,
                    Key = key,
                    ValueInt = value
                };

                _context.BotConfigurations.Add(config);
            }
            else
            {
                oldValue = config.ValueInt;
            }

            config.ValueInt = value;
            _context.SaveChanges();
            _logger.LogDebug($"Changed config {key} for channelId {channelId} from {oldValue} to {value}");
        }
        public async Task SetBoolValue(string key, bool value, ulong channelId = 0)
        {
            var config = await GetConfig(key, channelId);
            bool? oldValue = null;
            if (config == null)
            {
                config = new BotConfiguration
                {
                    ChannelId = channelId,
                    Key = key,
                    ValueBool = value
                };

                _context.BotConfigurations.Add(config);
            }
            else
            {
                oldValue = config.ValueBool;
            }

            config.ValueBool = value;
            _context.SaveChanges();
            _logger.LogDebug($"Changed config {key} for channelId {channelId} from {oldValue} to {value}");
        }


        private async Task<BotConfiguration> GetConfig(string key, ulong channelId = 0)
            => await _context.BotConfigurations.FirstOrDefaultAsync(x => x.Key == key && x.ChannelId == channelId);
    }
}