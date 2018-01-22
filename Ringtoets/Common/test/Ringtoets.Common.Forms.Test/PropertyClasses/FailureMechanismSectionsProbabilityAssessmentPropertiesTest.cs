// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
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

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsProbabilityAssessmentProperties(sections,
                                                                                                  failureMechanism,
                                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var probabilityAssessmentInput = mocks.Stub<IProbabilityAssessmentInput>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsProbabilityAssessmentProperties(null,
                                                                                                  failureMechanism,
                                                                                                  probabilityAssessmentInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var probabilityAssessmentInput = mocks.Stub<IProbabilityAssessmentInput>();
            mocks.ReplayAll();
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsProbabilityAssessmentProperties(sections,
                                                                                                  null,
                                                                                                  probabilityAssessmentInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var probabilityAssessmentInput = mocks.Stub<IProbabilityAssessmentInput>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            // Call
            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(sections,
                                                                                                failureMechanism,
                                                                                                probabilityAssessmentInput))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<IEnumerable<FailureMechanismSection>>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(sections, properties.Data);

                TestHelper.AssertTypeConverter<FailureMechanismSectionsProperties, ExpandableArrayConverter>(
                    nameof(FailureMechanismSectionsProperties.Sections));
                Assert.IsNotNull(properties.Sections);
                Assert.AreEqual(sections.Count(), properties.Sections.Length);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var probabilityAssessmentInput = mocks.Stub<IProbabilityAssessmentInput>();
            mocks.ReplayAll();

            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(sections,
                                                                                                failureMechanism,
                                                                                                probabilityAssessmentInput))
            {
                // Assert
                PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
                Assert.AreEqual(1, dynamicProperties.Count);

                PropertyDescriptor sectionsProperty = dynamicProperties[0];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionsProperty,
                                                                                "Algemeen",
                                                                                "Vakindeling",
                                                                                "Vakindeling waarmee de waterkering voor dit toetsspoor is " +
                                                                                "geschematiseerd ten behoeve van de beoordeling.",
                                                                                true);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenFailureMechanismUpdated_RefreshRequiredEventRaised()
        {
            // Given
            var mocks = new MockRepository();
            var probabilityAssessmentInput = mocks.Stub<IProbabilityAssessmentInput>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            using (var properties = new FailureMechanismSectionsProbabilityAssessmentProperties(sections,
                                                                                                failureMechanism,
                                                                                                probabilityAssessmentInput))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
                mocks.VerifyAll();
            }
        }
    }
}