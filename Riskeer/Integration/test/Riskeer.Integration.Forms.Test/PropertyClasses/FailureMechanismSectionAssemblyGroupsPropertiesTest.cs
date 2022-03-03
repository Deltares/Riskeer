﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Util;

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
                                                                            "De duidingsklassen voor het traject.",
                                                                            true);
        }

        [Test]
        public void GetFailureMechanismAssemblyGroups_AssemblySucceeds_CorrectlySetsProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var properties = new FailureMechanismSectionAssemblyGroupsProperties(assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyGroupProperties[] failureMechanismAssemblyGroups = properties.FailureMechanismAssemblyGroups;
                IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> output =
                    FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(assessmentSection);

                Assert.AreEqual(output.Count(), failureMechanismAssemblyGroups.Length);
                for (var i = 0; i < output.Count(); i++)
                {
                    FailureMechanismSectionAssemblyGroupBoundaries assemblyGroupBoundaries = output.ElementAt(i);

                    FailureMechanismSectionAssemblyGroupProperties property = failureMechanismAssemblyGroups[i];
                    Assert.AreEqual(assemblyGroupBoundaries.FailureMechanismSectionAssemblyGroup, property.Group);
                    Assert.AreEqual(assemblyGroupBoundaries.UpperBoundary, property.UpperBoundary);
                    Assert.AreEqual(assemblyGroupBoundaries.LowerBoundary, property.LowerBoundary);
                }
            }
        }
    }
}