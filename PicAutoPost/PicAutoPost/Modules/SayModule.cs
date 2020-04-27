﻿

using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slavestefan.Aphrodite.Web.Helpers;
using Slavestefan.Aphrodite.Web.Options;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [RequireUserPrecondition(new[] { Constants.Users.EmpressKatie, Constants.Users.Slavestefan})]
    public class SayModule : AphroditeModuleBase
    {
        private readonly IOptionsMonitor<SayOptions> _options;
        private readonly ILogger<SayModule> _logger;
        private readonly Bot _bot;

        public SayModule(IServiceProvider services, ILogger<SayModule> logger, Bot bot, IOptionsMonitor<SayOptions> options) : base(services)
        {
            _options = options;
            _logger = logger;
            _bot = bot;
        }

        [Command("Say")]
        public async Task Say([Remainder] string message)
        {
            try
            {
                var channelId = _options.CurrentValue.SayChannelId;
                var mentiontokens = message.Split(' ').Where(x => !string.IsNullOrEmpty(x) && x[0] == '@').Where(x => !string.Equals(x, "@everyone", StringComparison.OrdinalIgnoreCase));
                var mentionReplacePairs = mentiontokens.Select(x => new {Replacable = x, UserSnowflake = _bot.GuesstimateUser(x.Trim('@'), channelId)}).ToList();
                bool notFound = false;
                foreach (var alias in mentionReplacePairs)
                {
                    if (!alias.UserSnowflake.HasValue)
                    {
                        notFound = true;
                        break;
                    }

                    message = message.Replace(alias.Replacable, $"<@{alias.UserSnowflake}>");
                }

                if (!notFound)
                {
                    await _bot.SendRawMessage(message, channelId);
                    await ReplyAsync("Message sent");
                }
                else
                {
                    var answer = $"Could not find user with name(s) {string.Join(',', mentionReplacePairs.Where(x => x.UserSnowflake == null).Select(x => x.Replacable))}";
                    await ReplyAsync(answer);
                    _logger.LogWarning(answer);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not say phrase {message}: {ex.ToString()}");
            }
        }
    }
}