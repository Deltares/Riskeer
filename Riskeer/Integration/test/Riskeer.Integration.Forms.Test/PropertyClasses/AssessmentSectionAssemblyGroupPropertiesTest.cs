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
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionAssemblyGroupPropertiesTest
    {
        [Test]
        public void Constructor_AssemblyGroupBoundariesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyGroupProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyGroupBoundaries", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var assemblyGroup = new AssessmentSectionAssemblyGroupBoundaries(random.NextDouble(),
                                                                                random.NextDouble(),
                                                                                random.NextEnumValue<AssessmentSectionAssemblyGroup>());

            // Call
            var properties = new AssessmentSectionAssemblyGroupProperties(assemblyGroup);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<AssessmentSectionAssemblyGroupBoundaries>>(properties);
            Assert.AreSame(assemblyGroup, properties.Data);
            TestHelper.AssertTypeConverter<AssessmentSectionAssemblyGroupProperties, ExpandableObjectConverter>();

            Assert.AreEqual(assemblyGroup.AssessmentSectionAssemblyGroup, properties.Group);
            Assert.AreEqual(assemblyGroup.LowerBoundary, properties.LowerBoundary);
            Assert.AreEqual(assemblyGroup.UpperBoundary, properties.UpperBoundary);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var random = new Random(39);
            var assemblyGroup = new AssessmentSectionAssemblyGroupBoundaries(random.NextDouble(),
                                                                                random.NextDouble(),
                                                                                random.NextEnumValue<AssessmentSectionAssemblyGroup>());

            // Call
            var properties = new AssessmentSectionAssemblyGroupProperties(assemblyGroup);
            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "Naam van de duidingsklasse.",
                                                                            true);

            PropertyDescriptor lowerBoundaryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerBoundaryProperty,
                                                                            generalCategoryName,
                                                                            "Ondergrens [1/jaar]",
                                                                            "Ondergrens van de duidingsklasse.",
                                                                            true);

            PropertyDescriptor upperBoundaryProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryProperty,
                                                                            generalCategoryName,
                                                                            "Bovengrens [1/jaar]",
                                                                            "Bovengrens van de duidingsklasse.",
                                                                            true);
        }

        [Test]
        public void ToString_Always_ReturnsAssemblyGroupDisplayName()
        {
            // Setup
            var random = new Random(39);
            var assemblyGroup = random.NextEnumValue<AssessmentSectionAssemblyGroup>();
            var properties = new AssessmentSectionAssemblyGroupProperties(new AssessmentSectionAssemblyGroupBoundaries(
                                                                                                            random.NextDouble(),
                                                                                                            random.NextDouble(),
                                                                                                            assemblyGroup));

            // Call
            var result = properties.ToString();

            // Assert
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(assemblyGroup), result);
        }
    }
}