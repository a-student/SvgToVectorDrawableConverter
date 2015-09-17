using System;
using JetBrains.Annotations;

namespace PathFillTypeConverter.Exceptions
{
    class PathDataConverterException : Exception
    {
        public PathDataConverterException()
        { }

        public PathDataConverterException([CanBeNull] string message)
            : base(message)
        { }

        public PathDataConverterException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : base(message, innerException)
        { }
    }
}
