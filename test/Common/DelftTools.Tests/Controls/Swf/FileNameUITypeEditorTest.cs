using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class FileNameUITypeEditorTest
    {
        [Test, NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void ShowInPropertyGrid()
        {
            var grid = new PropertyGrid
                {
                    SelectedObject = new TestProperties()
                };

            WindowsFormsTestHelper.ShowModal(grid);
        }
    }

    public class TestProperties
    {
        [Editor(typeof(FileNameUITypeEditor), typeof(UITypeEditor))]
        [SaveFile]
        [FileDialogFilter("Assembly files (*.dll, *.exe)|*.dll;*.exe|All files (*.*)|*.*")]
        [FileDialogTitle("Set test save title")]
        public string SaveFileEditorExample { get; set; }

        [Editor(typeof(FileNameUITypeEditor), typeof(UITypeEditor))]
        [FileDialogFilter("Text files (*.txt)|*.txt|All files (*.*)|*.*")]
        [FileDialogTitle("Set test open title")]
        public string OpenFileEditorExample { get; set; }

        [Editor(typeof(FileNameUITypeEditor), typeof(UITypeEditor))]
        [SelectDirOnly]
        public string SelectDirExample { get; set; }
    }
}
