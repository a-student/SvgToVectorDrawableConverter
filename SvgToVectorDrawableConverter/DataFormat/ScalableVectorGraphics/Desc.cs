using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    [XmlNames("desc", "description")]
    class Desc : Element
    {
        public Desc([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
