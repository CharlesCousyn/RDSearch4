using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebCrawler
{
    
        [XmlRoot(ElementName = "IdList")]
        public class IdList
        {
            [XmlElement(ElementName = "Id")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "Link")]
        public class Link
        {
            [XmlElement(ElementName = "Id")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "LinkSetDb")]
        public class LinkSetDb
        {
            [XmlElement(ElementName = "DbTo")]
            public string DbTo { get; set; }
            [XmlElement(ElementName = "LinkName")]
            public string LinkName { get; set; }
            [XmlElement(ElementName = "Link")]
            public List<Link> Link { get; set; }
        }

        [XmlRoot(ElementName = "LinkSet")]
        public class LinkSet
        {
            [XmlElement(ElementName = "DbFrom")]
            public string DbFrom { get; set; }
            [XmlElement(ElementName = "IdList")]
            public IdList IdList { get; set; }
            [XmlElement(ElementName = "LinkSetDb")]
            public LinkSetDb LinkSetDb { get; set; }
        }

        [XmlRoot(ElementName = "eLinkResult")]
        public class ELinkResult
        {
            [XmlElement(ElementName = "LinkSet")]
            public List<LinkSet> LinkSet { get; set; }
        }
}
