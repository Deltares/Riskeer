using System.Drawing;
using DelftTools.TestUtils;
using DeltaShell.Gui.Forms;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class SelectItemDialogTest
    {
        [Test,Category(TestCategory.WindowsForms)]
        public void ShowItemsInDialogTest()
        {
            SelectItemDialog dialog = new SelectItemDialog() {Text = "Which item do you want?"};
            
            dialog.AddItemType("item 1","Category 1",new Bitmap(10,10),null);
            dialog.AddItemType("item 2", "Category 2", new Bitmap(10, 10), null);
            dialog.AddItemType("item 3", "Category 2", new Bitmap(10, 10), null);
            //dialog.ShowDialog();
            WindowsFormsTestHelper.ShowModal(dialog);
            
        }

    }
}