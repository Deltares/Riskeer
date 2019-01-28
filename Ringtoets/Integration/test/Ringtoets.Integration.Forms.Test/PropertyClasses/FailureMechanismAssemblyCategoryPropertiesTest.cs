// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoryPropertiesTest
    {
        [Test]
        public void Constructor_AssemblyCategoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoryProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assemblyCategory", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var assemblyCategory = new FailureMechanismAssemblyCategory(random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            // Call
            var properties = new FailureMechanismAssemblyCategoryProperties(assemblyCategory);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismAssemblyCategory>>(properties);
            Assert.AreSame(assemblyCategory, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoryProperties, ExpandableObjectConverter>();

            Assert.AreEqual(assemblyCategory.Group, properties.Group);
            Assert.AreEqual(assemblyCategory.LowerBoundary, properties.LowerBoundary);
            Assert.AreEqual(assemblyCategory.UpperBoundary, properties.UpperBoundary);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var random = new Random(39);
            var assemblyCategory = new FailureMechanismAssemblyCategory(random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            // Call
            var properties = new FailureMechanismAssemblyCategoryProperties(assemblyCategory);
            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "Naam van de categorie.",
                                                                            true);

            PropertyDescriptor lowerBoundaryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerBoundaryProperty,
                                                                            generalCategoryName,
                                                                            "Ondergrens [1/jaar]",
                                                                            "Ondergrens van de categorie.",
                                                                            true);

            PropertyDescriptor upperBoundaryProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryProperty,
                                                                            generalCategoryName,
                                                                            "Bovengrens [1/jaar]",
                                                                            "Bovengrens van de categorie.",
                                                                            true);
        }

        [Test]
        public void ToString_Always_ReturnsCategoryGroupDisplayName()
        {
            // Setup
            var random = new Random(39);
            var categoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
            var properties = new FailureMechanismAssemblyCategoryProperties(new FailureMechanismAssemblyCategory(random.NextDouble(),
                                                                                                                 random.NextDouble(),
                                                                                                                 categoryGroup));

            // Call
            string result = properties.ToString();

            // Assert
            Assert.AreEqual(new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(categoryGroup).DisplayName,
                            result);
        }
    }
}