using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Forms;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Forms
{
    [TestFixture]
    public class TextDocumentViewTest
    {
        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void ShowTextDocumentViewWithTextDocument()
        {
            var textDocument = new TextDocument(false) {Content = "qq1"};
            var textDocumentView = new TextDocumentView {Data = textDocument};
            
            var textBox = textDocumentView.Controls.OfType<RichTextBox>().First(); //hacky

            Assert.AreEqual(textDocument.Content, textBox.Text);
            WindowsFormsTestHelper.ShowModal(textDocumentView);
        }

        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void ShowTextDocumentUpdatingFromBackgroundWorker()
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            var textDocument = new TextDocument(false) {Content = "qq1"};

            var view = new TextDocumentView { Data = textDocument };
            WindowsFormsTestHelper.Show(view);
            var bgw = new BackgroundWorker();
            bgw.DoWork += (s, e) =>
                              {
                                  int i = 0;
                                  while (i<100)
                                  {
                                      textDocument.Content += string.Format("aapjes");
                                      Thread.Sleep(100);
                                      i++;
                                  }
                                  
                              };
            

            
            
            bgw.RunWorkerAsync();
            while (bgw.IsBusy)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            
        }
        
        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void SetNullDataInDocumentView()
        {
            var textDocument = new TextDocument(true);
            textDocument.Content = "qq1";

            var textDocumentView = new TextDocumentView();

            textDocumentView.Data = textDocument;

            //action! this should not cause an exception
            textDocumentView.Data = null;
        }
    }
}
