

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
                var tokens = message.Split(' ');
                ulong? replyChannel = null;
                if (tokens[0].StartsWith("$"))
                {
                    switch (tokens[0])
                    {
                        case "$sarah":
                            replyChannel = 702184718103478309;
                            break;
                        case "$stefan":
                            replyChannel = 646857015787782180;
                            break;
                        case "$test":
                            replyChannel = 655835216174120961;
                            break;
                        default:
                            if (ulong.TryParse(tokens[0].Trim('$'), out var result))
                            {
                                replyChannel = result;
                            }
                            break;
                    }

                    tokens = tokens.Skip(1).ToArray();
                }

                if (replyChannel == null)
                {
                    replyChannel = _options.CurrentValue.SayChannelId;
                }
                var mentiontokens = tokens.Where(x => !string.IsNullOrEmpty(x) && x[0] == '@').Where(x => !string.Equals(x, "@everyone", StringComparison.OrdinalIgnoreCase));
                var mentionReplacePairs = mentiontokens.Select(x => new {Replacable = x, UserSnowflake = _bot.GuesstimateUser(x.Trim('@'), replyChannel.Value)}).ToList();
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
                    await _bot.SendRawMessage(string.Join(' ', tokens), replyChannel.Value);
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