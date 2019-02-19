// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PropertyClasses;

namespace Riskeer.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GeneralWaveConditionsInputPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GeneralWaveConditionsInputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GeneralWaveConditionsInput>>(properties);
            Assert.IsEmpty(properties.ToString());
        }

        [Test]
        public void Data_SetNewGeneralWaveConditionsInput_ReturnCorrectPropertyValues()
        {
            // Setup
            var properties = new GeneralWaveConditionsInputProperties();
            var random = new Random(21);
            double a = random.NextDouble();
            double b = random.NextDouble();
            double c = random.NextDouble();

            // Call
            properties.Data = new GeneralWaveConditionsInput(a, b, c);

            // Assert
            Assert.AreEqual(a, properties.A, properties.A.GetAccuracy());
            Assert.AreEqual(b, properties.B, properties.B.GetAccuracy());
            Assert.AreEqual(c, properties.C, properties.C.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new GeneralWaveConditionsInputProperties
            {
                Data = new GeneralWaveConditionsInput(0.1, 0.2, 0.3)
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string miscCategory = "Misc";

            PropertyDescriptor aProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(aProperty,
                                                                            miscCategory,
                                                                            "a",
                                                                            "De waarde van de parameter 'a' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor bProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bProperty,
                                                                            miscCategory,
                                                                            "b",
                                                                            "De waarde van de parameter 'b' in de berekening voor golfcondities.",
                                                                            true);

            PropertyDescriptor cProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(cProperty,
                                                                            miscCategory,
                                                                            "c",
                                                                            "De waarde van de parameter 'c' in de berekening voor golfcondities.",
                                                                            true);
        }
    }
}