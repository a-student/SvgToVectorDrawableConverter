using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Defs : Element
    {
        public Defs(XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
