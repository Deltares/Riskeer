using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Forms;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test.Forms
{
    [TestFixture]
    public class TextDocumentViewTest
    {
        [Test]
        public void SetNullDataInDocumentView()
        {
            var textDocument = new TextDocument(true)
            {
                Content = "Lorem ipsum"
            };

            var textDocumentView = new TextDocumentView
            {
                Data = textDocument
            };

            textDocumentView.Data = null; // Note: this should not cause an exception
        }
    }
}