using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Forms;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Forms
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
