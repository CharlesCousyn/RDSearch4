using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{

    public class OrphaData
    {
        public JDBOR[] JDBOR { get; set; }
    }

    public class JDBOR
    {
        public string date { get; set; }
        public string version { get; set; }
        public string copyright { get; set; }
        public Disorderlist[] DisorderList { get; set; }
    }

    public class Disorderlist
    {
        public string count { get; set; }
        public Disorder[] Disorder { get; set; }
    }

    public class Disorder
    {
        public string id { get; set; }
        public string OrphaNumber { get; set; }
        public Expertlink[] ExpertLink { get; set; }
        public Name[] Name { get; set; }
        public Disorderflaglist[] DisorderFlagList { get; set; }
        public Synonymlist[] SynonymList { get; set; }
        public Disordertype[] DisorderType { get; set; }
        public Externalreferencelist[] ExternalReferenceList { get; set; }
        public Disorderdisorderassociationlist[] DisorderDisorderAssociationList { get; set; }
        public Textualinformationlist[] TextualInformationList { get; set; }
    }

    public class Expertlink
    {
        public string lang { get; set; }
        public string link { get; set; }
    }

    public class Name
    {
        public string lang { get; set; }
        public string label { get; set; }
    }

    public class Disorderflaglist
    {
        public string count { get; set; }
        public Disorderflag[] DisorderFlag { get; set; }
    }

    public class Disorderflag
    {
        public string id { get; set; }
        public string Label { get; set; }
    }

    public class Synonymlist
    {
        public string count { get; set; }
        public Synonym[] Synonym { get; set; }
    }

    public class Synonym
    {
        public string lang { get; set; }
        public string label { get; set; }
    }

    public class Disordertype
    {
        public string id { get; set; }
        public string OrphaNumber { get; set; }
        public Name[] Name { get; set; }
    }

    public class Externalreferencelist
    {
        public string count { get; set; }
        public Externalreference[] ExternalReference { get; set; }
    }

    public class Externalreference
    {
        public string id { get; set; }
        public string Source { get; set; }
        public string Reference { get; set; }
        public Disordermappingrelation[] DisorderMappingRelation { get; set; }
        public object DisorderMappingICDRelation { get; set; }
        public Disordermappingvalidationstatu[] DisorderMappingValidationStatus { get; set; }
    }

    public class Disordermappingrelation
    {
        public string id { get; set; }
        public string OrphaNumber { get; set; }
        public Name[] Name { get; set; }
    }

    public class Disordermappingvalidationstatu
    {
        public string id { get; set; }
        public string OrphaNumber { get; set; }
        public Name[] Name { get; set; }
    }
    
    public class Disorderdisorderassociationlist
    {
        public string count { get; set; }
    }

    public class Textualinformationlist
    {
        public string count { get; set; }
        public Textualinformation[] TextualInformation { get; set; }
    }

    public class Textualinformation
    {
        public string id { get; set; }
        public string lang { get; set; }
        public Textsectionlist[] TextSectionList { get; set; }
    }

    public class Textsectionlist
    {
        public string count { get; set; }
        public Textsection[] TextSection { get; set; }
    }

    public class Textsection
    {
        public string id { get; set; }
        public string lang { get; set; }
        public Textsectiontype[] TextSectionType { get; set; }
        public string Contents { get; set; }
    }

    public class Textsectiontype
    {
        public string id { get; set; }
        public Name[] Name { get; set; }
    }
}
