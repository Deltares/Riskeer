using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;
using DelftTools.Utils.Xml.Serialization;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Xml.Serialization
{
    [TestFixture]
    public class DictionarySerializerTest
    {
        private readonly string className = "DictionarySerializerTest";

        [Test]
        public void HashtableTest()
        {
            string methodName = "HashtableTest";
            string filePath = className + "." + methodName + ".xml";
            Hashtable hashtable = new Hashtable();
            hashtable.Add(1, "Aaron");
            hashtable.Add(2, "Monica");
            hashtable.Add(3, "Michelle");
            hashtable.Add(4, "Michael");
            hashtable.Add(5, "Nathan");

            FileStream fs = new FileStream(filePath, FileMode.Create);
            DictionarySerializer ds = new DictionarySerializer(hashtable);
            XmlSerializer s = new XmlSerializer(typeof (DictionarySerializer));
            s.Serialize(fs, ds);
            fs.Close();

            FileStream fsopen = new FileStream(filePath, FileMode.Open);
            ds = (DictionarySerializer) s.Deserialize(fsopen);
            Console.WriteLine("Deserialized " + ds.dictionary.Count + " items");
        }

        [Test]
        public void ListDictionaryTest()
        {
            string methodName = "ListDictionaryTest";
            string filePath = className + "." + methodName + ".xml";
            ListDictionary listDictionary = new ListDictionary();
            listDictionary.Add(1, "Aaron");
            listDictionary.Add(2, "Monica");
            listDictionary.Add(3, "Michelle");
            listDictionary.Add(4, "Michael");
            listDictionary.Add(5, "Nathan");

            FileStream fs = new FileStream(filePath, FileMode.Create);
            DictionarySerializer ds = new DictionarySerializer(listDictionary);
            XmlSerializer s = new XmlSerializer(typeof (DictionarySerializer));
            s.Serialize(fs, ds);
            fs.Close();

            FileStream fsopen = new FileStream(filePath, FileMode.Open);
            ds = (DictionarySerializer) s.Deserialize(fsopen);
            Console.WriteLine("Deserialized " + ds.dictionary.Count + " items");
        }

        [Test]
        public void HybridDictionaryTest()
        {
            string methodName = "HybridDictionaryTest";
            string filePath = className + "." + methodName + ".xml";
            HybridDictionary hybridDictionary = new HybridDictionary();
            hybridDictionary.Add(1, "Aaron");
            hybridDictionary.Add(2, "Monica");
            hybridDictionary.Add(3, "Michelle");
            hybridDictionary.Add(4, "Michael");
            hybridDictionary.Add(5, "Nathan");

            FileStream fs = new FileStream(filePath, FileMode.Create);
            DictionarySerializer ds = new DictionarySerializer(hybridDictionary);
            XmlSerializer s = new XmlSerializer(typeof (DictionarySerializer));
            s.Serialize(fs, ds);
            fs.Close();

            FileStream fsopen = new FileStream(filePath, FileMode.Open);
            ds = (DictionarySerializer) s.Deserialize(fsopen);
            Console.WriteLine("Deserialized " + ds.dictionary.Count + " items");
        }

        [Test]
        public void SortedListTest()
        {
            string methodName = "SortedListTest";
            string filePath = className + "." + methodName + ".xml";
            SortedList sortedList = new SortedList();
            sortedList.Add(1, "Aaron");
            sortedList.Add(2, "Monica");
            sortedList.Add(3, "Michelle");
            sortedList.Add(4, "Michael");
            sortedList.Add(5, "Nathan");

            FileStream fs = new FileStream(filePath, FileMode.Create);
            DictionarySerializer ds = new DictionarySerializer(sortedList);
            XmlSerializer s = new XmlSerializer(typeof (DictionarySerializer));
            s.Serialize(fs, ds);
            fs.Close();

            FileStream fsopen = new FileStream(filePath, FileMode.Open);
            ds = (DictionarySerializer) s.Deserialize(fsopen);
            Console.WriteLine("Deserialized " + ds.dictionary.Count + " items");
        }
    }
}