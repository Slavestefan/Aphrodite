﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class DebugModule : DbModuleBase<PicAutoPostContext>
    {
        private readonly Bot _bot;
        private readonly PostingServiceHost _postingServiceHost;

        public DebugModule(IServiceProvider services, Bot bot, PostingServiceHost postingServiceHost) : base(services)
        {
            _bot = bot;
            _postingServiceHost = postingServiceHost;
        }

        [Command("Status")]
        public async Task Status()
        {
            if (Context.Message.Author.Id != Constants.Users.Slavestefan && Context.Message.Author.Id != Constants.Users.EmpressKatie)
            {
                return;
            }

            var postingServiceStatus = _postingServiceHost.GetPostingServiceStatus();

            await ReplyAsync($"```Currently running {postingServiceStatus.Count} services: \n{string.Join(Environment.NewLine, postingServiceStatus.Select(x => $"ChannelId: {x.Item1} Running: {x.Item2} Timer: about {x.Item3.TotalMinutes} Minutes"))}```");
            await ReplyAsync($"```{TypedDbContext.Pictures.Count()} pictures in store.```");
        }
    }
}