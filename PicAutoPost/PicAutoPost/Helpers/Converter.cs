using Discord;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Web.Helpers
{
    public static class Converter
    {
        public static Embed ToEmbed(Picture pic)
        {
            var builder = new EmbedBuilder
            {
                ImageUrl = pic.Location.ToString(),
            };

            return builder.Build();
        }
    }
}