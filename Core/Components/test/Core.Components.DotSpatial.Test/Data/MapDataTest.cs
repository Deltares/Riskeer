﻿using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.Exceptions;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        [TestFixtureTearDown]
        public void CleanRenamedFile()
        {
            var newPath = string.Format("{0}\\Resources\\DR10_binnenteen.shp", Environment.CurrentDirectory
            var oldPath = string.Format("{0}\\Resources\\DR10_teen.shp", Environment.CurrentDirectory);

            RenameFile(newPath, oldPath);

            File.Delete(oldPath);
        }

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
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory);

            // Call
            var succeeded = data.AddShapeFile(filePath);

            // Assert
            Assert.IsTrue(succeeded);
            CollectionAssert.AreEquivalent(new[]
            {
                filePath
            }, data.FilePaths);
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
            CollectionAssert.AreEquivalent(new[]
            {
                filePath
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
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.dbf", Environment.CurrentDirectory);

            // Call
            TestDelegate testDelegate = () => data.AddShapeFile(filePath);

            // Assert
            MapDataException exception = Assert.Throws<MapDataException>(testDelegate);
            string expectedMessage = string.Format("Bestand op pad {0} heeft niet de juiste .shp extensie.", filePath);
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
            var path = string.Format("{0}\\Resources\\DR10_binnenteen.shp", Environment.CurrentDirectory);
            data.AddShapeFile(path);

            // Call
            bool result = data.IsValid();

            // Assert
            Assert.IsTrue(result);
            CollectionAssert.AreEqual(new[]
            {
                path
            }, data.FilePaths);
        }

        [Test]
        public void IsValid_FilePathsNotValid_ReturnsFalse()
        {
            // Setup
            var data = new MapData();
            var path = string.Format("{0}\\Resources\\DR10_binnenteen.shp", Environment.CurrentDirectory);
            var newPath = string.Format("{0}\\Resources\\DR10_teen.shp", Environment.CurrentDirectory);

            data.AddShapeFile(path);

            // Pre-condition
            bool result = data.IsValid();

            RenameFile(newPath, path);

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
                RenameFile(path, newPath);
            }
        }

        [Test]
        public void IsValid_OneFilePathIsInvalid_ReturnsFalse()
        {
            // Setup
            var data = new MapData();
            var paths = new[]
            {
                string.Format("{0}\\Resources\\DR10_binnenteen.shp", Environment.CurrentDirectory),
                string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory),
            };
            var newPath = string.Format("{0}\\Resources\\DR10_teen.shp", Environment.CurrentDirectory);

            foreach (var path in paths)
            {
                data.AddShapeFile(path);
            }

            // Pre-condition
            bool result = data.IsValid();

            RenameFile(newPath, paths[0]);

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
                RenameFile(paths[0], newPath);
            }
        }

        private static void RenameFile(string newPath, string path)
        {
            File.Delete(newPath);
            File.Move(path, newPath);
        }
    }
}