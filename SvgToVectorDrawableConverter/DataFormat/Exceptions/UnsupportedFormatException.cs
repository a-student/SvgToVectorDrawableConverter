using System;

namespace SvgToVectorDrawableConverter.DataFormat.Exceptions
{
    class UnsupportedFormatException : Exception
    {
        public UnsupportedFormatException()
        { }

        public UnsupportedFormatException(String message)
            : base(message)
        { }

        public UnsupportedFormatException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
