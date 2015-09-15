using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Style : Element
    {
        public Style([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
