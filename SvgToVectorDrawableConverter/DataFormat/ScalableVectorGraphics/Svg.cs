using System;
using System.Collections.Specialized;
using System.Windows;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

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

        public Rect ViewBox
        {
            get
            {
                var value = GetAttribute<string>();
                if (!string.IsNullOrEmpty(value))
                {
                    return Parser.ParseViewBox(value);
                }
                try
                {
                    return Parser.ParseViewBox($"0 0 {Width} {Height}");
                }
                catch (Exception e)
                {
                    throw new UnsupportedFormatException("The viewBox attribute must be set.", e);
                }
            }
        }
    }
}
