using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Title : Element
    {
        public Title([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
