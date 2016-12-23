// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismContextPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int lengthEffectPropertyIndex = 2;
        private const int frunupModelFactorPropertyIndex = 3;
        private const int fbFactorPropertyIndex = 4;
        private const int fnFactorPropertyIndex = 5;
        private const int fshallowModelFactorPropertyIndex = 6;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var handler = CreateSimpleHandler();

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                handler
            );

            // Assert
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, properties.Name);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, properties.Code);
            Assert.AreEqual(2, properties.LengthEffect);
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            Assert.AreEqual(generalInput.FbFactor.Mean, properties.FbFactor.Mean);
            Assert.AreEqual(generalInput.FbFactor.StandardDeviation, properties.FbFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FnFactor.Mean, properties.FnFactor.Mean);
            Assert.AreEqual(generalInput.FnFactor.StandardDeviation, properties.FnFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FrunupModelFactor.Mean, properties.FrunupModelFactor.Mean);
            Assert.AreEqual(generalInput.FrunupModelFactor.StandardDeviation, properties.FrunupModelFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FshallowModelFactor.Mean, properties.FshallowModelFactor.Mean);
            Assert.AreEqual(generalInput.FshallowModelFactor.StandardDeviation, properties.FshallowModelFactor.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            var handler = CreateSimpleHandler();
            int numberOfChangedProperties = 1;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                handler
            );
            const int newLengthEffect = 10;

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var handler = CreateSimpleHandler();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties(
                new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock),
                handler
            );

            // Assert
            var generalCategory = "Algemeen";
            var lengthEffectParameterCategory = "Lengte-effect parameters";
            var modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectProperty,
                                                                            lengthEffectParameterCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.");

            PropertyDescriptor frunupModelFactorProperty = dynamicProperties[frunupModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(frunupModelFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(frunupModelFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Frunup [-]",
                                                                            "De parameter 'Frunup' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fbModelProperty = dynamicProperties[fbFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fbModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fbModelProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fb [-]",
                                                                            "De parameter 'Fb' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fnFactorProperty = dynamicProperties[fnFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fnFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fnFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fn [-]",
                                                                            "De parameter 'Fn' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fshallowProperty = dynamicProperties[fshallowModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fshallowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fshallowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fondiep [-]",
                                                                            "De parameter 'Fondiep' die gebruikt wordt in de berekening.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        private IFailureMechanismPropertyChangeHandler CreateSimpleHandler()
        {
            var handler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler>();
            handler.Stub(h => h.ConfirmPropertyChange()).Return(true);
            handler.Stub(h => h.PropertyChanged(Arg<GrassCoverErosionInwardsFailureMechanism>.Matches(z => true))).Return(Enumerable.Empty<IObservable>());

            return handler;
        }
    }
}