using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DelftTools.Utils.Xml.Serialization
{
 
    /// <summary>
    /// serialize anything that derives from IDictionary such as Hashtable, SortedList, ListDictionary, or HybridDictionary
    /// http://msdn.microsoft.com/msdnmag/issues/03/06/XMLFiles/
    /// converts both key and value to string on serialization, does not
    /// store the originating type.
    /// </summary>
    public class DictionarySerializer : IXmlSerializable
    {
        const string NS = "http://www.develop.com/xml/serialization";

        public IDictionary dictionary;

        public DictionarySerializer() 
        {
            dictionary = new Hashtable();
        }
        public DictionarySerializer(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public void WriteXml(XmlWriter w)
        {
            w.WriteStartElement("dictionary", NS);
            foreach (object key in dictionary.Keys)
            {
                object value = dictionary[key];
               
                w.WriteStartElement("item", NS);
                w.WriteElementString("key", NS, key.ToString());
                w.WriteElementString("value", NS, value.ToString());
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }

        public void ReadXml(XmlReader r)
        {
            r.Read(); // move past container
            r.ReadStartElement("dictionary");
            while (r.NodeType != XmlNodeType.EndElement)
            {			
                r.ReadStartElement("item", NS);
                string key = r.ReadElementString("key", NS);
                string value = r.ReadElementString("value", NS);
                r.ReadEndElement();
                r.MoveToContent();
                dictionary.Add(key, value);
            }
        }

        public XmlSchema GetSchema()
        {
            return XmlSchema.Read(new StringReader(
                                      "<xs:schema id='DictionarySchema' targetNamespace='http://www.develop.com/xml/serialization' elementFormDefault='qualified' xmlns='http://www.develop.com/xml/serialization' xmlns:mstns='http://www.develop.com/xml/serialization' xmlns:xs='http://www.w3.org/2001/XMLSchema'><xs:complexType name='DictionaryType'><xs:sequence><xs:element name='item' type='ItemType' maxOccurs='unbounded' /></xs:sequence></xs:complexType><xs:complexType name='ItemType'><xs:sequence><xs:element name='key' type='xs:string' /><xs:element name='value' type='xs:string' /></xs:sequence></xs:complexType><xs:element name='dictionary' type='mstns:DictionaryType'></xs:element></xs:schema>"), null);

        }

    }
}