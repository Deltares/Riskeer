using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Test.Forms
{
    [TestFixture]
    public class MapTreeViewNodePresenterTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void TestGetChildNodeObjects()
        {
            var mapLegendView = mocks.StrictMock<MapLegendView>();

            var map = new Map();
            map.Layers.Add(new VectorLayer
            {
                Name = "Test1", ShowInTreeView = true
            });
            map.Layers.Add(new VectorLayer
            {
                Name = "Test2", ShowInTreeView = false
            });

            mocks.ReplayAll();

            var mapTreeViewNodePresenter = new MapTreeViewNodePresenter(mapLegendView);
            var childNodes = mapTreeViewNodePresenter.GetChildNodeObjects(map);

            var enumerator = childNodes.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Test1", ((VectorLayer) enumerator.Current).Name);
            Assert.IsFalse(enumerator.MoveNext());

            mocks.VerifyAll();
        }
    }
}