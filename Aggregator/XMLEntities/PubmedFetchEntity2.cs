/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace WebCrawler2
{
    [XmlRoot(ElementName = "journal-id")]
    public class Journalid
    {
        [XmlAttribute(AttributeName = "journal-id-type")]
        public string Journalidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "journal-title-group")]
    public class Journaltitlegroup
    {
        [XmlElement(ElementName = "journal-title")]
        public string Journaltitle { get; set; }
    }

    [XmlRoot(ElementName = "issn")]
    public class Issn
    {
        [XmlAttribute(AttributeName = "pub-type")]
        public string Pubtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "journal-meta")]
    public class Journalmeta
    {
        [XmlElement(ElementName = "journal-id")]
        public List<Journalid> Journalid { get; set; }
        [XmlElement(ElementName = "journal-title-group")]
        public Journaltitlegroup Journaltitlegroup { get; set; }
        [XmlElement(ElementName = "issn")]
        public List<Issn> Issn { get; set; }
        [XmlElement(ElementName = "publisher")]
        public Publisher Publisher { get; set; }
    }

    [XmlRoot(ElementName = "article-id")]
    public class Articleid
    {
        [XmlAttribute(AttributeName = "pub-id-type")]
        public string Pubidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "subj-group")]
    public class Subjgroup
    {
        [XmlElement(ElementName = "subject")]
        public string Subject { get; set; }
        [XmlAttribute(AttributeName = "subj-group-type")]
        public string Subjgrouptype { get; set; }
        [XmlElement(ElementName = "subj-group")]
        public Subjgroup subjgroup { get; set; }
    }

    [XmlRoot(ElementName = "article-categories")]
    public class Articlecategories
    {
        [XmlElement(ElementName = "subj-group")]
        public Subjgroup Subjgroup { get; set; }
    }

    [XmlRoot(ElementName = "title-group")]
    public class Titlegroup
    {
        [XmlElement(ElementName = "article-title")]
        public string Articletitle { get; set; }
    }

    [XmlRoot(ElementName = "name")]
    public class Name
    {
        [XmlElement(ElementName = "surname")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "given-names")]
        public string Givennames { get; set; }
        [XmlElement(ElementName = "suffix")]
        public string Suffix { get; set; }
    }

    [XmlRoot(ElementName = "xref")]
    public class Xref
    {
        [XmlAttribute(AttributeName = "ref-type")]
        public string Reftype { get; set; }
        [XmlAttribute(AttributeName = "rid")]
        public string Rid { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
    }

    [XmlRoot(ElementName = "contrib")]
    public class Contrib
    {
        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "xref")]
        public List<Xref> Xref { get; set; }
        [XmlAttribute(AttributeName = "contrib-type")]
        public string Contribtype { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "contrib-id")]
        public Contribid Contribid { get; set; }
        [XmlElement(ElementName = "address")]
        public Address Address { get; set; }
        [XmlAttribute(AttributeName = "corresp")]
        public string Corresp { get; set; }
        [XmlElement(ElementName = "degrees")]
        public string Degrees { get; set; }
        [XmlElement(ElementName = "uri")]
        public Uri Uri { get; set; }
        [XmlAttribute(AttributeName = "equal-contrib")]
        public string Equalcontrib { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "role")]
        public string Role { get; set; }
    }

    [XmlRoot(ElementName = "contrib-group")]
    public class Contribgroup
    {
        [XmlElement(ElementName = "contrib")]
        public List<Contrib> Contrib { get; set; }
        [XmlElement(ElementName = "aff")]
        public List<Aff> Aff { get; set; }
    }

    [XmlRoot(ElementName = "aff")]
    public class Aff
    {
        [XmlElement(ElementName = "label")]
        public List<string> Label { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "institution-wrap")]
        public Institutionwrap Institutionwrap { get; set; }
        [XmlElement(ElementName = "email")]
        public List<string> Email { get; set; }
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
        [XmlElement(ElementName = "institution")]
        public string Institution { get; set; }
        [XmlElement(ElementName = "addr-line")]
        public string Addrline { get; set; }
        [XmlElement(ElementName = "country")]
        public string Country { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "italic")]
        public string Italic { get; set; }
        [XmlAttribute(AttributeName = "rid")]
        public string Rid { get; set; }
    }

    [XmlRoot(ElementName = "corresp")]
    public class Corresp
    {
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "email")]
        public Email Email { get; set; }
        [XmlAttribute(AttributeName = "content-type")]
        public string Contenttype { get; set; }
        [XmlElement(ElementName = "bold")]
        public string Bold { get; set; }
        [XmlElement(ElementName = "institution")]
        public List<string> Institution { get; set; }
        [XmlElement(ElementName = "addr-line")]
        public string Addrline { get; set; }
        [XmlElement(ElementName = "country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
    }

    [XmlRoot(ElementName = "fn")]
    public class Fn
    {
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "fn-type")]
        public string Fntype { get; set; }
        [XmlElement(ElementName = "p")]
        public List<P> P { get; set; }
    }

    [XmlRoot(ElementName = "author-notes")]
    public class Authornotes
    {
        [XmlElement(ElementName = "corresp")]
        public Corresp Corresp { get; set; }
        [XmlElement(ElementName = "fn")]
        public List<Fn> Fn { get; set; }
    }

    [XmlRoot(ElementName = "pub-date")]
    public class Pubdate
    {
        [XmlElement(ElementName = "day")]
        public string Day { get; set; }
        [XmlElement(ElementName = "month")]
        public string Month { get; set; }
        [XmlElement(ElementName = "year")]
        public string Year { get; set; }
        [XmlAttribute(AttributeName = "pub-type")]
        public string Pubtype { get; set; }
        [XmlElement(ElementName = "season")]
        public string Season { get; set; }
    }

    [XmlRoot(ElementName = "ext-link")]
    public class Extlink
    {
        [XmlAttribute(AttributeName = "ext-link-type")]
        public string Extlinktype { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "license-p")]
    public class Licensep
    {
        [XmlElement(ElementName = "ext-link")]
        public List<Extlink> Extlink { get; set; }
        [XmlElement(ElementName = "bold")]
        public string Bold { get; set; }
        [XmlElement(ElementName = "uri")]
        public Uri Uri { get; set; }
    }

    [XmlRoot(ElementName = "license")]
    public class License
    {
        [XmlElement(ElementName = "license-p")]
        public Licensep Licensep { get; set; }
        [XmlAttribute(AttributeName = "license-type")]
        public string Licensetype { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "permissions")]
    public class Permissions
    {
        [XmlElement(ElementName = "license")]
        public License License { get; set; }
        [XmlElement(ElementName = "copyright-statement")]
        public string Copyrightstatement { get; set; }
        [XmlElement(ElementName = "copyright-year")]
        public string Copyrightyear { get; set; }
        [XmlElement(ElementName = "copyright-holder")]
        public string Copyrightholder { get; set; }
    }

    [XmlRoot(ElementName = "p")]
    public class P
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "graphic")]
        public Graphic Graphic { get; set; }
        [XmlElement(ElementName = "xref")]
        public List<Xref> Xref { get; set; }
        [XmlElement(ElementName = "italic")]
        public Italic Italic { get; set; }
        [XmlElement(ElementName = "ext-link")]
        public Extlink Extlink { get; set; }
        [XmlElement(ElementName = "list")]
        public List List { get; set; }
        [XmlElement(ElementName = "sub")]
        public string Sub { get; set; }
        [XmlElement(ElementName = "sc")]
        public string Sc { get; set; }
        [XmlElement(ElementName = "table-wrap")]
        public List<Tablewrap> Tablewrap { get; set; }
        [XmlElement(ElementName = "funding-source")]
        public List<Fundingsource> Fundingsource { get; set; }
        [XmlElement(ElementName = "fig")]
        public List<Fig> Fig { get; set; }
        [XmlElement(ElementName = "sup")]
        public List<Sup> Sup { get; set; }
        [XmlElement(ElementName = "supplementary-material")]
        public Supplementarymaterial Supplementarymaterial { get; set; }
        [XmlElement(ElementName = "bold")]
        public List<Bold> Bold { get; set; }
    }

    [XmlRoot(ElementName = "abstract")]
    public class Abstract
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "p")]
        public List<P> P { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "abstract-type")]
        public string Abstracttype { get; set; }
        [XmlElement(ElementName = "sec")]
        public List<Sec> Sec { get; set; }
    }

    [XmlRoot(ElementName = "graphic")]
    public class Graphic
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "article-meta")]
    public class Articlemeta
    {
        [XmlElement(ElementName = "article-id")]
        public List<Articleid> Articleid { get; set; }
        [XmlElement(ElementName = "article-categories")]
        public Articlecategories Articlecategories { get; set; }
        [XmlElement(ElementName = "title-group")]
        public Titlegroup Titlegroup { get; set; }
        [XmlElement(ElementName = "contrib-group")]
        public Contribgroup Contribgroup { get; set; }
        [XmlElement(ElementName = "aff")]
        public List<Aff> Aff { get; set; }
        [XmlElement(ElementName = "author-notes")]
        public Authornotes Authornotes { get; set; }
        [XmlElement(ElementName = "pub-date")]
        public List<Pubdate> Pubdate { get; set; }
        [XmlElement(ElementName = "volume")]
        public string Volume { get; set; }
        [XmlElement(ElementName = "issue")]
        public string Issue { get; set; }
        [XmlElement(ElementName = "fpage")]
        public string Fpage { get; set; }
        [XmlElement(ElementName = "lpage")]
        public string Lpage { get; set; }
        [XmlElement(ElementName = "permissions")]
        public Permissions Permissions { get; set; }
        [XmlElement(ElementName = "abstract")]
        public List<Abstract> Abstract { get; set; }
        [XmlElement(ElementName = "history")]
        public History History { get; set; }
        [XmlElement(ElementName = "kwd-group")]
        public List<Kwdgroup> Kwdgroup { get; set; }
        [XmlElement(ElementName = "elocation-id")]
        public string Elocationid { get; set; }
        [XmlElement(ElementName = "custom-meta-group")]
        public Custommetagroup Custommetagroup { get; set; }
        [XmlElement(ElementName = "self-uri")]
        public Selfuri Selfuri { get; set; }
        [XmlElement(ElementName = "counts")]
        public Counts Counts { get; set; }
        [XmlElement(ElementName = "funding-group")]
        public Fundinggroup Fundinggroup { get; set; }
    }

    [XmlRoot(ElementName = "front")]
    public class Front
    {
        [XmlElement(ElementName = "journal-meta")]
        public Journalmeta Journalmeta { get; set; }
        [XmlElement(ElementName = "article-meta")]
        public Articlemeta Articlemeta { get; set; }
    }

    [XmlRoot(ElementName = "sec")]
    public class Sec
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "p")]
        public List<P> P { get; set; }
        [XmlAttribute(AttributeName = "sec-type")]
        public string Sectype { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "sec")]
        public List<Sec> sec { get; set; }
        [XmlElement(ElementName = "supplementary-material")]
        public List<Supplementarymaterial> Supplementarymaterial { get; set; }
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlElement(ElementName = "fig")]
        public List<Fig> Fig { get; set; }
    }

    [XmlRoot(ElementName = "italic")]
    public class Italic
    {
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
    }

    [XmlRoot(ElementName = "media")]
    public class Media
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlElement(ElementName = "caption")]
        public Caption Caption { get; set; }
    }

    [XmlRoot(ElementName = "supplementary-material")]
    public class Supplementarymaterial
    {
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlElement(ElementName = "media")]
        public Media Media { get; set; }
        [XmlAttribute(AttributeName = "content-type")]
        public string Contenttype { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "body")]
    public class Body
    {
        [XmlElement(ElementName = "sec")]
        public List<Sec> Sec { get; set; }
        [XmlElement(ElementName = "p")]
        public List<P> P { get; set; }
    }

    [XmlRoot(ElementName = "ack")]
    public class Ack
    {
        [XmlElement(ElementName = "p")]
        public string P { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "sec")]
        public List<Sec> Sec { get; set; }
    }

    [XmlRoot(ElementName = "fn-group")]
    public class Fngroup
    {
        [XmlElement(ElementName = "fn")]
        public List<Fn> Fn { get; set; }
    }

    [XmlRoot(ElementName = "person-group")]
    public class Persongroup
    {
        [XmlElement(ElementName = "name")]
        public List<Name> Name { get; set; }
        [XmlAttribute(AttributeName = "person-group-type")]
        public string Persongrouptype { get; set; }
        [XmlElement(ElementName = "etal")]
        public string Etal { get; set; }
        [XmlElement(ElementName = "collab")]
        public string Collab { get; set; }
    }

    [XmlRoot(ElementName = "pub-id")]
    public class Pubid
    {
        [XmlAttribute(AttributeName = "pub-id-type")]
        public string Pubidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "element-citation")]
    public class Elementcitation
    {
        [XmlElement(ElementName = "person-group")]
        public List<Persongroup> Persongroup { get; set; }
        [XmlElement(ElementName = "year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "volume")]
        public string Volume { get; set; }
        [XmlElement(ElementName = "fpage")]
        public string Fpage { get; set; }
        [XmlElement(ElementName = "pub-id")]
        public List<Pubid> Pubid { get; set; }
        [XmlAttribute(AttributeName = "publication-type")]
        public string Publicationtype { get; set; }
        [XmlElement(ElementName = "lpage")]
        public string Lpage { get; set; }
        [XmlElement(ElementName = "comment")]
        public Comment Comment { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "article-title")]
        public Articletitle Articletitle { get; set; }
        [XmlElement(ElementName = "issue")]
        public string Issue { get; set; }
        [XmlElement(ElementName = "publisher-loc")]
        public string Publisherloc { get; set; }
        [XmlElement(ElementName = "publisher-name")]
        public string Publishername { get; set; }
        [XmlElement(ElementName = "edition")]
        public string Edition { get; set; }
        [XmlElement(ElementName = "elocation-id")]
        public string Elocationid { get; set; }
        [XmlElement(ElementName = "conf-loc")]
        public string Confloc { get; set; }
        [XmlElement(ElementName = "conf-date")]
        public string Confdate { get; set; }
        [XmlElement(ElementName = "month")]
        public string Month { get; set; }
        [XmlElement(ElementName = "season")]
        public string Season { get; set; }
        [XmlElement(ElementName = "day")]
        public string Day { get; set; }
        [XmlElement(ElementName = "collab")]
        public string Collab { get; set; }
        [XmlElement(ElementName = "chapter-title")]
        public string Chaptertitle { get; set; }
        [XmlElement(ElementName = "supplement")]
        public string Supplement { get; set; }
        [XmlElement(ElementName = "source")]
        public Source Source { get; set; }
        [XmlElement(ElementName = "ext-link")]
        public Extlink Extlink { get; set; }
        [XmlElement(ElementName = "object-id")]
        public Objectid Objectid { get; set; }
    }

    [XmlRoot(ElementName = "ref")]
    public class Ref
    {
        [XmlElement(ElementName = "element-citation")]
        public Elementcitation Elementcitation { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlElement(ElementName = "mixed-citation")]
        public Mixedcitation Mixedcitation { get; set; }
    }

    [XmlRoot(ElementName = "comment")]
    public class Comment
    {
        [XmlElement(ElementName = "ext-link")]
        public Extlink Extlink { get; set; }
    }

    [XmlRoot(ElementName = "ref-list")]
    public class Reflist
    {
        [XmlElement(ElementName = "ref")]
        public List<Ref> Ref { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "back")]
    public class Back
    {
        [XmlElement(ElementName = "ack")]
        public Ack Ack { get; set; }
        [XmlElement(ElementName = "fn-group")]
        public Fngroup Fngroup { get; set; }
        [XmlElement(ElementName = "ref-list")]
        public Reflist Reflist { get; set; }
        [XmlElement(ElementName = "notes")]
        public List<Notes> Notes { get; set; }
        [XmlElement(ElementName = "glossary")]
        public Glossary Glossary { get; set; }
        [XmlElement(ElementName = "sec")]
        public List<Sec> Sec { get; set; }
    }

    [XmlRoot(ElementName = "caption")]
    public class Caption
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "p")]
        public List<P> P { get; set; }
    }

    [XmlRoot(ElementName = "fig")]
    public class Fig
    {
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlElement(ElementName = "caption")]
        public Caption Caption { get; set; }
        [XmlElement(ElementName = "graphic")]
        public Graphic Graphic { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlAttribute(AttributeName = "fig-type")]
        public string Figtype { get; set; }
        [XmlElement(ElementName = "alt-text")]
        public Alttext Alttext { get; set; }
    }

    [XmlRoot(ElementName = "list-item")]
    public class Listitem
    {
        [XmlElement(ElementName = "p")]
        public P P { get; set; }
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "list")]
    public class List
    {
        [XmlElement(ElementName = "list-item")]
        public List<Listitem> Listitem { get; set; }
        [XmlAttribute(AttributeName = "list-type")]
        public string Listtype { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "boxed-text")]
    public class Boxedtext
    {
        [XmlElement(ElementName = "caption")]
        public Caption Caption { get; set; }
        [XmlElement(ElementName = "list")]
        public List List { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }
    }

    [XmlRoot(ElementName = "floats-group")]
    public class Floatsgroup
    {
        [XmlElement(ElementName = "fig")]
        public List<Fig> Fig { get; set; }
        [XmlElement(ElementName = "boxed-text")]
        public Boxedtext Boxedtext { get; set; }
        [XmlElement(ElementName = "table-wrap")]
        public List<Tablewrap> Tablewrap { get; set; }
    }

    [XmlRoot(ElementName = "article")]
    public class Article
    {
        [XmlElement(ElementName = "front")]
        public Front Front { get; set; }
        [XmlElement(ElementName = "body")]
        public Body Body { get; set; }
        [XmlElement(ElementName = "back")]
        public Back Back { get; set; }
        [XmlElement(ElementName = "floats-group")]
        public Floatsgroup Floatsgroup { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
        [XmlAttribute(AttributeName = "mml", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mml { get; set; }
        [XmlAttribute(AttributeName = "article-type")]
        public string Articletype { get; set; }
    }

    [XmlRoot(ElementName = "publisher")]
    public class Publisher
    {
        [XmlElement(ElementName = "publisher-name")]
        public string Publishername { get; set; }
        [XmlElement(ElementName = "publisher-loc")]
        public string Publisherloc { get; set; }
    }

    [XmlRoot(ElementName = "date")]
    public class Date
    {
        [XmlElement(ElementName = "day")]
        public string Day { get; set; }
        [XmlElement(ElementName = "month")]
        public string Month { get; set; }
        [XmlElement(ElementName = "year")]
        public string Year { get; set; }
        [XmlAttribute(AttributeName = "date-type")]
        public string Datetype { get; set; }
    }

    [XmlRoot(ElementName = "history")]
    public class History
    {
        [XmlElement(ElementName = "date")]
        public List<Date> Date { get; set; }
    }

    [XmlRoot(ElementName = "kwd-group")]
    public class Kwdgroup
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "kwd")]
        public List<string> Kwd { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang { get; set; }
        [XmlAttribute(AttributeName = "kwd-group-type")]
        public string Kwdgrouptype { get; set; }
    }

    [XmlRoot(ElementName = "alt-text")]
    public class Alttext
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "th")]
    public class Th
    {
        [XmlElement(ElementName = "hr")]
        public string Hr { get; set; }
        [XmlAttribute(AttributeName = "colspan")]
        public string Colspan { get; set; }
        [XmlAttribute(AttributeName = "align")]
        public string Align { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "valign")]
        public string Valign { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlAttribute(AttributeName = "rowspan")]
        public string Rowspan { get; set; }
        [XmlElement(ElementName = "italic")]
        public string Italic { get; set; }
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
    }

    [XmlRoot(ElementName = "tr")]
    public class Tr
    {
        [XmlElement(ElementName = "th")]
        public List<Th> Th { get; set; }
        [XmlElement(ElementName = "td")]
        public List<Td> Td { get; set; }
    }

    [XmlRoot(ElementName = "thead")]
    public class Thead
    {
        [XmlElement(ElementName = "tr")]
        public List<Tr> Tr { get; set; }
    }

    [XmlRoot(ElementName = "td")]
    public class Td
    {
        [XmlElement(ElementName = "italic")]
        public List<string> Italic { get; set; }
        [XmlAttribute(AttributeName = "align")]
        public string Align { get; set; }
        [XmlElement(ElementName = "xref")]
        public List<Xref> Xref { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "sub")]
        public string Sub { get; set; }
        [XmlElement(ElementName = "sc")]
        public string Sc { get; set; }
        [XmlAttribute(AttributeName = "colspan")]
        public string Colspan { get; set; }
        [XmlAttribute(AttributeName = "valign")]
        public string Valign { get; set; }
        [XmlAttribute(AttributeName = "rowspan")]
        public string Rowspan { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlElement(ElementName = "hr")]
        public string Hr { get; set; }
        [XmlElement(ElementName = "break")]
        public List<string> Break { get; set; }
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
        [XmlAttribute(AttributeName = "char")]
        public string Char { get; set; }
    }

    [XmlRoot(ElementName = "tbody")]
    public class Tbody
    {
        [XmlElement(ElementName = "tr")]
        public List<Tr> Tr { get; set; }
    }

    [XmlRoot(ElementName = "table")]
    public class Table
    {
        [XmlElement(ElementName = "thead")]
        public Thead Thead { get; set; }
        [XmlElement(ElementName = "tbody")]
        public Tbody Tbody { get; set; }
        [XmlAttribute(AttributeName = "frame")]
        public string Frame { get; set; }
        [XmlAttribute(AttributeName = "rules")]
        public string Rules { get; set; }
        [XmlElement(ElementName = "col")]
        public List<Col> Col { get; set; }
    }

    [XmlRoot(ElementName = "table-wrap")]
    public class Tablewrap
    {
        [XmlElement(ElementName = "label")]
        public string Label { get; set; }
        [XmlElement(ElementName = "caption")]
        public Caption Caption { get; set; }
        [XmlElement(ElementName = "alt-text")]
        public Alttext Alttext { get; set; }
        [XmlElement(ElementName = "table")]
        public Table Table { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlElement(ElementName = "object-id")]
        public Objectid Objectid { get; set; }
        [XmlAttribute(AttributeName = "orientation")]
        public string Orientation { get; set; }
        [XmlElement(ElementName = "table-wrap-foot")]
        public Tablewrapfoot Tablewrapfoot { get; set; }
        [XmlElement(ElementName = "alternatives")]
        public Alternatives Alternatives { get; set; }
    }

    [XmlRoot(ElementName = "funding-source")]
    public class Fundingsource
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "institution-wrap")]
        public Institutionwrap Institutionwrap { get; set; }
    }

    [XmlRoot(ElementName = "article-title")]
    public class Articletitle
    {
        [XmlElement(ElementName = "italic")]
        public List<string> Italic { get; set; }
        [XmlElement(ElementName = "sc")]
        public string Sc { get; set; }
        [XmlElement(ElementName = "sup")]
        public string Sup { get; set; }
    }

    [XmlRoot(ElementName = "mixed-citation")]
    public class Mixedcitation
    {
        [XmlAttribute(AttributeName = "publication-type")]
        public string Publicationtype { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "person-group")]
        public Persongroup Persongroup { get; set; }
        [XmlElement(ElementName = "article-title")]
        public string Articletitle { get; set; }
        [XmlElement(ElementName = "source")]
        public string Source { get; set; }
        [XmlElement(ElementName = "year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "volume")]
        public string Volume { get; set; }
        [XmlElement(ElementName = "fpage")]
        public string Fpage { get; set; }
        [XmlElement(ElementName = "lpage")]
        public string Lpage { get; set; }
        [XmlElement(ElementName = "pub-id")]
        public List<Pubid> Pubid { get; set; }
        [XmlElement(ElementName = "collab")]
        public string Collab { get; set; }
        [XmlElement(ElementName = "issue")]
        public string Issue { get; set; }
        [XmlElement(ElementName = "publisher-loc")]
        public string Publisherloc { get; set; }
        [XmlElement(ElementName = "publisher-name")]
        public string Publishername { get; set; }
        [XmlElement(ElementName = "ext-link")]
        public Extlink Extlink { get; set; }
    }

    [XmlRoot(ElementName = "contrib-id")]
    public class Contribid
    {
        [XmlAttribute(AttributeName = "contrib-id-type")]
        public string Contribidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "authenticated")]
        public string Authenticated { get; set; }
    }

    [XmlRoot(ElementName = "address")]
    public class Address
    {
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
    }

    [XmlRoot(ElementName = "institution-id")]
    public class Institutionid
    {
        [XmlAttribute(AttributeName = "institution-id-type")]
        public string Institutionidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "institution-wrap")]
    public class Institutionwrap
    {
        [XmlElement(ElementName = "institution-id")]
        public List<Institutionid> Institutionid { get; set; }
        [XmlElement(ElementName = "institution")]
        public List<string> Institution { get; set; }
    }

    [XmlRoot(ElementName = "custom-meta")]
    public class Custommeta
    {
        [XmlElement(ElementName = "meta-name")]
        public string Metaname { get; set; }
        [XmlElement(ElementName = "meta-value")]
        public string Metavalue { get; set; }
    }

    [XmlRoot(ElementName = "custom-meta-group")]
    public class Custommetagroup
    {
        [XmlElement(ElementName = "custom-meta")]
        public Custommeta Custommeta { get; set; }
    }

    [XmlRoot(ElementName = "notes")]
    public class Notes
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "p")]
        public string P { get; set; }
        [XmlAttribute(AttributeName = "notes-type")]
        public string Notestype { get; set; }
        [XmlElement(ElementName = "sec")]
        public List<Sec> Sec { get; set; }
    }

    [XmlRoot(ElementName = "email")]
    public class Email
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "self-uri")]
    public class Selfuri
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "sup")]
    public class Sup
    {
        [XmlElement(ElementName = "xref")]
        public List<Xref> Xref { get; set; }
    }

    [XmlRoot(ElementName = "object-id")]
    public class Objectid
    {
        [XmlAttribute(AttributeName = "pub-id-type")]
        public string Pubidtype { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "uri")]
    public class Uri
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "fig-count")]
    public class Figcount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "table-count")]
    public class Tablecount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "equation-count")]
    public class Equationcount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "ref-count")]
    public class Refcount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "page-count")]
    public class Pagecount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "word-count")]
    public class Wordcount
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "counts")]
    public class Counts
    {
        [XmlElement(ElementName = "fig-count")]
        public Figcount Figcount { get; set; }
        [XmlElement(ElementName = "table-count")]
        public Tablecount Tablecount { get; set; }
        [XmlElement(ElementName = "equation-count")]
        public Equationcount Equationcount { get; set; }
        [XmlElement(ElementName = "ref-count")]
        public Refcount Refcount { get; set; }
        [XmlElement(ElementName = "page-count")]
        public Pagecount Pagecount { get; set; }
        [XmlElement(ElementName = "word-count")]
        public Wordcount Wordcount { get; set; }
    }

    [XmlRoot(ElementName = "title")]
    public class Title
    {
        [XmlElement(ElementName = "italic")]
        public List<string> Italic { get; set; }
    }

    [XmlRoot(ElementName = "table-wrap-foot")]
    public class Tablewrapfoot
    {
        [XmlElement(ElementName = "p")]
        public P P { get; set; }
    }

    [XmlRoot(ElementName = "award-group")]
    public class Awardgroup
    {
        [XmlElement(ElementName = "funding-source")]
        public Fundingsource Fundingsource { get; set; }
        [XmlElement(ElementName = "award-id")]
        public string Awardid { get; set; }
    }

    [XmlRoot(ElementName = "funding-group")]
    public class Fundinggroup
    {
        [XmlElement(ElementName = "award-group")]
        public Awardgroup Awardgroup { get; set; }
    }

    [XmlRoot(ElementName = "col")]
    public class Col
    {
        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
        [XmlAttribute(AttributeName = "span")]
        public string Span { get; set; }
    }

    [XmlRoot(ElementName = "alternatives")]
    public class Alternatives
    {
        [XmlElement(ElementName = "graphic")]
        public Graphic Graphic { get; set; }
        [XmlElement(ElementName = "table")]
        public Table Table { get; set; }
    }

    [XmlRoot(ElementName = "def")]
    public class Def
    {
        [XmlElement(ElementName = "p")]
        public P P { get; set; }
    }

    [XmlRoot(ElementName = "def-item")]
    public class Defitem
    {
        [XmlElement(ElementName = "term")]
        public string Term { get; set; }
        [XmlElement(ElementName = "def")]
        public Def Def { get; set; }
    }

    [XmlRoot(ElementName = "def-list")]
    public class Deflist
    {
        [XmlElement(ElementName = "def-item")]
        public List<Defitem> Defitem { get; set; }
    }

    [XmlRoot(ElementName = "glossary")]
    public class Glossary
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "def-list")]
        public Deflist Deflist { get; set; }
    }

    [XmlRoot(ElementName = "bold")]
    public class Bold
    {
        [XmlElement(ElementName = "xref")]
        public Xref Xref { get; set; }
    }

    [XmlRoot(ElementName = "source")]
    public class Source
    {
        [XmlElement(ElementName = "italic")]
        public string Italic { get; set; }
    }

    [XmlRoot(ElementName = "pmc-articleset")]
    public class Pmcarticleset
    {
        [XmlElement(ElementName = "article")]
        public List<Article> Article { get; set; }
    }

}
