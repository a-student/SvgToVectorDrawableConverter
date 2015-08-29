using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;
// ReSharper disable PossibleNullReferenceException

namespace SvgToVectorDrawableConverter.DataFormat.Utils
{
    static class SvgPreprocessor
    {
        public static void Preprocess([NotNull] string filename)
        {
            var xmlDocument = new XmlDocument { XmlResolver = null };
            xmlDocument.Load(filename);

            WrapSvgContentInG(xmlDocument);

            var map = new Dictionary<string, XmlNode>();
            FillMap(map, xmlDocument.DocumentElement);
            InlineUses(map, xmlDocument.DocumentElement);

            xmlDocument.Save(filename);
        }

        private static void WrapSvgContentInG(XmlDocument xmlDocument)
        {
            var svg = xmlDocument.DocumentElement;
            var g = xmlDocument.CreateElement("g", svg.NamespaceURI);
            while (svg.HasChildNodes)
            {
                g.AppendChild(svg.FirstChild);
            }
            svg.AppendChild(g);
        }

        private static void FillMap(IDictionary<string, XmlNode> map, XmlNode xmlNode)
        {
            var id = xmlNode.Attributes?["id"];
            if (id != null)
            {
                var clone = xmlNode.CloneNode(true);
                clone.Attributes.RemoveNamedItem("id");
                map[id.Value] = clone;
            }
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                FillMap(map, childNode);
            }
        }

        private static void InlineUses(IReadOnlyDictionary<string, XmlNode> map, XmlNode xmlNode)
        {
            var childNodes = xmlNode.ChildNodes;
            for (var i = 0; i < childNodes.Count; i++)
            {
                var childNode = childNodes[i];
                if (childNode.Name == "use")
                {
                    var href = childNode.Attributes.GetNamedItem("href", "http://www.w3.org/1999/xlink");
                    var hrefValue = href.Value;
                    if (!hrefValue.StartsWith("#"))
                    {
                        throw new UnsupportedFormatException("Only references within an SVG document are allowed.");
                    }
                    childNode.Attributes.Remove((XmlAttribute)href);
                    hrefValue = hrefValue.Substring(1);
                    childNode = CreateInline(childNode, map[hrefValue].CloneNode(true));
                    xmlNode.ReplaceChild(childNode, childNodes[i]);
                }
                InlineUses(map, childNode);
            }
        }

        private static readonly HashSet<string> UseAttributes = new HashSet<string> { "x", "y", "width", "height" };

        private static XmlElement CreateInline(XmlNode useNode, XmlNode refNode)
        {
            if (refNode.Name == "symbol" || refNode.Name == "svg")
            {
                throw new UnsupportedFormatException($"References to {refNode.Name} are not supported.");
            }

            var inline = useNode.OwnerDocument.CreateElement("g", useNode.NamespaceURI);

            foreach (XmlAttribute attribute in useNode.Attributes)
            {
                if (!UseAttributes.Contains(attribute.Name))
                {
                    inline.SetAttributeNode((XmlAttribute)attribute.Clone());
                }
            }

            var x = useNode.Attributes["x"]?.Value ?? "0";
            var y = useNode.Attributes["y"]?.Value ?? "0";
            var transform = inline.GetAttribute("transform").Trim();
            if (transform.Length > 0)
            {
                transform += ' ';
            }
            transform += $"translate({x}, {y})";
            inline.SetAttribute("transform", transform);

            foreach (XmlNode child in useNode.ChildNodes)
            {
                inline.AppendChild(child);
            }
            inline.AppendChild(refNode);

            return inline;
        }
    }
}
