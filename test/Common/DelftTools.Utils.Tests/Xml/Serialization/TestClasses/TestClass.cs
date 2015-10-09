using System;
using System.Xml.Serialization;
using DelftTools.Utils.Xml.Serialization;

namespace DelftTools.Utils.Tests.Xml.Serialization.TestClasses
{
    [Serializable]
    //class is public to enable serialization
    public class TestClass
    {
        private BaseClass someclass;

        [XmlElement(Type = typeof(CustomXmlSerializer<BaseClass>))]
        public BaseClass SomeClass
        {
            get
            {
                return someclass;
            }
            set
            {
                someclass = value;
            }
        }
    }
}