using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class DbModuleBase<TDbContext> : ModuleBase<SocketCommandContext> where TDbContext : DbContext, IDisposable
    {
        protected TDbContext TypedDbContext
        {
            get;
        }

        protected IServiceScope Scope
        {
            get;
        }

        public DbModuleBase(IServiceProvider services)
        {
            Scope = services.CreateScope();
            TypedDbContext = Scope.ServiceProvider.GetRequiredService<TDbContext>();
        }

        public void Dispose()
        {
            Scope.Dispose();
        }

        public async Task ReplySimpleEmbedAsync(string message, Color? color = null)
        {
            var embed = new EmbedBuilder
            {
                Color = color,
                Fields = new System.Collections.Generic.List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Reply",
                        Value = message
                    }
                }
            };

            await ReplySimpleEmbedAsync(embed);
        }

        public async Task ReplyWithDefaultErrorMessage()
        {
            await ReplySimpleEmbedAsync("An error has occured. My maintenance slave has been informed.");
        }

        public async Task ReplySimpleEmbedAsync(EmbedBuilder embed)
        {
            if (embed.Footer == null)
            {
                embed.Footer = new EmbedFooterBuilder
                {
                    Text = $"Request by {Context.User.Username}#{Context.User.Discriminator}"
                };
            }

            await ReplyAsync(embed: embed.Build());
        }

    }
}