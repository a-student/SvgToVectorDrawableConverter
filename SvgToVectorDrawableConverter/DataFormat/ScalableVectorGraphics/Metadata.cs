using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Metadata : Element
    {
        public Metadata([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
