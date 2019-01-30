// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionsPropertiesTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionsProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            string sourcePath = TestHelper.GetScratchPadPath();
            IEnumerable<FailureMechanismSection> sections = new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            failureMechanism.SetSections(sections, sourcePath);

            // Call
            using (var properties = new FailureMechanismSectionsProperties(failureMechanism))
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
                    FailureMechanismSectionProperties property = properties.Sections[i];
                    Assert.AreSame(section, property.Data);

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

            // Call
            using (var properties = new FailureMechanismSectionsProperties(failureMechanism))
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
            var failureMechanism = new TestFailureMechanism();

            using (var properties = new FailureMechanismSectionsProperties(failureMechanism))
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