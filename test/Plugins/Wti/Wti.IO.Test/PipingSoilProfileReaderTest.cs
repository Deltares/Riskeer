using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using log4net.Core;
using NUnit.Framework;
using Wti.Data;
using Wti.IO.Exceptions;
using Wti.IO.Properties;

namespace Wti.IO.Test
{
    public class PipingSoilProfileReaderTest
    {

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSoilProfilesReader");

        [Test]
        public void Constructor_CorrectPath_ReturnsNewInstance()
        {
            // Setup
            var testFile = "empty.soil";

            // Call
            var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile));
             
            // Assert
            Assert.NotNull(pipingSoilProfilesReader);
        }

        [Test]
        public void Constructor_NonExistingPath_ThrowsFileNotFoundException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(testFile);

            // Assert
            var exception = Assert.Throws<FileNotFoundException>(test);
            Assert.AreEqual(String.Format(Resources.Error_File_0_does_not_exist, testFile), exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ReadSoilProfiles_NullOrEmpty_ThrowsArgumentException(string fileName)
        {
            // Call
            TestDelegate test = () => new PipingSoilProfileReader(fileName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_PathMustBeSpecified, exception.Message);
        }

        [Test]
        public void ReadSoilProfiles_IncorrectFormatFile_ThrowsSqLiteException()
        {
            // Setup
            var dbName = "text";
            var testFile = Path.Combine(testDataPath, dbName + ".txt");
            var pipingSoilProfilesReader = new PipingSoilProfileReader(testFile);

            // Call
            TestDelegate test = () => pipingSoilProfilesReader.Read();

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual(String.Format(Resources.Error_SoilProfileReadFromDatabase, dbName), exception.Message);
        }
    }
}