// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationsPropertiesTest
    {
        [Test]
        public void Constructor_SectionConfigurationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionConfigurationsProperties(null, failureMechanism, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfigurations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();

            // Call
            void Call() => new FailureMechanismSectionConfigurationsProperties(sectionConfigurations, null, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string sourcePath = "just//a//section//path";

            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>
            {
                new FailureMechanismSectionConfiguration(section)
            };

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                section
            }, sourcePath);

            // Call
            using (var properties = new FailureMechanismSectionConfigurationsProperties(sectionConfigurations, failureMechanism, random.NextRoundedDouble()))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<IObservableEnumerable<FailureMechanismSectionConfiguration>>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(sectionConfigurations, properties.Data);

                TestHelper.AssertTypeConverter<FailureMechanismSectionsProperties, ExpandableArrayConverter>(
                    nameof(FailureMechanismSectionsProperties.Sections));
                Assert.AreEqual(sectionConfigurations.Count, properties.SectionConfigurations.Length);

                double sectionStart = 0;
                for (var i = 0; i < sectionConfigurations.Count; i++)
                {
                    FailureMechanismSectionConfiguration sectionConfiguration = sectionConfigurations[i];
                    FailureMechanismSectionConfigurationProperties property = properties.SectionConfigurations[i];
                    Assert.AreSame(sectionConfiguration.Section, property.Data);

                    double sectionEnd = sectionStart + sectionConfiguration.Section.Length;
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

            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();

            // Call
            using (var properties = new FailureMechanismSectionConfigurationsProperties(sectionConfigurations, failureMechanism, double.NaN))
            {
                // Assert
                PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
                Assert.AreEqual(2, dynamicProperties.Count);
                const string generalCategoryName = "Algemeen";

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
            var failureMechanism = new TestFailureMechanism();
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>();

            using (var properties = new FailureMechanismSectionConfigurationsProperties(sectionConfigurations, failureMechanism, double.NaN))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenFailureMechanismSectionConfigurationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);
            var sectionConfigurations = new ObservableList<FailureMechanismSectionConfiguration>
            {
                sectionConfiguration
            };

            var failureMechanism = new TestFailureMechanism();

            using (var properties = new FailureMechanismSectionConfigurationsProperties(sectionConfigurations, failureMechanism, double.NaN))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                sectionConfiguration.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }
        }
    }
}