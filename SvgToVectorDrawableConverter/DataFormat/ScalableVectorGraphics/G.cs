using System.Collections.Specialized;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class G : ElementWithChildren
    {
        public G(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style
        {
            get { return Parser.ParseStyle(GetAttribute<string>()); }
        }

        public Transform Transform
        {
            get { return Parser.ParseTransform(GetAttribute<string>()); }
        }
    }
}
