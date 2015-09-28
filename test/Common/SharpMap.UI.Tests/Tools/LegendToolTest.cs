using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools.Decorations;

namespace SharpMap.UI.Tests.Tools
{
    [TestFixture]
    public class LegendToolTest
    {
        [Test,Category(TestCategory.WindowsForms)]
        [Ignore("Requires visual inspection")]
        public void IsLegendBigEnoughForContent()
        {
            var mapControl = new MapControl();

            var groupLayer = new GroupLayer("Group");
            var sublayer = new GroupLayer("Subgroup");
            var subsublayer = new GroupLayer("Subsubgroup");
            var subsubsublayer = new GroupLayer("Subsubsubgroup");
            var subsubsubsublayer = new GroupLayer("Subsubsubsubgroup");
            var vectorLayer = new VectorLayer("Very long layer name");
            vectorLayer.DataSource = new DataTableFeatureProvider("POINT(50 50)");
            subsubsubsublayer.Layers.Add(vectorLayer);
            subsubsublayer.Layers.Add(subsubsubsublayer);
            subsublayer.Layers.Add(subsubsublayer);
            sublayer.Layers.Add(subsublayer);
            groupLayer.Layers.Add(sublayer);
            mapControl.Map.Layers.Add(groupLayer);

            mapControl.GetToolByType<LegendTool>().Visible = true;

            WindowsFormsTestHelper.ShowModal(mapControl);
        }
    }
}
