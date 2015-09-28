using DelftTools.TestUtils;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Data;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class FeatureDataRowPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowProperties()
        {
            var table = new FeatureDataTable();

            WindowsFormsTestHelper.ShowPropertyGridForObject(new FeatureDataRowProperties { Data = table.NewRow() });
        }
    }
}