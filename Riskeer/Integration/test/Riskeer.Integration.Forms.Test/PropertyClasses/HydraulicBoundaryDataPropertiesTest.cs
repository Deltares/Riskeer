// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDataPropertiesTest
    {
        private const int filePathPropertyIndex = 0;

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDataProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryData>>(properties);
            Assert.AreSame(hydraulicBoundaryData, properties.Data);
        }

        [Test]
        [TestCase(@"test\path\test.test", @"test\path")]
        [TestCase("", "")]
        public void GetProperties_WithData_ReturnsExpectedValues(string filePath, string expectedWorkingDirectory)
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = filePath
                }
            };

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(expectedWorkingDirectory, properties.WorkingDirectory);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            var properties = new HydraulicBoundaryDataProperties(hydraulicBoundaryData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";
            PropertyDescriptor fileDirectoryProperty = dynamicProperties[filePathPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fileDirectoryProperty,
                                                                            expectedCategory,
                                                                            "Bestandsmap",
                                                                            "Locatie van de bestandsmap.",
                                                                            true);
        }
    }
}