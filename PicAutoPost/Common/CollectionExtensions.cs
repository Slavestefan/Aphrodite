using System;
using System.Collections.Generic;
using System.Linq;

namespace Slavestefan.Aphrodite.Common
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets a random element from Array
        /// </summary>
        public static T GetRandom<T>(this T[] source)
        {
            return source.GetRandom(new Random());
        }

        /// <summary>
        /// Gets a random element from Array. Use this to maintain seed stability.
        /// </summary>
        public static T GetRandom<T>(this T[] source, Random rng)
        {
            return source[rng.Next(0, source.Length)];
        }

        public static IList<T> GetRandomNonRepeating<T>(this IList<T> source, int amount)
            => source.GetRandomNonRepeating(new Random(), amount);


        public static IList<T> GetRandomNonRepeating<T>(this IList<T> source, Random rng, int amount)
        {
            if (source.Count() < amount)
            {
                throw new InvalidOperationException("Cannot get more non-repeating random elements than there are in source collection");
            }

            var skips = new List<int>();
            for (var i = 0; i < amount; ++i)
            {
                int roll;
                do
                {
                    roll = rng.Next(0, source.Count());
                } while (skips.Contains(roll));

                skips.Add(roll);
            }

            return skips.Select(x => source.Skip(x).First()).ToList();
        }
    }
}
