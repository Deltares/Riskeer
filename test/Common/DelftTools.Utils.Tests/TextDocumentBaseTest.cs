using System.ComponentModel;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    public class TextDocumentBaseTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var docBase = new SimpleTextDocumentBase();

            // assert
            Assert.IsInstanceOf<INameable>(docBase);
            Assert.IsInstanceOf<INotifyPropertyChanged>(docBase);
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
            var docBase = new SimpleTextDocumentBase(isReadOnly);

            // assert
            Assert.IsInstanceOf<INameable>(docBase);
            Assert.IsInstanceOf<INotifyPropertyChanged>(docBase);
            Assert.AreEqual(isReadOnly, docBase.ReadOnly);
            Assert.IsNull(docBase.Name);
            Assert.IsNull(docBase.Content);
        }

        private class SimpleTextDocumentBase : TextDocumentBase
        {
            public SimpleTextDocumentBase() {}

            public SimpleTextDocumentBase(bool isReadOnly) : base(isReadOnly) {}
        }
    }
}