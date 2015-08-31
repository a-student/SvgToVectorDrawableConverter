using System;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    abstract class PathBase : Element
    {
        protected PathBase([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public string PathData
        {
            get { return GetAttribute<string>(); }
            set
            {
                SetAttribute(value, "android");
                SetAttribute(value, "auto");
                SetAttribute(value, "better");
            }
        }

        public FillType FillType
        {
            get { return (FillType)Enum.Parse(typeof(FillType), GetAttribute(default(FillType).ToString())); }
            set { SetAttribute(value, "better"); }
        }
    }
}
