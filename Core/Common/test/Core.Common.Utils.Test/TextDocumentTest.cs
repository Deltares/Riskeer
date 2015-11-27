using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    public class TextDocumentTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var docBase = new TextDocument();

            // assert
            Assert.IsFalse(docBase.ReadOnly);
            Assert.IsNull(docBase.Name);
            Assert.IsNull(docBase.Content);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadOnlyConstructor_ExpectedValues(bool isReadOnly)
        {
            // call
            var docBase = new TextDocument(isReadOnly);

            // assert
            Assert.AreEqual(isReadOnly, docBase.ReadOnly);
            Assert.IsNull(docBase.Name);
            Assert.IsNull(docBase.Content);
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
    }
}