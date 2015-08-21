using System.Collections.Specialized;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    interface IStyleableElement
    {
        StringDictionary Style { get; }
    }
}
