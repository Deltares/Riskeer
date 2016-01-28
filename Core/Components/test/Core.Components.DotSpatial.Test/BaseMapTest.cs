using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.Exceptions;
using Core.Components.DotSpatial.Properties;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test
{
    [TestFixture]
    public class BaseMapTest
    {
        private readonly string segmentsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_segments.shp");
        private readonly string dijkvakgebiedenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_dijkvakgebieden.shp");
        private readonly string binnenTeenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_binnenteen.shp");
        private readonly string tempTeenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_teen.shp");

        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var baseMap = new BaseMap();

            // Assert
            Assert.IsInstanceOf<Control>(baseMap);
        }

        [Test]
        public void Data_ShapeFileIsValidMissesNeededFiles_ThrowsFileNotFoundException()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();

            data.AddShapeFile(segmentsFile);

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

            data.AddShapeFile(binnenTeenFile);

            RenameFile(tempTeenFile, binnenTeenFile);

            // Call
            TestDelegate testDelegate = () => map.SetMapData(data);

            try
            {
                // Assert
                Assert.Throws<MapDataException>(testDelegate);
            }
            finally
            {
                // Place the original file back for other tests.
                RenameFile(binnenTeenFile, tempTeenFile);
            }
        }

        [Test]
        public void Data_IsValid_DoesNotThrowException()
        {
            // Setup
            var map = new BaseMap();
            var data = new MapData();

            data.AddShapeFile(dijkvakgebiedenFile);

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

            var excpectedLog = string.Format(Resources.BaseMap_LoadData_Shape_file_on_path__0__is_added_to_the_map_, dijkvakgebiedenFile);

            data.AddShapeFile(dijkvakgebiedenFile);

            var mapComponent = TypeUtils.GetField<Map>(map, "map");

            // Pre-condition
            var preLayerCount = mapComponent.GetLayers().Count;

            // Call
            Action action = () => map.SetMapData(data);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(action, excpectedLog);
            Assert.AreEqual(preLayerCount + 1, mapComponent.GetLayers().Count);
        }

        private static void RenameFile(string newPath, string path)
        {
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            File.Move(path, newPath);
        }
    }
}