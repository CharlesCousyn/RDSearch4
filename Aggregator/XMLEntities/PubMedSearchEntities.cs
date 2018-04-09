using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class eSearchResult
    {

        private long countField;

        private int retMaxField;

        private int retStartField;

        private int queryKeyField;

        private string webEnvField;

        private long[] idListField;

        private eSearchResultTranslationSet translationSetField;

        private eSearchResultTranslationStack translationStackField;

        private string queryTranslationField;

        /// <remarks/>
        public long Count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }

        /// <remarks/>
        public int RetMax
        {
            get
            {
                return this.retMaxField;
            }
            set
            {
                this.retMaxField = value;
            }
        }

        /// <remarks/>
        public int RetStart
        {
            get
            {
                return this.retStartField;
            }
            set
            {
                this.retStartField = value;
            }
        }

        /// <remarks/>
        public int QueryKey
        {
            get
            {
                return this.queryKeyField;
            }
            set
            {
                this.queryKeyField = value;
            }
        }

        /// <remarks/>
        public string WebEnv
        {
            get
            {
                return this.webEnvField;
            }
            set
            {
                this.webEnvField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable = false)]
        public long[] IdList
        {
            get
            {
                return this.idListField;
            }
            set
            {
                this.idListField = value;
            }
        }

        /// <remarks/>
        public eSearchResultTranslationSet TranslationSet
        {
            get
            {
                return this.translationSetField;
            }
            set
            {
                this.translationSetField = value;
            }
        }

        /// <remarks/>
        public eSearchResultTranslationStack TranslationStack
        {
            get
            {
                return this.translationStackField;
            }
            set
            {
                this.translationStackField = value;
            }
        }

        /// <remarks/>
        public string QueryTranslation
        {
            get
            {
                return this.queryTranslationField;
            }
            set
            {
                this.queryTranslationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class eSearchResultTranslationSet
    {

        private eSearchResultTranslationSetTranslation translationField;

        /// <remarks/>
        public eSearchResultTranslationSetTranslation Translation
        {
            get
            {
                return this.translationField;
            }
            set
            {
                this.translationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class eSearchResultTranslationSetTranslation
    {

        private string fromField;

        private string toField;

        /// <remarks/>
        public string From
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        public string To
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class eSearchResultTranslationStack
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OP", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("TermSet", typeof(eSearchResultTranslationStackTermSet))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class eSearchResultTranslationStackTermSet
    {

        private string termField;

        private string fieldField;

        private uint countField;

        private string explodeField;

        /// <remarks/>
        public string Term
        {
            get
            {
                return this.termField;
            }
            set
            {
                this.termField = value;
            }
        }

        /// <remarks/>
        public string Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }

        /// <remarks/>
        public uint Count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }

        /// <remarks/>
        public string Explode
        {
            get
            {
                return this.explodeField;
            }
            set
            {
                this.explodeField = value;
            }
        }
    }


}
