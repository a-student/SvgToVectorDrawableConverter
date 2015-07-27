using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Metadata : Element
    {
        public Metadata(XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
