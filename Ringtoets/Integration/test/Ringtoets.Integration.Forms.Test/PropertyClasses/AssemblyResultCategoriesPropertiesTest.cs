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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssemblyResultCategoriesPropertiesTest
    {
        [Test]
        public void Constructor_CategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultCategoriesProperties(null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultCategoriesProperties(Enumerable.Empty<FailureMechanismAssemblyCategory>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories = GetFailureMechanismAssemblyCategories();

            // Call
            var properties = new AssemblyResultCategoriesProperties(expectedFailureMechanismCategories, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IEnumerable<FailureMechanismAssemblyCategory>>>(properties);

            TestHelper.AssertTypeConverter<AssemblyResultCategoriesProperties, ExpandableArrayConverter>(
                nameof(AssemblyResultCategoriesProperties.AssemblyCategories));

            Assert.AreSame(expectedFailureMechanismCategories, properties.Data);
            Assert.AreEqual(assessmentSection.FailureProbabilityMarginFactor, properties.FailureProbabilityMarginFactor);
            AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismAssemblyCategoryProperties(
                expectedFailureMechanismCategories,
                properties);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new AssemblyResultCategoriesProperties(GetFailureMechanismAssemblyCategories(),
                                                                    new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor categoriesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoriesProperty,
                                                                            generalCategoryName,
                                                                            "Categoriegrenzen groep 1 en 2",
                                                                            "De categoriegrenzen voor de gecombineerde toetssporen in groep 1 en 2.",
                                                                            true);

            PropertyDescriptor failureMechanismSectionCategoriesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismSectionCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Gecombineerde faalkansruimte",
                                                                            "De gecombineerde faalkansruimte voor de toetssporen in groep 1 en 2.",
                                                                            true);
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