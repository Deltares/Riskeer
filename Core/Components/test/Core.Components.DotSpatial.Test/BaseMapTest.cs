using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.Properties;
using DotSpatial.Controls;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test
{
    [TestFixture]
    public class BaseMapTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.StrictMock<Map>();
            mocks.ReplayAll();

            // Call
            var baseMap = new BaseMap();

            // Assert
            Assert.IsInstanceOf<Control>(baseMap);
            Assert.IsNotNull(map);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_ShapeFileIsValidMissesNeededFiles_ThrowsFileNotFoundException()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();

            data.AddShapeFile(string.Format("{0}\\Resources\\DR10_segments.shp", Environment.CurrentDirectory));

            // Call
            TestDelegate setDataDelegate = () => map.SetMapData(data);

            // Assert
            Assert.Throws<FileNotFoundException>(setDataDelegate);
        }

        [Test]
        public void SetDataOnMap_FileDeleted_ThrowsFileNotFoundException()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();
            var filePath = string.Format("{0}\\Resources\\DR10_binnenteen.shp", Environment.CurrentDirectory);

            data.AddShapeFile(filePath);

            File.Delete(filePath);

            // Call
            TestDelegate testDelegate = () => map.SetMapData(data);

            // Assert
            Assert.Throws<FileNotFoundException>(testDelegate);
        }

        [Test]
        public void Data_IsValid_DoesNotThrowException()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();

            data.AddShapeFile(string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory));

            // Call
            TestDelegate setDataDelegate = () => map.SetMapData(data);

            // Assert
            Assert.DoesNotThrow(setDataDelegate);
        }
        
        [Test]
        public void SetDataOnMap_Succeeds_AddOneMapLayerAndWriteLog()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();

            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory);
            var excpectedLog = string.Format(Resources.BaseMap_LoadData_Shape_file_on_path__0__is_added_to_the_map_, filePath);

            data.AddShapeFile(filePath);

            var mapComponent = TypeUtils.GetField<Map>(map, "map");
            
            // Pre-condition
            var preLayerCount = mapComponent.GetLayers().Count;

            // Call
            Action action = () => map.SetMapData(data);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(action, excpectedLog);
            Assert.AreEqual(preLayerCount+1, mapComponent.GetLayers().Count);
        }
    }
}