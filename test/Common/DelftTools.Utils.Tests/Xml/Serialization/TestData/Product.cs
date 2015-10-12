using System.Xml.Serialization;

namespace DelftTools.Utils.Tests.Xml.Serialization.TestData
{
    [XmlRoot("product", Namespace = "", IsNullable = false)]
    public class Product
    {
        [XmlElement("product_id")]
        public int ProductID;

        [XmlElement("name")]
        public string Name;

        [XmlElement("list_price", DataType = "decimal")]
        public decimal ListPrice;
    }
}