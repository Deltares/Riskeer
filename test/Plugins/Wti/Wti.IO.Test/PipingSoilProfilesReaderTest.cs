using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;
using Wti.Data;

namespace Wti.IO.Test
{
    public class PipingSoilProfilesReaderTest
    {

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSoilProfilesReader");

        [Test]
        public void Connect_CorrectPath_ReturnsTrue()
        {
            // Setup
            var testFile = "empty.soil";
            var pipingSoilProfilesReader = new PipingSoilProfilesReader(Path.Combine(testDataPath, testFile));

            // Call
            Boolean result = pipingSoilProfilesReader.Connect();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Connect_NonExistingPath_ReturnsFalse()
        {
            // Setup
            var testFile = "none.soil";
            var pipingSoilProfilesReader = new PipingSoilProfilesReader(Path.Combine(testDataPath, testFile));

            // Call
            Boolean result = pipingSoilProfilesReader.Connect();

            // Assert
            Assert.IsFalse(result);
        }


        [Test]
        public void ReadSoilProfiles_CompleteDatabase_Returns2Profiles()
        {
            // Setup
            var testFile = "complete.soil";
            var pipingSoilProfilesReader = new PipingSoilProfilesReader(Path.Combine(testDataPath, testFile));
            pipingSoilProfilesReader.Connect();

            // Call
            ICollection<PipingSoilProfile> result = pipingSoilProfilesReader.ReadSoilProfiles();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.Count(psp => psp.Id == 1 && psp.Name == "10Y_005_STBI"));
            Assert.AreEqual(1, result.Count(psp => psp.Id == 3 && psp.Name == "10Y_041_STBI"));
        }
    }
}