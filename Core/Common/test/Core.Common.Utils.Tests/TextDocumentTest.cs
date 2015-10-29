using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Core.Common.Utils.Tests
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
            var doc = new TextDocument
            {
                Name = text
            };

            // call
            var toString = doc.ToString();

            // assert
            Assert.AreEqual(text, toString);
        }

        [Test]
        public void Clone()
        {
            var orignalTextDocument = new TextDocument(true)
            {
                Name = "Name",
                Content = "Content"
            };

            var clonedTextDocument = (TextDocument) orignalTextDocument.Clone();

            Assert.AreNotSame(orignalTextDocument, clonedTextDocument);
            Assert.AreEqual(orignalTextDocument.ReadOnly, clonedTextDocument.ReadOnly);
            Assert.AreEqual(orignalTextDocument.Name, clonedTextDocument.Name);
            Assert.AreEqual(orignalTextDocument.Content, clonedTextDocument.Content);
        }
    }
}