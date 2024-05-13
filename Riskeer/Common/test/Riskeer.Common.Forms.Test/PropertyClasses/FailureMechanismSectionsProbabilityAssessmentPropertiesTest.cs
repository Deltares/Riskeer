﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionsProbabilityAssessmentPropertiesTest
    {
        [Test]
        public void Constructor_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionsProbabilityAssessmentProperties(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ProbabilityAssessmentInput probabilityAssessmentInput = ProbabilityAssessmentInputTestFactory.Create();

            // Call
            void Call() => new FailureMechanismSectionsProbabilityAssessmentProperties(null, probabilityAssessmentInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            string sourcePath = TestHelper.GetScratchPadPath();

            ProbabilityAssessmentInput probabilityAssessmentInput = ProbabilityAssessmentInputTestFactory.Create();

            IEnumerable<FailureMechanismSection> sections = new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };
            failureMechanism.SetSections(sections, sourcePath);

            // Call
            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(
                       failureMechanism, probabilityAssessmentInput))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<IFailureMechanism>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(failureMechanism, properties.Data);

                TestHelper.AssertTypeConverter<FailureMechanismSectionsProperties, ExpandableArrayConverter>(
                    nameof(FailureMechanismSectionsProperties.Sections));
                Assert.AreEqual(sections.Count(), properties.Sections.Length);

                double sectionStart = 0;
                for (var i = 0; i < sections.Count(); i++)
                {
                    FailureMechanismSection section = sections.ElementAt(i);
                    FailureMechanismSectionProbabilityAssessmentProperties property = properties.Sections[i];

                    Assert.AreSame(section, property.Data);
                    Assert.AreEqual(1 + probabilityAssessmentInput.A * section.Length / probabilityAssessmentInput.B,
                                    property.N,
                                    property.N.GetAccuracy());

                    double sectionEnd = sectionStart + section.Length;
                    Assert.AreEqual(sectionStart, property.SectionStart, property.SectionStart.GetAccuracy());
                    Assert.AreEqual(sectionEnd, property.SectionEnd, property.SectionEnd.GetAccuracy());
                    sectionStart = sectionEnd;
                }

                Assert.AreEqual(sourcePath, properties.SourcePath);
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            ProbabilityAssessmentInput probabilityAssessmentInput = ProbabilityAssessmentInputTestFactory.Create();

            // Call
            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(
                       failureMechanism, probabilityAssessmentInput))
            {
                // Assert
                PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
                Assert.AreEqual(2, dynamicProperties.Count);
                var generalCategoryName = "Algemeen";

                PropertyDescriptor sourcePathProperty = dynamicProperties[0];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sourcePathProperty,
                                                                                generalCategoryName,
                                                                                "Bronlocatie",
                                                                                "De locatie van het bestand waaruit de vakindeling is geïmporteerd.",
                                                                                true);

                PropertyDescriptor sectionsProperty = dynamicProperties[1];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionsProperty,
                                                                                generalCategoryName,
                                                                                "Vakindeling",
                                                                                "Vakindeling waarmee de waterkering voor dit faalmechanisme is " +
                                                                                "geschematiseerd ten behoeve van de beoordeling.",
                                                                                true);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenFailureMechanismUpdated_RefreshRequiredEventRaised()
        {
            // Given
            ProbabilityAssessmentInput probabilityAssessmentInput = ProbabilityAssessmentInputTestFactory.Create();
            var failureMechanism = new TestFailureMechanism();

            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(
                       failureMechanism, probabilityAssessmentInput))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }
        }
    }
}