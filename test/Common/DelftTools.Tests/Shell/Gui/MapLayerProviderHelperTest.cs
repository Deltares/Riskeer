using System.Collections.Generic;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Api;
using SharpMap.Api.Layers;
using SharpMap.Layers;

namespace DelftTools.Tests.Shell.Gui
{
    [TestFixture]
    public class MapLayerProviderHelperTest
    {
        [Test]
        public void CreateLayerRecursive()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var projectItemGroupLayer = mocks.Stub<GroupLayer>();
            var projectItemLayer = mocks.Stub<ILayer>();
            var projectItem = mocks.StrictMock<IProjectItem>();
            var projectItemName = "Project item";

            projectItemGroupLayer.Layers = new EventedList<ILayer>();

            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItem,null)).Return(true);
            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItemName, projectItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(projectItem, null)).Return(projectItemGroupLayer);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemName, projectItem)).Return(projectItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemName });

            mocks.ReplayAll();

            var layerDataDictionary = new Dictionary<ILayer, object>();

            var layer = MapLayerProviderHelper.CreateLayersRecursive(projectItem, null ,new[] { layerProvider }, layerDataDictionary);

            Assert.AreEqual(layer, projectItemGroupLayer);
            Assert.AreEqual(1, projectItemGroupLayer.Layers.Count);
            Assert.AreEqual(projectItemLayer, projectItemGroupLayer.Layers[0]);

            Assert.AreEqual(projectItemName, layerDataDictionary[projectItemLayer]);

            mocks.VerifyAll();
        }

        [Test]
        public void ProviderGivingChildObjectGetsFirstChanceAtProvidingLayer()
        {
            var mocks = new MockRepository();

            var firstChanceProvider = mocks.StrictMock<IMapLayerProvider>();
            var otherProvider = mocks.StrictMock<IMapLayerProvider>();

            var projectItem = mocks.StrictMock<IProjectItem>();
            var projectItemName = "Project item";

            var objLayer = new GroupLayer();
            var subLayer = new VectorLayer();

            otherProvider.Expect(lp => lp.CanCreateLayerFor(projectItem, null)).Return(false);
            firstChanceProvider.Expect(lp => lp.CanCreateLayerFor(projectItem, null)).Return(true);
            firstChanceProvider.Expect(lp => lp.CreateLayer(projectItem, null)).Return(objLayer);
            firstChanceProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemName });
            // this is the important part; we don't want 'otherProvider' to be called here:
            firstChanceProvider.Expect(lp => lp.CanCreateLayerFor(projectItemName, projectItem)).Return(true); 
            firstChanceProvider.Expect(lp => lp.CreateLayer(projectItemName, projectItem)).Return(subLayer);

            mocks.ReplayAll();

            MapLayerProviderHelper.CreateLayersRecursive(projectItem, null, new[] {otherProvider, firstChanceProvider});

            mocks.VerifyAll();
        }

        [Test]
        public void RefreshLayersRecursive()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var projectItemGroupLayer = new GroupLayer();
            var projectItemLayer = mocks.Stub<ILayer>();

            var projectItem = mocks.StrictMock<IProjectItem>();
            var projectItemName = "Project item";

            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItem, null)).Return(true);
            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItemName, projectItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemName, projectItem)).Return(projectItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemName });

            mocks.ReplayAll();

            // Start with only a group layer
            var layerDataDictionary = new Dictionary<ILayer, object> { {projectItemGroupLayer,projectItem} };

            // RefreshLayersRecursive should detect that the layer is out of sync, and add the project item mlayer
            MapLayerProviderHelper.RefreshLayersRecursive(projectItemGroupLayer,layerDataDictionary, new[] { layerProvider }, null);

            Assert.AreEqual(1, projectItemGroupLayer.Layers.Count);
            Assert.AreEqual(projectItemLayer, projectItemGroupLayer.Layers[0]);

            Assert.AreEqual(projectItemName, layerDataDictionary[projectItemLayer]);

            mocks.VerifyAll();
        }
        
        [Test]
        public void RefreshLayersRecursiveDoesNotNeedlesslyRemoveLayer()
        {
            var mocks = new MockRepository();

            var layerProvider = mocks.StrictMock<IMapLayerProvider>();
            var projectItemGroupLayer = new GroupLayer();
            var projectItemLayer1 = mocks.Stub<ILayer>();
            var projectItemLayer2 = mocks.Stub<ILayer>();

            var projectItem = mocks.StrictMock<IProjectItem>();
            var projectItemName1 = "Project item 1"; 
            var projectItemName2 = "Project item 2";
            
            // first time (create)
            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItem, null)).Return(true).Repeat.Twice();
            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItemName1, projectItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(projectItem, null)).Return(projectItemGroupLayer);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemName1, projectItem)).Return(projectItemLayer1);
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemName1 });

            // second time (refresh)
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemName1, projectItemName2 }); 
            layerProvider.Expect(lp => lp.CanCreateLayerFor(projectItemName2, projectItem)).Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemName2, projectItem)).Return(projectItemLayer2);

            mocks.ReplayAll();

            var layerDataDictionary = new Dictionary<ILayer, object>();
            var mapLayerProviders = new[] { layerProvider };

            // create
            MapLayerProviderHelper.CreateLayersRecursive(projectItem, null, mapLayerProviders, layerDataDictionary);

            var layerAdded = 0;
            projectItemGroupLayer.Layers.CollectionChanged += (s, e) =>
                {
                    if (e.Action == NotifyCollectionChangeAction.Remove)
                        Assert.Fail("No remove should occur");
                    else if (e.Action == NotifyCollectionChangeAction.Add)
                        layerAdded++;
                };

            // refresh (but it should not remove layer for project item 1)
            MapLayerProviderHelper.RefreshLayersRecursive(projectItemGroupLayer, layerDataDictionary, mapLayerProviders, null);

            Assert.AreEqual(1, layerAdded);
            Assert.AreEqual(2, projectItemGroupLayer.Layers.Count);

            Assert.AreEqual(projectItemName1, layerDataDictionary[projectItemLayer1]);

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

            var projectItem = mocks.StrictMock<IProjectItem>();
            var projectItemNameOld = "Project item old";
            var projectItemNameNew = "Project item new";

            // verify the datasource is disposed (this happens through Layer.Dispose right now):
            oldDataSource.Expect(fp => fp.Dispose());
            oldDataSource.Expect(fp => fp.AddNewFeatureFromGeometryDelegate);
            oldDataSource.Expect(fp => fp.FeaturesChanged += null).IgnoreArguments().Repeat.Once();
            oldDataSource.Expect(fp => fp.FeaturesChanged -= null).IgnoreArguments().Repeat.Twice();
            oldDataSource.Expect(fp => fp.CoordinateSystemChanged += null).IgnoreArguments().Repeat.Once();
            oldDataSource.Expect(fp => fp.CoordinateSystemChanged -= null).IgnoreArguments().Repeat.Twice();

            // make sure the layerprovider does the right stuff:
            layerProvider.Expect(lp => lp.CanCreateLayerFor(null, projectItem)).IgnoreArguments().Repeat.Any().Return(true);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemNameOld, projectItem)).Return(oldModelItemLayer);
            layerProvider.Expect(lp => lp.CreateLayer(projectItemNameNew, projectItem)).Return(newModelItemLayer);
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemNameOld });
            layerProvider.Expect(lp => lp.ChildLayerObjects(projectItem)).Return(new[] { projectItemNameNew });

            mocks.ReplayAll();

            oldModelItemLayer.DataSource = oldDataSource;

            // Start with only a modelLayer
            var layerDataDictionary = new Dictionary<ILayer, object> { { modelLayer, projectItem } };

            // RefreshLayersRecursive should detect that the layer is out of sync, and add the old layer
            MapLayerProviderHelper.RefreshLayersRecursive(modelLayer, layerDataDictionary, new[] { layerProvider }, null);
            
            // expect the old layer to be removed (and thus diposed), and a new layer added:
            MapLayerProviderHelper.RefreshLayersRecursive(modelLayer, layerDataDictionary, new[] { layerProvider }, null);

            Assert.AreEqual(1, modelLayer.Layers.Count);
            Assert.AreEqual(newModelItemLayer, modelLayer.Layers[0]);

            mocks.VerifyAll();
        }
    }
}