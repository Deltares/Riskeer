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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PropertyClasses;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingFailureMechanismPropertiesBaseTest
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 0;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new TestPipingFailureMechanismProperties(
                null, new PipingFailureMechanismPropertiesBase.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestPipingFailureMechanismProperties(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var properties = new TestPipingFailureMechanismProperties(
                failureMechanism, new PipingFailureMechanismPropertiesBase.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingFailureMechanism>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var properties = new TestPipingFailureMechanismProperties(
                failureMechanism, new PipingFailureMechanismPropertiesBase.ConstructionProperties
                {
                    NamePropertyIndex = namePropertyIndex,
                    CodePropertyIndex = codePropertyIndex
                });

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het faalmechanisme.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het faalmechanisme.",
                                                                            true);
        }

        private class TestPipingFailureMechanismProperties : PipingFailureMechanismPropertiesBase
        {
            public TestPipingFailureMechanismProperties(PipingFailureMechanism data, ConstructionProperties constructionProperties)
                : base(data, constructionProperties) {}
        }
    }
}