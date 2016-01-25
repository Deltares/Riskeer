using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        [Test]
        public void Constructor_Always_CreatesHashSet()
        {
            // Call
            var data = new MapData();
            var filePaths = TypeUtils.GetField<HashSet<string>>(data, "filePaths");

            // Assert
            Assert.IsNotNull(filePaths);
            Assert.IsInstanceOf<IEnumerable<string>>(data.FilePaths);
        }

        [Test]
        public void AddShapeFile_IsValid_ReturnsTrue()
        {
            // Setup
            var data = new MapData();
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory);

            // Call
            var succeeded = data.AddShapeFile(filePath);

            // Assert
            Assert.IsTrue(succeeded);
        }

        [Test]
        public void AddShapeFile_IsDuplicate_ReturnsFalse()
        {
            // Setup
            var data = new MapData();
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory);

            // Call
            var succeededFirst = data.AddShapeFile(filePath);
            var succeededSecond = data.AddShapeFile(filePath);

            // Assert
            Assert.IsTrue(succeededFirst);
            Assert.IsFalse(succeededSecond);
        }

        [Test]
        public void AddShapeFile_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Setup
            var data = new MapData();
            var filePath = "Test";

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(filePath);

            // Assert
            Assert.Throws<FileNotFoundException>(testDelegate);
        }

        [Test]
        public void AddShapeFile_ExtensionIsInvalid_ThrowsArgumentException()
        {
            // Setup
            var data = new MapData();
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.dbf", Environment.CurrentDirectory);

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(filePath);

            // Assert
            Assert.Throws<ArgumentException>(testDelegate);
        }

        [Test]
        public void AddShapeFile_IsNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MapData();
            
            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(null);

            // Assert
            Assert.Throws<ArgumentNullException>(testDelegate);
        }

        [Test]
        public void FilePaths_Always_EqualsAddedShapeFiles()
        {
            // Setup
            var data = new MapData();
            var filePathsToAdd = new string[]
            {
                string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory),
                string.Format("{0}\\Resources\\DR10_segments.shp", Environment.CurrentDirectory)
            };

            var addedPaths = new HashSet<string>();

            foreach (string path in filePathsToAdd)
            {
                // Call
                data.AddShapeFile(path);
                addedPaths.Add(path);

                // Assert
                Assert.AreEqual(data.FilePaths, addedPaths);
            }
        }
    }
}