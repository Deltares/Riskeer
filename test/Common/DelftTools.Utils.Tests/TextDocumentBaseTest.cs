using System.ComponentModel;

using DelftTools.Utils.Data;
using IEditableObject = DelftTools.Utils.Editing.IEditableObject;

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
            Assert.IsInstanceOf<IUnique<long>>(docBase);
            Assert.IsInstanceOf<IEditableObject>(docBase);
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
            Assert.IsInstanceOf<IUnique<long>>(docBase);
            Assert.IsInstanceOf<IEditableObject>(docBase);
            Assert.IsInstanceOf<INotifyPropertyChanged>(docBase);
            Assert.AreEqual(isReadOnly, docBase.ReadOnly);
            Assert.IsNull(docBase.Name);
            Assert.IsNull(docBase.Content);
        }

        class SimpleTextDocumentBase : TextDocumentBase
        {
            public SimpleTextDocumentBase()
            {
            }

            public SimpleTextDocumentBase(bool isReadOnly) : base(isReadOnly)
            {
            }
        } 
    }
}