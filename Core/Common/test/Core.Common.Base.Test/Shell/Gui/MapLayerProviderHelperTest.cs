using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Shell.Gui
{
    [TestFixture]
    public class MapLayerProviderHelperTest
    {
        [Test]
        public void CreateLayerRecursive()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var dummyItemGroupLayer = mocks.Stub<GroupLayer>();
            var dummyItemLayer = mocks.Stub<ILayer>();
            var dummyItem = mocks.StrictMock<IDummyItem>();
            var dummyItemName = "Dummy item";

            dummyItemGroupLayer.Layers = new EventedList<ILayer>();

            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItem, null)).Return(true);
            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItemName, dummyItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItem, null)).Return(dummyItemGroupLayer);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemName, dummyItem)).Return(dummyItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemName
            });

            mocks.ReplayAll();

            var layerDataDictionary = new Dictionary<ILayer, object>();

            var layer = MapLayerProviderHelper.CreateLayersRecursive(dummyItem, null, new[]
            {
                layerProvider
            }, layerDataDictionary);

            Assert.AreEqual(layer, dummyItemGroupLayer);
            Assert.AreEqual(1, dummyItemGroupLayer.Layers.Count);
            Assert.AreEqual(dummyItemLayer, dummyItemGroupLayer.Layers[0]);

            Assert.AreEqual(dummyItemName, layerDataDictionary[dummyItemLayer]);

            mocks.VerifyAll();
        }

        [Test]
        public void ProviderGivingChildObjectGetsFirstChanceAtProvidingLayer()
        {
            var mocks = new MockRepository();

            var firstChanceProvider = mocks.StrictMock<IMapLayerProvider>();
            var otherProvider = mocks.StrictMock<IMapLayerProvider>();

            var dummyItem = mocks.StrictMock<IDummyItem>();
            var dummyItemName = "Dummy item";

            var objLayer = new GroupLayer();
            var subLayer = new VectorLayer();

            otherProvider.Expect(lp => lp.CanCreateLayerFor(dummyItem, null)).Return(false);
            firstChanceProvider.Expect(lp => lp.CanCreateLayerFor(dummyItem, null)).Return(true);
            firstChanceProvider.Expect(lp => lp.CreateLayer(dummyItem, null)).Return(objLayer);
            firstChanceProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemName
            });
            // this is the important part; we don't want 'otherProvider' to be called here:
            firstChanceProvider.Expect(lp => lp.CanCreateLayerFor(dummyItemName, dummyItem)).Return(true);
            firstChanceProvider.Expect(lp => lp.CreateLayer(dummyItemName, dummyItem)).Return(subLayer);

            mocks.ReplayAll();

            MapLayerProviderHelper.CreateLayersRecursive(dummyItem, null, new[]
            {
                otherProvider,
                firstChanceProvider
            });

            mocks.VerifyAll();
        }

        [Test]
        public void RefreshLayersRecursive()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var dummyItemGroupLayer = new GroupLayer();
            var dummyItemLayer = mocks.Stub<ILayer>();

            var dummyItem = mocks.StrictMock<IDummyItem>();
            var dummyItemName = "Dummy item";

            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItem, null)).Return(true);
            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItemName, dummyItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemName, dummyItem)).Return(dummyItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemName
            });

            mocks.ReplayAll();

            // Start with only a group layer
            var layerDataDictionary = new Dictionary<ILayer, object>
            {
                {
                    dummyItemGroupLayer, dummyItem
                }
            };

            // RefreshLayersRecursive should detect that the layer is out of sync, and add the dummy item mlayer
            MapLayerProviderHelper.RefreshLayersRecursive(dummyItemGroupLayer, layerDataDictionary, new[]
            {
                layerProvider
            }, null);

            Assert.AreEqual(1, dummyItemGroupLayer.Layers.Count);
            Assert.AreEqual(dummyItemLayer, dummyItemGroupLayer.Layers[0]);

            Assert.AreEqual(dummyItemName, layerDataDictionary[dummyItemLayer]);

            mocks.VerifyAll();
        }

        [Test]
        public void RefreshLayersRecursiveDoesNotNeedlesslyRemoveLayer()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var dummyItemGroupLayer = new GroupLayer();
            var dummyItemLayer1 = mocks.Stub<ILayer>();
            var dummyItemLayer2 = mocks.Stub<ILayer>();

            var dummyItem = mocks.StrictMock<IDummyItem>();
            var dummyItemName1 = "Dummy item 1";
            var dummyItemName2 = "Dummy item 2";

            // first time (create)
            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItem, null)).Return(true).Repeat.Twice();
            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItemName1, dummyItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItem, null)).Return(dummyItemGroupLayer);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemName1, dummyItem)).Return(dummyItemLayer1);
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemName1
            });

            // second time (refresh)
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemName1,
                dummyItemName2
            });
            layerProvider.Expect(lp => lp.CanCreateLayerFor(dummyItemName2, dummyItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemName2, dummyItem)).Return(dummyItemLayer2);

            mocks.ReplayAll();

            var layerDataDictionary = new Dictionary<ILayer, object>();
            var mapLayerProviders = new[]
            {
                layerProvider
            };

            // create
            MapLayerProviderHelper.CreateLayersRecursive(dummyItem, null, mapLayerProviders, layerDataDictionary);

            var layerAdded = 0;
            dummyItemGroupLayer.Layers.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangeAction.Remove)
                {
                    Assert.Fail("No remove should occur");
                }
                else if (e.Action == NotifyCollectionChangeAction.Add)
                {
                    layerAdded++;
                }
            };

            // refresh (but it should not remove layer for dummy item 1)
            MapLayerProviderHelper.RefreshLayersRecursive(dummyItemGroupLayer, layerDataDictionary, mapLayerProviders, null);

            Assert.AreEqual(1, layerAdded);
            Assert.AreEqual(2, dummyItemGroupLayer.Layers.Count);

            Assert.AreEqual(dummyItemName1, layerDataDictionary[dummyItemLayer1]);

            mocks.VerifyAll();
        }

        [Test]
        public void RefreshLayersRecursiveShouldDisposeLayersOnRemove()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var modelLayer = new GroupLayer();

            var oldDataSource = mocks.StrictMock<IFeatureProvider>();
            var oldModelItemLayer = new VectorLayer();
            var newModelItemLayer = mocks.Stub<ILayer>();

            var dummyItem = mocks.StrictMock<IDummyItem>();
            var dummyItemNameOld = "Dummy item old";
            var dummyItemNameNew = "Dummy item new";

            // verify the datasource is disposed (this happens through Layer.Dispose right now):
            oldDataSource.Expect(fp => fp.Dispose());
            oldDataSource.Expect(fp => fp.AddNewFeatureFromGeometryDelegate);
            oldDataSource.Expect(fp => fp.FeaturesChanged += null).IgnoreArguments().Repeat.Once();
            oldDataSource.Expect(fp => fp.FeaturesChanged -= null).IgnoreArguments().Repeat.Twice();
            oldDataSource.Expect(fp => fp.CoordinateSystemChanged += null).IgnoreArguments().Repeat.Once();
            oldDataSource.Expect(fp => fp.CoordinateSystemChanged -= null).IgnoreArguments().Repeat.Twice();

            // make sure the layerprovider does the right stuff:
            layerProvider.Expect(lp => lp.CanCreateLayerFor(null, dummyItem)).IgnoreArguments().Repeat.Any().Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemNameOld, dummyItem)).Return(oldModelItemLayer);
            layerProvider.Expect(lp => lp.CreateLayer(dummyItemNameNew, dummyItem)).Return(newModelItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemNameOld
            });
            layerProvider.Expect(lp => lp.ChildLayerObjects(dummyItem)).Return(new[]
            {
                dummyItemNameNew
            });

            mocks.ReplayAll();

            oldModelItemLayer.DataSource = oldDataSource;

            // Start with only a modelLayer
            var layerDataDictionary = new Dictionary<ILayer, object>
            {
                {
                    modelLayer, dummyItem
                }
            };

            // RefreshLayersRecursive should detect that the layer is out of sync, and add the old layer
            MapLayerProviderHelper.RefreshLayersRecursive(modelLayer, layerDataDictionary, new[]
            {
                layerProvider
            }, null);

            // expect the old layer to be removed (and thus diposed), and a new layer added:
            MapLayerProviderHelper.RefreshLayersRecursive(modelLayer, layerDataDictionary, new[]
            {
                layerProvider
            }, null);

            Assert.AreEqual(1, modelLayer.Layers.Count);
            Assert.AreEqual(newModelItemLayer, modelLayer.Layers[0]);

            mocks.VerifyAll();
        }

        public interface IDummyItem {}
    }
}