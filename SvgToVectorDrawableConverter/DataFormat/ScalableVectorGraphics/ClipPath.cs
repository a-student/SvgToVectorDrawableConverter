using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class ClipPath : ElementWithChildren
    {
        public ClipPath([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
