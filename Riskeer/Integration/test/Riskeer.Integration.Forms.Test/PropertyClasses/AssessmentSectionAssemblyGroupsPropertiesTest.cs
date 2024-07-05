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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Groups;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionAssemblyGroupsPropertiesTest
    {
        [Test]
        public void Constructor_assessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyGroupsProperties(null);

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
            var properties = new AssessmentSectionAssemblyGroupsProperties(assessmentSection);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionAssemblyGroupsProperties>(properties);
            Assert.AreSame(assessmentSection, properties.Data);
            TestHelper.AssertTypeConverter<AssessmentSectionAssemblyGroupsProperties, ExpandableArrayConverter>(
                nameof(AssessmentSectionAssemblyGroupsProperties.AssessmentSectionAssemblyGroups));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new AssessmentSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor assessmentSectionAssemblyGroupsProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(assessmentSectionAssemblyGroupsProperty,
                                                                            generalCategoryName,
                                                                            "Veiligheidscategorieën",
                                                                            "De veiligheidscategorieën voor het traject.",
                                                                            true);
        }

        [Test]
        public void GetAssessmentSectionAssemblyGroups_AssemblyThrowsException_SetsEmptyProperties()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                var properties = new AssessmentSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

                // Assert
                Assert.IsEmpty(properties.AssessmentSectionAssemblyGroups);
            }
        }

        [Test]
        public void GetAssessmentSectionAssemblyGroups_AssemblySucceeds_CorrectlySetsProperties()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator;

                // Call
                var properties = new AssessmentSectionAssemblyGroupsProperties(new AssessmentSection(AssessmentSectionComposition.Dike));

                // Assert
                AssessmentSectionAssemblyGroupProperties[] assessmentSectionAssemblyGroups = properties.AssessmentSectionAssemblyGroups;
                IEnumerable<AssessmentSectionAssemblyGroupBoundaries> output = calculator.AssessmentSectionAssemblyGroupBoundariesOutput;
                Assert.AreEqual(output.Count(), assessmentSectionAssemblyGroups.Length);
                for (var i = 0; i < output.Count(); i++)
                {
                    AssessmentSectionAssemblyGroupBoundaries assemblyGroup = output.ElementAt(i);

                    AssessmentSectionAssemblyGroupProperties property = assessmentSectionAssemblyGroups[i];
                    Assert.AreEqual(assemblyGroup.AssessmentSectionAssemblyGroup, property.Group);
                    Assert.AreEqual(assemblyGroup.UpperBoundary, property.UpperBoundary);
                    Assert.AreEqual(assemblyGroup.LowerBoundary, property.LowerBoundary);
                }
            }
        }
    }
}