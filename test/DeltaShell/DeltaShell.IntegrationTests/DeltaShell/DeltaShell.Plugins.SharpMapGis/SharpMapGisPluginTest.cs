using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap;
using SharpMap.Layers;

namespace DeltaShell.IntegrationTests.DeltaShell.DeltaShell.Plugins.SharpMapGis
{
    [TestFixture]
    public class SharpMapGisPluginTest
    {
        [Test]
        public void SettingRenderRequiredDoesNotSetProjectIsChanged()
        {
            //test describes problem in which the project is marked 'changed' because of a rendering 
            var project = new Project();
            var map = new Map();
            var vectorLayer = new VectorLayer();

            project.Items.Add(map);
            map.Layers.Add(vectorLayer);

            project.IsChanged = false;

            //action! flip the renderrequired flag
            vectorLayer.RenderRequired = !vectorLayer.RenderRequired;

            //check the project is not marked 'changed'
            Assert.IsFalse(project.IsChanged);
        }
   }
}