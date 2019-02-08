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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties(context, () => (RoundedDouble) 1.1, handler);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionCategoryWaveConditionsInputContextProperties<
                StabilityStoneCoverWaveConditionsInputContext, StabilityStoneCoverWaveConditionsInput,
                StabilityStoneCoverWaveConditionsCalculationType>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual(calculation.InputParameters.CalculationType, properties.RevetmentType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptor revetmentTypeProperty = dynamicProperties[10];
            Assert.IsInstanceOf<EnumTypeConverter>(revetmentTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(revetmentTypeProperty,
                                                                            modelSettingsCategory,
                                                                            "Type bekleding",
                                                                            "Het type van de bekleding waarvoor berekend wordt.");
            mocks.VerifyAll();
        }

        [Test]
        public void RevetmentType_Always_InputChangedAndObservablesNotified()
        {
            var calculationType = new Random(21).NextEnumValue<StabilityStoneCoverWaveConditionsCalculationType>();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.RevetmentType = calculationType);
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<StabilityStoneCoverWaveConditionsInputContextProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(customHandler.Called);
            mocks.VerifyAll();
        }
    }
}