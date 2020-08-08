

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Users;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class AphroditeModuleBase : DbModuleBase<PicAutoPostContext>
    {
        protected BotConfigService BotConfigService
        {
            get;
        }

        public AphroditeModuleBase(IServiceProvider services) : base(services)
        {
            BotConfigService = Scope.ServiceProvider.GetRequiredService<BotConfigService>();
        }

        protected User GetUserFromSnowflakeOrUsernameOrMention(string userDescriptor)
        {
            if (userDescriptor.StartsWith("<@!"))
            {
                userDescriptor = userDescriptor.Trim('<', '@', '!', '>');
            }

            if (ulong.TryParse(userDescriptor, out var snowflake))
            {
                return TypedDbContext.Users.FirstOrDefault(x => x.DiscordId == snowflake);
            }

            return TypedDbContext.Users.FirstOrDefault(x => x.Username == userDescriptor);
        }
    }
}