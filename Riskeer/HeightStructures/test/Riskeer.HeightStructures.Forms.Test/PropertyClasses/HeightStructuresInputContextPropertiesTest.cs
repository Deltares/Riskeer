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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PropertyClasses;

namespace Riskeer.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresInputContextPropertiesTest
    {
        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int hydraulicBoundaryLocationPropertyIndex = 2;

        private const int modelFactorSuperCriticalFlowPropertyIndex = 3;
        
        private const int structureNormalOrientationPropertyIndex = 4;
        private const int levelCrestStructurePropertyIndex = 5;
        private const int widthFlowAperturesPropertyIndex = 6;
        private const int stormDurationPropertyIndex = 7;

        private const int criticalOvertoppingDischargePropertyIndex = 8;
        private const int flowWidthAtBottomProtectionPropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;

        private const int storageStructureAreaPropertyIndex = 11;
        private const int allowedLevelIncreaseStoragePropertyIndex = 12;
        
        private const int foreshoreProfilePropertyIndex = 13;
        private const int useBreakWaterPropertyIndex = 14;
        private const int useForeshorePropertyIndex = 15;
        
        private const int calculateIllustrationPointsPropertyIndex = 16;

        private MockRepository mockRepository;
        private IAssessmentSection assessmentSection;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            assessmentSection = mockRepository.Stub<IAssessmentSection>();
        }

        [Test]
        public void Constructor_WithoutData_ThrowsArgumentNullException()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new HeightStructuresInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            TestDelegate test = () => new HeightStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<HeightStructure, HeightStructuresInput, StructuresCalculation<HeightStructuresInput>,
                HeightStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            HeightStructuresInput input = calculation.InputParameters;
            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);

            TestHelper.AssertTypeConverter<HeightStructuresInputContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(HeightStructuresInputContextProperties.FailureProbabilityStructureWithErosion));

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.ModelFactorSuperCriticalFlow, false, true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string modelSettingsCategory = "Modelfactoren";
            const string schematizationIncomingFlowCategory = "Schematisering instromend debiet/volume";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(17, dynamicProperties.Count);

            PropertyDescriptor modelFactorSuperCriticalFlowProperty = dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(modelFactorSuperCriticalFlowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(modelFactorSuperCriticalFlowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor overloopdebiet volkomen overlaat [-]",
                                                                            "Modelfactor voor het overloopdebiet over een volkomen overlaat.",
                                                                            true);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(levelCrestStructureProperty,
                                                                            schematizationIncomingFlowCategory,
                                                                            "Kerende hoogte [m+NAP]",
                                                                            "Kerende hoogte van het kunstwerk.",
                                                                            true);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [-]", dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Hydraulische belastingenlocatie", dynamicProperties[hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[stormDurationPropertyIndex].DisplayName);
            Assert.AreEqual("Illustratiepunten inlezen", dynamicProperties[calculateIllustrationPointsPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithOrWithoutStructure_CorrectReadOnlyForStructureDependentProperties(bool hasStructure)
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            if (hasStructure)
            {
                calculation.InputParameters.Structure = new TestHeightStructure();
            }

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            bool expectedReadOnly = !hasStructure;

            PropertyDescriptor failureProbabilityStructureWithErosionProperty = dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex];
            Assert.AreEqual(expectedReadOnly, failureProbabilityStructureWithErosionProperty.IsReadOnly);

            DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(properties.LevelCrestStructure, expectedReadOnly, expectedReadOnly);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, "path");

            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<ForeshoreProfile> availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableStructures_SetInputContextInstanceWithStructures_ReturnStructures()
        {
            // Setup
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                new TestHeightStructure()
            }, "some folder");
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<HeightStructure> availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.HeightStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);

            var newStructure = new TestHeightStructure();
            var handler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            FailureMechanismSection failureMechanismSection =
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(-10.0, -10.0),
                    new Point2D(10.0, 10.0)
                });
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection
            });
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            properties.Structure = newStructure;

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ModelFactorSuperCriticalFlow_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.ModelFactorSuperCriticalFlow.Mean = newMean);
        }

        [Test]
        public void LevelCrestStructure_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.LevelCrestStructure.Mean = newMean);
        }

        private void SetPropertyAndVerifyNotificationsAndOutput(Action<HeightStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            HeightStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestHeightStructure();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new HeightStructuresInputContext(input,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection);
            var properties = new HeightStructuresInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}