﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoriesPropertiesTest
    {
        [Test]
        public void Constructor_GetFailureMechanismAssemblyCategoryFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoriesProperties(null,
                                                                                       GetFailureMechanismSectionAssemblyCategories());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyCategories", exception.ParamName);
        }

        [Test]
        public void Constructor_GetFailureMechanismSectionAssemblyCategoryFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCategoriesProperties(GetFailureMechanismAssemblyCategories(),
                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssemblyCategories", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories = GetFailureMechanismAssemblyCategories();
            IEnumerable<FailureMechanismSectionAssemblyCategory> expectedFailureMechanismSectionCategories = GetFailureMechanismSectionAssemblyCategories();

            // Call
            var properties = new FailureMechanismAssemblyCategoriesProperties(expectedFailureMechanismCategories,
                                                                              expectedFailureMechanismSectionCategories);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IEnumerable<FailureMechanismAssemblyCategory>>>(properties);
            Assert.AreSame(expectedFailureMechanismCategories, properties.Data);

            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoriesProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismAssemblyCategoriesProperties.FailureMechanismAssemblyCategories));
            TestHelper.AssertTypeConverter<FailureMechanismAssemblyCategoriesProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismAssemblyCategoriesProperties.FailureMechanismSectionAssemblyCategories));

            Assert.AreEqual(expectedFailureMechanismCategories.Count(), properties.FailureMechanismAssemblyCategories.Length);
            for (var i = 0; i < expectedFailureMechanismCategories.Count(); i++)
            {
                FailureMechanismAssemblyCategory category = expectedFailureMechanismCategories.ElementAt(i);
                FailureMechanismAssemblyCategoryProperties property = properties.FailureMechanismAssemblyCategories[i];
                Assert.AreEqual(category.Group, property.Group);
                Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
            }

            Assert.AreEqual(expectedFailureMechanismSectionCategories.Count(), properties.FailureMechanismSectionAssemblyCategories.Length);
            for (var i = 0; i < expectedFailureMechanismSectionCategories.Count(); i++)
            {
                FailureMechanismSectionAssemblyCategory category = expectedFailureMechanismSectionCategories.ElementAt(i);
                FailureMechanismSectionAssemblyCategoryProperties property = properties.FailureMechanismSectionAssemblyCategories[i];
                Assert.AreEqual(category.Group, property.Group);
                Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new FailureMechanismAssemblyCategoriesProperties(GetFailureMechanismAssemblyCategories(),
                                                                              GetFailureMechanismSectionAssemblyCategories());
            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor failureMechanismCategoriesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Categoriegrenzen voor dit traject",
                                                                            "De categoriegrenzen voor dit traject en toetsspoor.",
                                                                            true);

            PropertyDescriptor failureMechanismSectionCategoriesProperty = dynamicProperties[1];
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

        private static IEnumerable<FailureMechanismAssemblyCategory> GetFailureMechanismAssemblyCategories()
        {
            var random = new Random(21);

            return new[]
            {
                new FailureMechanismAssemblyCategory(random.NextDouble(), 
                                                     random.NextDouble(), 
                                                     random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>())
            };
        }
    }
}