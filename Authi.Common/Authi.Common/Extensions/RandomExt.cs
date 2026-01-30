using System;
using System.Collections.Generic;

namespace Authi.App.Logic
{
    internal static class RandomExt
    {
        public static Random Current => _lazyCurrent.Value;

        private static readonly Lazy<Random> _lazyCurrent = new(() => new Random(DateTime.Now.Millisecond));

        public static bool NextBool(this Random random, double chance = 0.5d)
            => random.NextDouble() < chance;

        public static float NextFloat(this Random random, float min, float max)
            => (float)random.NextDouble(min, max);

        public static T NextItem<T>(this Random random, List<T> list)
            => list[random.Next(0, list.Count)];

        public static double NextDouble(this Random random, double min, double max)
        {
            var sys = random.NextDouble();
            var range = max - min;
            var result = sys * range + min;
            if (result < min) return min;
            if (result > max) return max;
            return result;
        }
    }
}
