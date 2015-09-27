using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PathFillTypeConverter.Extensions
{
    static class Linq
    {
        public static void AddRange<T>([NotNull] this ICollection<T> self, [NotNull] IEnumerable<T> collection)
        {
            var list = self as List<T>;
            if (list != null)
            {
                list.AddRange(collection);
                return;
            }
            foreach (var item in collection)
            {
                self.Add(item);
            }
        }

        public static TSource MinBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TKey> selector)
        {
            var comparer = Comparer<TKey>.Default;
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    throw new InvalidOperationException();
                }
                var lowest = iterator.Current;
                var lowestProjected = selector(lowest);
                while (iterator.MoveNext())
                {
                    var candidate = iterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, lowestProjected) >= 0) continue;
                    lowest = candidate;
                    lowestProjected = candidateProjected;
                }
                return lowest;
            }
        }

        public static TSource Second<TSource>([NotNull] this IEnumerable<TSource> source)
        {
            return source.ElementAt(1);
        }

        public static TSource LastButOne<TSource>([NotNull] this IEnumerable<TSource> source)
        {
            var list = source as IList<TSource>;
            if (list != null)
            {
                return list[list.Count - 2];
            }
            return source.Reverse().Second();
        }
    }
}
