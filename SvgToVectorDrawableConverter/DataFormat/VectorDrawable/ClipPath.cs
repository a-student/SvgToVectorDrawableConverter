using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    [XmlNames("clip-path")]
    class ClipPath : PathBase
    {
        public ClipPath([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
