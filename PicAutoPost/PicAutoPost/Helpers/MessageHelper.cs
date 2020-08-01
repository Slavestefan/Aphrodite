

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Discord.WebSocket;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Web.Helpers
{
    public static class MessageHelper
    {
        public static List<string> GetPictures(this SocketUserMessage msg)
        {
            List<string> picList = new List<string>();
            foreach (var embed in msg.Embeds)
            {
                picList.Add(embed.Url);
            }

            foreach (var attachment in msg.Attachments)
            {
                picList.Add(attachment.Url);
            }

            return picList.GroupBy(x => new { x }).Select(x => x.First()).ToList();
        }
    }
}