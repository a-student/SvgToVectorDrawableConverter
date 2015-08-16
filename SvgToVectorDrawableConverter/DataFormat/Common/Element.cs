using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    abstract class Element
    {
        internal XmlElement WrappedElement { get; }

        protected Element(XmlElement xmlElement)
        {
            WrappedElement = xmlElement;
        }

        protected T GetAttribute<T>(T defaultValue = default(T), [CallerMemberName] string name = null)
        {
            name = FirstCharToLower(name);
            var attribute = WrappedElement.Attributes[name];
            if (attribute == null)
            {
                return defaultValue;
            }
            return (T)Convert.ChangeType(attribute.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        protected void SetAttribute<T>(T value, string prefix, T defaultValue = default(T), [CallerMemberName] string name = null)
        {
            var namespaceUri = WrappedElement.GetNamespaceOfPrefix(prefix);
            if (string.IsNullOrEmpty(namespaceUri))
            {
                return;
            }

            name = FirstCharToLower(name);
            var attribute = WrappedElement.Attributes[name, namespaceUri];
            if (attribute == null)
            {
                attribute = WrappedElement.OwnerDocument.CreateAttribute(name, namespaceUri);
                WrappedElement.Attributes.Append(attribute);
            }
            if (!Equals(value, defaultValue))
            {
                attribute.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            else
            {
                WrappedElement.Attributes.Remove(attribute);
            }
        }

        private static string FirstCharToLower(string s)
        {
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public override string ToString()
        {
            return WrappedElement.Name;
        }
    }
}
