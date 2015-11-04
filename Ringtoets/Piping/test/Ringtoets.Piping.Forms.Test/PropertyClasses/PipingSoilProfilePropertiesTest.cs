﻿using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingSoilProfilePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingSoilProfileProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingSoilProfile>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some name>";
            IEnumerable<PipingSoilLayer> layers = new []
            {
                new PipingSoilLayer(-2), 
                new PipingSoilLayer(-4)
                {
                    IsAquifer = 1.0
                } 
            };
            var soilProfile = new PipingSoilProfile(expectedName, -5.0, layers);

            var properties = new PipingSoilProfileProperties
            {
                Data = soilProfile
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(soilProfile.Layers.Select(l => l.Top), properties.TopLevels);
            Assert.AreEqual(soilProfile.Bottom, properties.Bottom);
        }
    }
}