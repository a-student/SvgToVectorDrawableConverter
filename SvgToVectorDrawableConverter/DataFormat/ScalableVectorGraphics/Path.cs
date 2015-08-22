using System.Collections.Specialized;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Path : Element, IStyleableElement
    {
        public Path([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public Transform Transform => Parser.ParseTransform(GetAttribute<string>());

        public string D => GetAttribute<string>();
    }
}
