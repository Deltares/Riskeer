using System.Collections.Generic;
using DelftTools.TestUtils;
using DeltaShell.Gui.Forms;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class SelectViewDialogTest
    {
        [Test,Category(TestCategory.WindowsForms)]
        public void ShowDialog()
        {
            const string viewType1 = "View type 1";

            var dialog = new SelectViewDialog
                             {
                                 Items = new List<string>
                                             {
                                                 viewType1,
                                                 "View type 2",
                                                 "View type 3",
                                                 "View type 4"
                                             },
                                 DefaultViewName = viewType1
                             };
            
            WindowsFormsTestHelper.ShowModal(dialog);
        }
    }
}