using System;
using System.Linq;
using System.Reflection;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class GridBasedDialogTest
    {
        private PropertyInfo[] properties;
		
#if ! MONO		
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowDialogWithStrings()
        {
            var dialog = new GridBasedDialog();

            dialog.MasterSelectionChanged += dialog_SelectionChanged;

            Type type = dialog.GetType();
            properties = type.GetProperties();
            dialog.MasterDataSource = properties;
            WindowsFormsTestHelper.ShowModal(dialog);
        }
#endif		

        void dialog_SelectionChanged(object sender, EventArgs e)
        {
            GridBasedDialog gridBasedDialog = (GridBasedDialog)sender;

            if (1 == gridBasedDialog.MasterSelectedIndices.Count())
            {
                PropertyInfo propertyInfo = properties[gridBasedDialog.MasterSelectedIndices.ElementAt(0)];
                Type type = propertyInfo.GetType();
                PropertyInfo[] propertyInfoproperties = type.GetProperties();
                //, Value = propertyInfo.GetValue(pi, null).ToString()
                var ds = propertyInfoproperties.Select(pi => new { pi.Name }).ToArray();

                //Type type = propertyInfo.GetType();
                //PropertyInfo[] propertyInfoproperties = 
                //coverages.Select(c => new { c.Owner, Coverage = c }).ToArray();
                gridBasedDialog.SlaveDataSource = ds;
            }
        }
    }
}