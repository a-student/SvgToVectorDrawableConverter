using System;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    class XmlNamesAttribute : Attribute
    {
        public string[] Names { get; }

        public XmlNamesAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
