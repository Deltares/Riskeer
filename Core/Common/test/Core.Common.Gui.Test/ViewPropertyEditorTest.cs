using System.Drawing.Design;

using Core.Common.Gui.Commands;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class ViewPropertyEditorTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var editor = new ViewPropertyEditor();

            // Assert
            Assert.IsInstanceOf<UITypeEditor>(editor);
        }

        [Test]
        public void GetEditStyle_Always_ReturnModal()
        {
            // Setup
            var editor = new ViewPropertyEditor();

            // Call
            UITypeEditorEditStyle style = editor.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.Modal, style);
        }

        [Test]
        public void EditValue_Always_OpenViewForData()
        {
            // Setup
            var editor = new ViewPropertyEditor();
            var data = new object();

            var mocks = new MockRepository();
            var commandsMock = mocks.StrictMock<IViewCommands>();
            commandsMock.Expect(c => c.OpenView(data));
            mocks.ReplayAll();

            var originalValue = ViewPropertyEditor.ViewCommands;
            try
            {
                ViewPropertyEditor.ViewCommands = commandsMock;

                // Call
                editor.EditValue(null, null, data);

                // Assert
                mocks.VerifyAll(); // Expect 'OpenView' to be called.
            }
            finally
            {
                ViewPropertyEditor.ViewCommands = originalValue;
            }
        }
    }
}