using System;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    class Group : ElementWithChildren
    {
        public Group(XmlElement wrappedElement)
        : base(wrappedElement)
        { }

        public double Rotation
        {
            get { return GetAttribute<double>(); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
            }
        }

        public double PivotX
        {
            get { return GetAttribute<double>(); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
            }
        }

        public double PivotY
        {
            get { return GetAttribute<double>(); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
            }
        }

        public double ScaleX
        {
            get { return GetAttribute(1d); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android", 1);
                SetAttribute(Convert.ToSingle(value), "auto", 1);
            }
        }

        public double ScaleY
        {
            get { return GetAttribute(1d); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android", 1);
                SetAttribute(Convert.ToSingle(value), "auto", 1);
            }
        }

        public double TranslateX
        {
            get { return GetAttribute<double>(); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
            }
        }

        public double TranslateY
        {
            get { return GetAttribute<double>(); }
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
            }
        }
    }
}
