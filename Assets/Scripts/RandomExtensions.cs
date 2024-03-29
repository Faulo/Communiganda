using System;
using System.Linq;

namespace Communiganda {
    public static class RandomExtensions {
        static readonly Random random = new Random();

        public static T RandomElement<T>(this T[] enumerable) {
            if (enumerable == null || enumerable.Length == 0) {
                throw new ArgumentNullException("enumerable");
            }

            return enumerable.ElementAt(random.Next(enumerable.Length));
        }

        public static bool Contains<T>(this T[] haystack, T needle) {
            foreach (var item in haystack) {
                if (item.Equals(needle)) {
                    return true;
                }
            }
            return false;
        }

        public static void Shuffle<T>(this Random rng, T[] array) {
            int n = array.Length;
            while (n > 1) {
                int k = rng.Next(n--);
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}