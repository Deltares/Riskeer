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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;

namespace Ringtoets.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresInputContextPropertiesTest
    {
        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int structureNormalOrientationPropertyIndex = 2;
        private const int flowWidthAtBottomProtectionPropertyIndex = 3;
        private const int widthFlowAperturesPropertyIndex = 4;
        private const int storageStructureAreaPropertyIndex = 5;
        private const int allowedLevelIncreaseStoragePropertyIndex = 6;
        private const int levelCrestStructurePropertyIndex = 7;
        private const int criticalOvertoppingDischargePropertyIndex = 8;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 9;
        private const int foreshoreProfilePropertyIndex = 10;
        private const int useBreakWaterPropertyIndex = 11;
        private const int useForeshorePropertyIndex = 12;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 13;
        private const int hydraulicBoundaryLocationPropertyIndex = 14;
        private const int stormDurationPropertyIndex = 15;

        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutData_ExpectedValues()
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
        public void Constructor_WithoutHandler_ExpectedValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            // Call
            TestDelegate test = () => new HeightStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_WithDataAndHandler_ExpectedValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<HeightStructure, HeightStructuresInput, StructuresCalculation<HeightStructuresInput>, HeightStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            HeightStructuresInput input = calculation.InputParameters;
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            // Call
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationCategory = "Schematisatie";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithOrWithoutStructure_CorrectReadOnlyForStructureDependentProperties(bool hasStructure)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);
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

            AssertPropertiesInState(properties.LevelCrestStructure, expectedReadOnly);

            mockRepository.VerifyAll();
        }

        private static void AssertPropertiesInState(object properties, bool expectedReadOnly)
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(expectedReadOnly, dynamicProperties[1].IsReadOnly);
            Assert.AreEqual(expectedReadOnly, dynamicProperties[2].IsReadOnly);
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                }
            };
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            var availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableStructures_SetInputContextInstanceWithStructures_ReturnStructures()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                HeightStructures =
                {
                    new TestHeightStructure()
                }
            };
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            // Call
            var availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.HeightStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var inputContext = new HeightStructuresInputContext(calculation.InputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSectionStub);

            var newStructure = new TestHeightStructure();
            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new HeightStructuresInputContextProperties(inputContext, handler);

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(-10.0, -10.0),
                new Point2D(10.0, 10.0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            properties.Structure = newStructure;

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void LevelCrestStructure_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.LevelCrestStructure.Mean = newMean);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(Action<HeightStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            HeightStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestHeightStructure();

            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
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