using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Defs : Element
    {
        public Defs([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
