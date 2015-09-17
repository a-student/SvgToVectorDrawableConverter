using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PathFillTypeConverter.Extensions
{
    static class EnumerableExtension
    {
        [NotNull]
        public static IReadOnlyList<T> ToReadOnlyList<T>([NotNull] this IEnumerable<T> self) => self.ToList().AsReadOnly();
    }
}
