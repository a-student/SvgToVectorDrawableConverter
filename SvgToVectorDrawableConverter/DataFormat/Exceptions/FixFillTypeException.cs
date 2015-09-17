using System;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Exceptions
{
    class FixFillTypeException : Exception
    {
        public FixFillTypeException([NotNull] Exception innerException)
            : base(null, innerException)
        { }
    }
}
