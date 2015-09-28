using DelftTools.TestUtils;
using DelftTools.Utils.Tests.Xml.Serialization.TestClasses;
using DelftTools.Utils.Xml.Serialization;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Xml.Serialization
{
    [TestFixture]
    public class CustomSerializerTest
    {
        private static DerivedClass derived;
        private static TestClass test;

        [SetUp]
        public void Initialize()
        {
            derived = new DerivedClass();
            test = new TestClass();
        }

        [TearDown]
        public void Terminate()
        {
            derived = null;
            test = null;
        }

        [Test]
        [Category(TestCategory.DataAccess)]
        public void Write()
        {
            //try to serialize a derived class while xml tags are only present in the baseclass
            string filePath1 = @"CustomSerializerTest.Write.xml";
                
            derived.Id = 121;
            derived.AnotherId = 1600;
            test.SomeClass = derived;
            ObjectXmlSerializer<TestClass>.Save(test, filePath1);

            //reload the class, compare the property values
            TestClass myClassFromFile = ObjectXmlSerializer<TestClass>.Load(filePath1);
            Assert.AreEqual(test.SomeClass.Id, myClassFromFile.SomeClass.Id);
        }

        [Test]
        [Category(TestCategory.DataAccess)]
        public void WriteAndReadParameter()
        {
/*
            writer.WriteStartElement("dataItem");
            writer.WriteAttributeString("type", o.GetType().ToString());
            XmlSerializer serializer = CustomXmlSerializer<IDataItem>.CreateXmlSerializer(o.GetType(), typeof(IDataItem));
            serializer.Serialize(writer, o as IDataItem, ns);
            writer.WriteEndElement();

            Folder folder = new Folder();
            XmlSerializer serializer = new XmlSerializer(typeof(Folder), "DelftTools.Shell.Core");
            Task s = (Task)serializer.Deserialize(reader);
            folder.Items.Add(s);
            
            string typeName = reader.GetAttribute("type");
            reader.Read();
            XmlSerializer serializer = CustomXmlSerializer<IDataItem>.CreateXmlSerializer(typeName);
            IDataItem dataItem = (IDataItem)serializer.Deserialize(reader);
            folder.Items.Add(dataItem);
            reader.Read();
 */           
        }
    }
}