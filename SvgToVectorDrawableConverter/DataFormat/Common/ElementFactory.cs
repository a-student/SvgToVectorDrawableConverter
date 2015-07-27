using System;
using System.Linq;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    static class ElementFactory
    {
        private static string GetXmlName(Type elementType)
        {
            return elementType.Name.ToLower();
        }

        public static T Create<T>(XmlDocument xmlDocument, out XmlElement xmlElement)
            where T : Element
        {
            xmlElement = xmlDocument.CreateElement(GetXmlName(typeof(T)));
            return (T)Wrap(xmlElement);
        }

        private static readonly Type[] ElementTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(Element).IsAssignableFrom(x))
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetConstructor(new[] { typeof(XmlElement) }) != null)
            .ToArray();

        public static Element Wrap(XmlElement xmlElement)
        {
            try
            {
                var rootType = ElementTypes
                    .Single(x => GetXmlName(x) == xmlElement.OwnerDocument.DocumentElement.Name);
                var type = ElementTypes
                    .Where(x => x.Namespace == rootType.Namespace)
                    .Single(x => GetXmlName(x) == xmlElement.Name);
                return (Element)Activator.CreateInstance(type, xmlElement);
            }
            catch (InvalidOperationException e)
            {
                throw new UnsupportedFormatException(string.Format("Element '{0}' is not supported.", xmlElement.Name), e);
            }
        }
    }
}
