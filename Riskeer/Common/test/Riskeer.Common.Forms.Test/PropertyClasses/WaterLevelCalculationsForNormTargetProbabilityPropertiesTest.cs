// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilityPropertiesTest
    {
        private const int targetProbabilityPropertyIndex = 0;
        private const int calculationsPropertyIndex = 1;

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaterLevelCalculationsForNormTargetProbabilityProperties(null, 0.1);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculations", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            // Call
            var properties = new WaterLevelCalculationsForNormTargetProbabilityProperties(hydraulicBoundaryLocationCalculations, 0.1);

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationsProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocationCalculations, properties.Data);
            TestHelper.AssertTypeConverter<WaterLevelCalculationsForNormTargetProbabilityProperties, ExpandableArrayConverter>(
                nameof(WaterLevelCalculationsForNormTargetProbabilityProperties.Calculations));
            TestHelper.AssertTypeConverter<WaterLevelCalculationsForNormTargetProbabilityProperties, NoProbabilityValueDoubleConverter>(
                nameof(WaterLevelCalculationsForNormTargetProbabilityProperties.TargetProbability));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaterLevelCalculationsForNormTargetProbabilityProperties(new ObservableList<HydraulicBoundaryLocationCalculation>(), 0.1);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            "Algemeen",
                                                                            "Doelkans [1/jaar]",
                                                                            "Overschrijdingskans waarvoor de hydraulische belastingen worden berekend.",
                                                                            true);

            PropertyDescriptor locationsProperty = dynamicProperties[calculationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische belastingendatabase.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const double targetProbability = 0.1;
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            };

            // Call
            var properties = new WaterLevelCalculationsForNormTargetProbabilityProperties(hydraulicBoundaryLocationCalculations, targetProbability);

            // Assert
            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(1, properties.Calculations.Length);
        }
    }
}