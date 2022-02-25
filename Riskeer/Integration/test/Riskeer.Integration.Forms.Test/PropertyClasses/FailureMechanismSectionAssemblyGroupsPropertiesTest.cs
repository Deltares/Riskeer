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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupsPropertiesTest
    {
        [Test]
        public void Constructor_assessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionAssemblyGroupsProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            var properties = new FailureMechanismSectionAssemblyGroupsProperties(assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionAssemblyGroupsProperties>(properties);
            Assert.AreSame(assessmentSection, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismSectionAssemblyGroupsProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismSectionAssemblyGroupsProperties.FailureMechanismAssemblyGroups));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new FailureMechanismSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor failureMechanismSectionCategoriesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(failureMechanismSectionCategoriesProperty,
                                                                            generalCategoryName,
                                                                            "Duidingsklassen",
                                                                            "De duidingsklassen per vak voor dit toetsspoor.",
                                                                            true);
        }

        [Test]
        public void GetFailureMechanismAssemblyGroups_AssemblyThrowsException_SetsEmptyProperties()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                var properties = new FailureMechanismSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

                // Assert
                Assert.IsEmpty(properties.FailureMechanismAssemblyGroups);
            }
        }

        [Test]
        public void GetFailureMechanismAssemblyGroups_AssemblySucceeds_CorrectlySetsProperties()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;

                // Call
                var properties = new FailureMechanismSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

                // Assert
                FailureMechanismSectionAssemblyGroupProperties[] failureMechanismAssemblyGroups = properties.FailureMechanismAssemblyGroups;
                IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> output = calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput;
                Assert.AreEqual(output.Count(), failureMechanismAssemblyGroups.Length);
                for (var i = 0; i < output.Count(); i++)
                {
                    FailureMechanismSectionAssemblyGroupBoundaries category = output.ElementAt(i);

                    FailureMechanismSectionAssemblyGroupProperties property = failureMechanismAssemblyGroups[i];
                    Assert.AreEqual(DisplayFailureMechanismSectionAssemblyGroupConverter.Convert(category.Group), property.Group);
                    Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                    Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
                }
            }
        }
    }
}