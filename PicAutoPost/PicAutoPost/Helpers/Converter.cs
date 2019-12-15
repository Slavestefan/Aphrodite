using Discord;
using Slavestefan.PicAutoPost.Model;

namespace PicAutoPost.Helpers
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