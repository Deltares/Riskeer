using System;
using System.ComponentModel;

using DelftTools.TestUtils;
using DelftTools.Utils.Data;

using NUnit.Framework;

using IEditableObject = DelftTools.Utils.Editing.IEditableObject;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class TextDocumentTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var doc = new TextDocument();

            // assert
            Assert.IsInstanceOf<INameable>(doc);
            Assert.IsInstanceOf<ICloneable>(doc);
            Assert.IsInstanceOf<IUnique<long>>(doc);
            Assert.IsInstanceOf<IEditableObject>(doc);
            Assert.IsInstanceOf<INotifyPropertyChanged>(doc);
            Assert.IsFalse(doc.ReadOnly);
            Assert.IsNull(doc.Name);
            Assert.IsNull(doc.Content);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadOnlyConstructor_SetsReadOnly(bool isReadOnly)
        {
            // call
            var doc = new TextDocument(isReadOnly);

            // assert
            Assert.IsInstanceOf<INameable>(doc);
            Assert.IsInstanceOf<ICloneable>(doc);
            Assert.IsInstanceOf<IUnique<long>>(doc);
            Assert.IsInstanceOf<IEditableObject>(doc);
            Assert.IsInstanceOf<INotifyPropertyChanged>(doc);
            Assert.AreEqual(isReadOnly, doc.ReadOnly);
            Assert.IsNull(doc.Name);
            Assert.IsNull(doc.Content);
        }

        [Test]
        public void ToString_ReturnsName()
        {
            // setup
            const string text = "Test";
            var doc = new TextDocument { Name = text };

            // call
            var toString = doc.ToString();

            // assert
            Assert.AreEqual(text, toString);
        }
        
        [Test]
        public void Clone()
        {
            var doc = new TextDocument(true)
                          {
                              Content = "blabla"
                          };
            var clone = (TextDocument) doc.Clone();
            ReflectionTestHelper.AssertPublicPropertiesAreEqual(doc, clone);
            doc.Content = "kees";
            Assert.AreNotEqual(doc.Content, clone.Content);
        }
    }
}