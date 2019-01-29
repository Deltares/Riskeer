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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Forms.TestUtil;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCategoriesPropertiesTest
    {
        [Test]
        public void Constructor_GetFailureMechanismSectionAssemblyCategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionAssemblyCategoriesProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssemblyCategories", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            IEnumerable<FailureMechanismSectionAssemblyCategory> expectedFailureMechanismSectionCategories = GetFailureMechanismSectionAssemblyCategories();

            // Call
            var properties = new FailureMechanismSectionAssemblyCategoriesProperties(expectedFailureMechanismSectionCategories);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IEnumerable<FailureMechanismSectionAssemblyCategory>>>(properties);
            Assert.AreSame(expectedFailureMechanismSectionCategories, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoriesProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismSectionAssemblyCategoriesProperties.FailureMechanismSectionAssemblyCategories));

            AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismSectionAssemblyCategoryProperties(
                expectedFailureMechanismSectionCategories,
                properties);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new FailureMechanismSectionAssemblyCategoriesProperties(GetFailureMechanismSectionAssemblyCategories());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor failureMechanismSectionCategoriesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismSectionCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Categoriegrenzen per vak",
                                                                            "De categoriegrenzen per vak voor dit toetsspoor.",
                                                                            true);
        }

        private static IEnumerable<FailureMechanismSectionAssemblyCategory> GetFailureMechanismSectionAssemblyCategories()
        {
            var random = new Random(21);

            return new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };
        }
    }
}