using System.Collections.Specialized;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Path : Element
    {
        public Path(XmlElement wrappedElement)
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

        public string D
        {
            get { return GetAttribute<string>(); }
        }
    }
}
