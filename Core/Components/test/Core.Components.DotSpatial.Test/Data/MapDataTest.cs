using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.Exceptions;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        private readonly string segmentsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_segments.shp");
        private readonly string dijkvakgebiedenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_dijkvakgebieden.shp");
        private readonly string dijkvakgebiedenDbf = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_dijkvakgebieden.dbf");
        private readonly string binnenTeenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_binnenteen.shp");
        private readonly string tempTeenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Components.DotSpatial, "ShapeFiles"), "DR10_teen.shp");

        [Test]
        public void Constructor_Always_CreatesHashSet()
        {
            // Call
            var data = new MapData();
            var filePaths = TypeUtils.GetField<HashSet<string>>(data, "filePaths");

            // Assert
            Assert.IsNotNull(filePaths);
            CollectionAssert.IsEmpty(data.FilePaths);
        }

        [Test]
        public void AddShapeFile_IsValid_ReturnsTrue()
        {
            // Setup
            var data = new MapData();

            // Call
            var succeeded = data.AddShapeFile(dijkvakgebiedenFile);

            // Assert
            Assert.IsTrue(succeeded);
            CollectionAssert.AreEquivalent(new[]
            {
                dijkvakgebiedenFile
            }, data.FilePaths);
        }

        [Test]
        public void AddShapeFile_IsDuplicate_ReturnsFalse()
        {
            // Setup
            var data = new MapData();

            // Call
            var succeededFirst = data.AddShapeFile(dijkvakgebiedenFile);
            var succeededSecond = data.AddShapeFile(dijkvakgebiedenFile);

            // Assert
            Assert.IsTrue(succeededFirst);
            Assert.IsFalse(succeededSecond);
            CollectionAssert.AreEquivalent(new[]
            {
                dijkvakgebiedenFile
            }, data.FilePaths);
        }

        [Test]
        public void AddShapeFile_FileDoesNotExist_ThrowsMapDataException()
        {
            // Setup
            var data = new MapData();
            var filePath = "Test";

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(filePath);

            // Assert
            MapDataException exception = Assert.Throws<MapDataException>(testDelegate);
            string expectedMessage = string.Format("Bestand op pad {0} bestaat niet.", filePath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void AddShapeFile_ExtensionIsInvalid_ThrowsMapDataException()
        {
            // Setup
            var data = new MapData();

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(dijkvakgebiedenDbf);

            // Assert
            MapDataException exception = Assert.Throws<MapDataException>(testDelegate);
            string expectedMessage = string.Format("Bestand op pad {0} heeft niet de juiste .shp extensie.", dijkvakgebiedenDbf);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void AddShapeFile_IsNull_ThrowsMapDataException()
        {
            // Setup
            var data = new MapData();

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(null);

            // Assert
            MapDataException exception = Assert.Throws<MapDataException>(testDelegate);
            string expectedMessage = "A path is required when adding shape files";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void FilePaths_Always_EqualsAddedShapeFiles()
        {
            // Setup
            var data = new MapData();
            var filePathsToAdd = new string[]
            {
                dijkvakgebiedenFile,
                segmentsFile
            };

            var addedPaths = new HashSet<string>();

            foreach (string path in filePathsToAdd)
            {
                // Call
                data.AddShapeFile(path);
                addedPaths.Add(path);

                // Assert
                CollectionAssert.AreEqual(addedPaths, data.FilePaths);
            }
        }

        [Test]
        public void IsValid_FilePathsEmpty_ReturnsFalse()
        {
            // Setup
            var data = new MapData();

            // Call
            bool result = data.IsValid();

            // Assert
            Assert.IsFalse(result);
            CollectionAssert.IsEmpty(data.FilePaths);
        }

        [Test]
        public void IsValid_FilePathValid_ReturnsTrue()
        {
            // Setup
            var data = new MapData();
            data.AddShapeFile(binnenTeenFile);

            // Call
            bool result = data.IsValid();

            // Assert
            Assert.IsTrue(result);
            CollectionAssert.AreEqual(new[]
            {
                binnenTeenFile
            }, data.FilePaths);
        }

        [Test]
        public void IsValid_FilePathsNotValid_ReturnsFalse()
        {
            // Setup
            var data = new MapData();

            data.AddShapeFile(binnenTeenFile);

            // Pre-condition
            bool result = data.IsValid();

            RenameFile(tempTeenFile, binnenTeenFile);

            try
            {
                // Call
                bool resultAfterRename = data.IsValid();

                // Assert
                Assert.AreNotEqual(result, resultAfterRename);
                Assert.IsTrue(result);
                Assert.IsFalse(resultAfterRename);
            }
            finally
            {
                // Place original file back for other tests.
                RenameFile(binnenTeenFile, tempTeenFile);
            }
        }

        [Test]
        public void IsValid_OneFilePathIsInvalid_ReturnsFalse()
        {
            // Setup
            var data = new MapData();
            var paths = new[]
            {
                binnenTeenFile,
                dijkvakgebiedenFile
            };

            foreach (var path in paths)
            {
                data.AddShapeFile(path);
            }

            // Pre-condition
            bool result = data.IsValid();

            RenameFile(tempTeenFile, paths[0]);

            try
            {
                // Call
                bool resultAfterRename = data.IsValid();

                // Assert
                Assert.AreNotEqual(result, resultAfterRename);
                Assert.IsTrue(result);
                Assert.IsFalse(resultAfterRename);
                CollectionAssert.AreEquivalent(paths, data.FilePaths);
            }
            finally
            {
                // Place the original file back for other tests.
                RenameFile(paths[0], tempTeenFile);
            }
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