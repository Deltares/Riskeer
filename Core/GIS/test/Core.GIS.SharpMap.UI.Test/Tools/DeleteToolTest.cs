using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.GIS.SharpMap.UI.Test.Tools
{
    [TestFixture]
    public class DeleteToolTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [Test]
        public void CanDeleteWithoutEditableObject()
        {
            MapControl mapControl = new MapControl();

            //mapControl.ActivateTool(selectTool);

            VectorLayer vectorLayer = new VectorLayer();
            FeatureCollection layer2Data = new FeatureCollection();
            vectorLayer.DataSource = layer2Data;
            layer2Data.FeatureType = typeof(Feature);

            layer2Data.Add(new Point(4, 5));
            layer2Data.Add(new Point(0, 1));
            mapControl.Map.Layers.Add(vectorLayer);

            mapControl.SelectTool.Select((IFeature) layer2Data.Features[0]);

            mapControl.DeleteTool.DeleteSelection();
        }

        [Test]
        public void NopDeleteDoesNotTriggerBeginEdit()
        {
            MapControl mapControl = new MapControl();

            SelectTool selectTool = mapControl.SelectTool;

            VectorLayer vectorLayer = new VectorLayer();
            FeatureCollection layer2Data = new FeatureCollection();
            vectorLayer.DataSource = layer2Data;
            layer2Data.FeatureType = typeof(Feature);

            layer2Data.Add(new Point(4, 5));
            layer2Data.Add(new Point(0, 1));
            mapControl.Map.Layers.Add(vectorLayer);

            var featureMutator = mocks.StrictMock<IFeatureInteractor>();

            featureMutator.Expect(fm => fm.AllowDeletion()).Return(false).Repeat.Any();
            featureMutator.Expect(fm => fm.Delete()).Repeat.Never();

            mocks.ReplayAll();

            selectTool.Select((IFeature) layer2Data.Features[0]);

            selectTool.SelectedFeatureInteractors.Clear();
            selectTool.SelectedFeatureInteractors.Add(featureMutator); //inject our own feature editor

            mapControl.DeleteTool.DeleteSelection();

            mocks.VerifyAll();
        }

        [Test]
        public void ActualDeleteTriggersBeginEdit()
        {
            MapControl mapControl = new MapControl();

            SelectTool selectTool = mapControl.SelectTool;

            VectorLayer vectorLayer = new VectorLayer();
            FeatureCollection layer2Data = new FeatureCollection();
            vectorLayer.DataSource = layer2Data;
            layer2Data.FeatureType = typeof(Feature);

            layer2Data.Add(new Point(4, 5));
            layer2Data.Add(new Point(0, 1));
            mapControl.Map.Layers.Add(vectorLayer);

            var featureMutator = mocks.StrictMock<IFeatureInteractor>();
            
            featureMutator.Expect(fm => fm.AllowDeletion()).Return(true).Repeat.Any();
            featureMutator.Expect(fm => fm.Delete());
            
            mocks.ReplayAll();

            selectTool.Select((IFeature) layer2Data.Features[0]);

            selectTool.SelectedFeatureInteractors.Clear();
            selectTool.SelectedFeatureInteractors.Add(featureMutator); //inject our own feature editor

            mapControl.DeleteTool.DeleteSelection();

            mocks.VerifyAll();
        }
    }
}