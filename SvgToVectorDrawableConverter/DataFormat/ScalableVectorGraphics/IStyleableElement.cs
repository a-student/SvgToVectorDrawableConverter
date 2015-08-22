using System.Collections.Specialized;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    interface IStyleableElement
    {
        [NotNull]
        StringDictionary Style { get; }
    }
}
