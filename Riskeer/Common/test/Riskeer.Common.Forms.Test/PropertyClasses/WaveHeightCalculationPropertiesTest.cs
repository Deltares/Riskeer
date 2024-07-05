// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightCalculationPropertiesTest
    {
        private const int waveHeightPropertyIndex = 4;
        private const int convergencePropertyIndex = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new WaveHeightCalculationProperties(hydraulicBoundaryLocationCalculation, assessmentSection);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocationCalculation, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new WaveHeightCalculationProperties(hydraulicBoundaryLocationCalculation, assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(11, dynamicProperties.Count);

            const string resultCategory = "Resultaat";

            PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                            resultCategory,
                                                                            "Golfhoogte [m]",
                                                                            "Berekende golfhoogte.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            resultCategory,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de golfhoogte berekening?",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(39);
            var convergence = random.NextEnumValue<CalculationConvergence>();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), convergence)
            };

            // Call
            var properties = new WaveHeightCalculationProperties(hydraulicBoundaryLocationCalculation, assessmentSection);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, properties.Result);
            Assert.AreEqual(convergence, properties.Convergence);
            mocks.VerifyAll();
        }
    }
}