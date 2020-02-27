using System;

namespace Slavestefan.Aphrodite.Common
{
    public static class ArrayExtensions
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
    }
}
