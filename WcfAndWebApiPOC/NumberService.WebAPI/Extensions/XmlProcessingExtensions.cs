namespace NumberService.WebAPI.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public static class XmlProcessingExtensions
    {
        //C.f.: https://stackoverflow.com/questions/30190246/remove-element-namespace-prefixes-from-xml-string-fragment
        public static void RemoveNamespaces(this XElement node)
        {
            node.Name = node.Name.LocalName;

            foreach (var child in node.Elements())
            {
                RemoveNamespaces(child);
            }

            foreach (var attr in node.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                {
                    attr.Remove();
                }
            }
        }

        public static int ReadSingleIntFrom(this Stream xmlStream, string xPath, IXmlNamespaceResolver resolver = null)
        {
            var document = XDocument.Load(xmlStream);

            var value = resolver == null
                ? document.XPathSelectElement(xPath).Value
                : document.XPathSelectElement(xPath, resolver).Value;


            return int.Parse(value);
        }

        public static int[] ReadIntArrayFrom(this Stream xmlStream, string xPath, IXmlNamespaceResolver resolver = null)
        {
            var document = XDocument.Load(xmlStream);

            var nodes = resolver == null 
                ? document.XPathSelectElements(xPath)
                : document.XPathSelectElements(xPath, resolver);

            return nodes.Select(node => int.Parse(node.Value)).ToArray();
        }
    }
}
