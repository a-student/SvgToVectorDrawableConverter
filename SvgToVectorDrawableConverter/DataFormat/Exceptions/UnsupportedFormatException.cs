using System;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Exceptions
{
    class UnsupportedFormatException : Exception
    {
        public UnsupportedFormatException()
        { }

        public UnsupportedFormatException([CanBeNull] string message)
            : base(message)
        { }

        public UnsupportedFormatException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : base(message, innerException)
        { }
    }
}
