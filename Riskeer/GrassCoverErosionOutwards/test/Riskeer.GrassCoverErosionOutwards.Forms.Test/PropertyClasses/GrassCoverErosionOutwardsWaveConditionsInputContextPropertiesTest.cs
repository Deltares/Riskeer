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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Riskeer.Revetment.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            Assert.IsInstanceOf<FailureMechanismCategoryWaveConditionsInputContextProperties<
                GrassCoverErosionOutwardsWaveConditionsInputContext,
                GrassCoverErosionOutwardsWaveConditionsInput,
                GrassCoverErosionOutwardsWaveConditionsCalculationType>>(properties);
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

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            PropertyDescriptor revetmentTypeProperty = dynamicProperties[10];
            Assert.IsInstanceOf<EnumTypeConverter>(revetmentTypeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(revetmentTypeProperty,
                                                                            "Modelinstellingen",
                                                                            "Type bekleding",
                                                                            "Het type van de bekleding waarvoor berekend wordt.");
            mocks.VerifyAll();
        }

        [Test]
        public void RevetmentType_SetNewValue_InputChangedAndObservablesNotified()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            var properties = new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, customHandler);

            // Call
            properties.RevetmentType = random.NextEnumValue<GrassCoverErosionOutwardsWaveConditionsCalculationType>();

            // Assert
            Assert.IsTrue(customHandler.Called);
            mocks.VerifyAll();
        }
    }
}