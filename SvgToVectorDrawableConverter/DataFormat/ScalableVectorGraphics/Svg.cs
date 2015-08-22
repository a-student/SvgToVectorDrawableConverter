using System.Collections.Specialized;
using System.Windows;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Svg : ElementWithChildren, IStyleableElement
    {
        public Svg([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public string Width => GetAttribute<string>();

        public string Height => GetAttribute<string>();

        public Rect ViewBox => Parser.ParseViewBox(GetAttribute<string>());
    }
}
