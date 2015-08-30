using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Defs : ElementWithChildren
    {
        public Defs([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
