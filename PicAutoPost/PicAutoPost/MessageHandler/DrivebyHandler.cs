
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Slavestefan.Aphrodite.Common;
using Slavestefan.Aphrodite.Web.Constants;

namespace Slavestefan.Aphrodite.Web.MessageHandler
{
    public class DrivebyHandler : IMessageHandler
    {
        private static readonly IEmote[] MainPostEmotes =
        {
            Emote.Parse("<:emoji_4:617017994744168763>"),
            Emote.Parse("<a:emoji_3:617016026051117084>"),
            Emote.Parse("<:emoji_17:628762814026350592>"),
            Emote.Parse("<:blueballschastity:572907056596647996>"),
        };

        private static readonly IEmote[] ReactionEmotes =
        {
            new Emoji("😂"),
            new Emoji("🤣"),
            new Emoji("😜")
        };

        private readonly ConcurrentDictionary<ulong, bool> _activeChannels = new ConcurrentDictionary<ulong,bool>();

        public bool WantsToHandle(SocketMessage message)
            => WantsToHandle(message as IUserMessage);

        public bool WantsToHandle(IUserMessage message)
        {
            if (message == null)
            {
                return false;
            }

            return IsMissedMessage(message) || IsSobMessage(message);
        }

        private bool IsMissedMessage(IUserMessage message)
        {
            return (message.Author.Id == Users.EmpressKatie || message.Author.Id == Users.Slavestefan) && (message.Content.Contains("@everyone") && (message.Content.ToLower().Contains("missed it")));
        }

        private bool IsSobMessage(IUserMessage message)
        {
            return _activeChannels.TryGetValue(message.Channel.Id, out var active) && active == true && message.Author.Id != Users.EmpressKatie && ContainsSomeKindOfSob(message);
        }

        private bool ContainsSomeKindOfSob(IUserMessage message)
        {
            var msg = message.Content.ToLower();
            return msg.Contains("sob")
                   || msg.Contains("cry")
                   || msg.Contains("no")
                   || msg.Contains("cruel")
                   || msg.Contains("😭")
                   || msg.Contains("😢");
        }


        public async Task<bool> Handle(SocketMessage message)
        {
            var userMessage = message as IUserMessage;
            if (userMessage == null)
            {
                throw new ArgumentException("Message must be of type UserMessage", nameof(message));
            }

            if (IsMissedMessage(userMessage))
            {
                await userMessage.AddReactionAsync(MainPostEmotes.GetRandom());
                var updated = _activeChannels.AddOrUpdate(userMessage.Channel.Id, true, (x, y) => true);
                RemoveAfterWait(userMessage.Channel.Id);
                return true;
            }

            if (IsSobMessage(userMessage))
            {
                await userMessage.AddReactionAsync(ReactionEmotes.GetRandom());
            }

            return false;
        }

        private async void RemoveAfterWait(ulong channelId)
        {
            await Task.Delay(60000);
            _activeChannels.AddOrUpdate(channelId, false, (x, y) => false);
        }
    }
}