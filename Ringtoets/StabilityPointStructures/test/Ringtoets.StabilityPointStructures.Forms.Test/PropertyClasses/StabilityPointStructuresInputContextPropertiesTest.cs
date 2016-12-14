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
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructuresInputContextPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityPointStructuresInputContextProperties();

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<StabilityPointStructure, StabilityPointStructuresInput, StructuresCalculation<StabilityPointStructuresInput>, StabilityPointStructuresFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var properties = new StabilityPointStructuresInputContextProperties();

            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            StabilityPointStructuresInput input = calculation.InputParameters;
            var expectedFailureProbabilityRepairClosure = ProbabilityFormattingHelper.Format(input.FailureProbabilityRepairClosure);
            var expectedProbabilityCollisionSecondaryStructure = ProbabilityFormattingHelper.Format(input.ProbabilityCollisionSecondaryStructure);

            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreEqual(input.VolumicWeightWater, properties.VolumicWeightWater);
            Assert.AreSame(input.InsideWaterLevelFailureConstruction, properties.InsideWaterLevelFailureConstruction.Data);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);
            Assert.AreSame(input.FlowVelocityStructureClosable, properties.FlowVelocityStructureClosable.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreEqual(input.LoadSchematizationType, properties.LoadSchematizationType);
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreSame(input.ConstructiveStrengthLinearLoadModel, properties.ConstructiveStrengthLinearLoadModel.Data);
            Assert.AreSame(input.ConstructiveStrengthQuadraticLoadModel, properties.ConstructiveStrengthQuadraticLoadModel.Data);
            Assert.AreSame(input.StabilityLinearLoadModel, properties.StabilityLinearLoadModel.Data);
            Assert.AreSame(input.StabilityQuadraticLoadModel, properties.StabilityQuadraticLoadModel.Data);
            Assert.AreEqual(expectedFailureProbabilityRepairClosure, properties.FailureProbabilityRepairClosure);
            Assert.AreSame(input.FailureCollisionEnergy, properties.FailureCollisionEnergy.Data);
            Assert.AreSame(input.ShipMass, properties.ShipMass.Data);
            Assert.AreSame(input.ShipVelocity, properties.ShipVelocity.Data);
            Assert.AreEqual(input.LevellingCount, properties.LevellingCount);
            Assert.AreEqual(expectedProbabilityCollisionSecondaryStructure, properties.ProbabilityCollisionSecondaryStructure);
            Assert.AreSame(input.BankWidth, properties.BankWidth.Data);
            Assert.AreEqual(input.EvaluationLevel, properties.EvaluationLevel);
            Assert.AreEqual(input.VerticalDistance, properties.VerticalDistance);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                }
            };
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

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
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                StabilityPointStructures =
                {
                    new TestStabilityPointStructure()
                }
            };
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            var availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.StabilityPointStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 9;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            input.Attach(observerMock);

            var random = new Random(100);
            double newVolumicWeightWater = random.NextDouble();
            double newFactorStormDurationOpenStructure = random.NextDouble();
            var newInflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert;
            var newLoadSchematizationType = LoadSchematizationType.Quadratic;
            var newLevellingCount = 2;
            double newEvaluationLevel = random.NextDouble();
            double newVerticalDistance = random.NextDouble();

            // Call
            properties.VolumicWeightWater = (RoundedDouble) newVolumicWeightWater;
            properties.FactorStormDurationOpenStructure = (RoundedDouble) newFactorStormDurationOpenStructure;
            properties.InflowModelType = newInflowModelType;
            properties.LoadSchematizationType = newLoadSchematizationType;
            properties.FailureProbabilityRepairClosure = "1e-2";
            properties.LevellingCount = newLevellingCount;
            properties.ProbabilityCollisionSecondaryStructure = "1e-3";
            properties.EvaluationLevel = (RoundedDouble) newEvaluationLevel;
            properties.VerticalDistance = (RoundedDouble) newVerticalDistance;

            // Assert
            var expectedFailureProbabilityRepairClosure = ProbabilityFormattingHelper.Format(0.01);
            var expectedProbabilityCollisionSecondaryStructure = ProbabilityFormattingHelper.Format(0.001);

            Assert.AreEqual(newVolumicWeightWater, properties.VolumicWeightWater, properties.VolumicWeightWater.GetAccuracy());
            Assert.AreEqual(newFactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure.GetAccuracy());
            Assert.AreEqual(newInflowModelType, properties.InflowModelType);
            Assert.AreEqual(newLoadSchematizationType, properties.LoadSchematizationType);
            Assert.AreEqual(expectedFailureProbabilityRepairClosure, properties.FailureProbabilityRepairClosure);
            Assert.AreEqual(newLevellingCount, properties.LevellingCount);
            Assert.AreEqual(expectedProbabilityCollisionSecondaryStructure, properties.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(newEvaluationLevel, properties.EvaluationLevel, properties.EvaluationLevel.GetAccuracy());
            Assert.AreEqual(newVerticalDistance, properties.VerticalDistance, properties.VerticalDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityRepairClosure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            const int overflow = 1;
            string newProbabilityString = string.Concat(newValue.ToString("r", CultureInfo.InvariantCulture), overflow);

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityRepairClosure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityRepairClosure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetProbabilityCollisionSecondaryStructure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            const int overflow = 1;
            string newProbabilityString = string.Concat(newValue.ToString("r", CultureInfo.InvariantCulture), overflow);

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetProbabilityCollisionSecondaryStructure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProbabilityCollisionSecondaryStructure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LinearLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearLowSillVolumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearLowSillFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthLinearLoadModelProperty.Category);
            Assert.AreEqual("Lineaire belastingschematisering constructieve sterkte [kN/m²]", constructiveStrengthLinearLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de lineaire belastingschematisatie.", constructiveStrengthLinearLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearLowSillEvaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearLowSillVerticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearLowSillFailureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearLowSillLevellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearLowSillStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            Assert.AreEqual(schematizationCategory, stabilityLinearLoadModel.Category);
            Assert.AreEqual("Lineaire belastingschematisering stabiliteit [kN/m²]", stabilityLinearLoadModel.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.", stabilityLinearLoadModel.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linearLowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linearLowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linearLowSillStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linearLowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[linearLowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linearLowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linearLowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linearLowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linearLowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[linearLowSillModelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linearLowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linearLowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linearLowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[linearLowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linearLowSillStormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticLowSillVolumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticLowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticLowSillFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticLowSillFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticLowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticLowSillLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticLowSillLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticLowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering constructieve sterkte [kN/m]", constructiveStrengthQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.", constructiveStrengthQuadraticLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticLowSillBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticLowSillEvaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticLowSillVerticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticLowSillFailureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticLowSillFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticLowSillShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticLowSillShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticLowSillLevellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadraticLowSillStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering stabiliteit [kN/m]", stabilityQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.", stabilityQuadraticLoadModelProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadraticLowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadraticLowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadraticLowSillStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[quadraticLowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadraticLowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadraticLowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[quadraticLowSillModelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadraticLowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadraticLowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadraticLowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[quadraticLowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadraticLowSillStormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LinearFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linearFloodedCulvertVolumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[linearFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficiënt [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficiënt.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linearFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linearFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[linearFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linearFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthLinearLoadModelProperty.Category);
            Assert.AreEqual("Lineaire belastingschematisering constructieve sterkte [kN/m²]", constructiveStrengthLinearLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de lineaire belastingschematisatie.", constructiveStrengthLinearLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linearFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linearFloodedCulvertEvaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linearFloodedCulvertVerticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linearFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[linearFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linearFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linearFloodedCulvertLevellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linearFloodedCulvertStabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            Assert.AreEqual(schematizationCategory, stabilityLinearLoadModel.Category);
            Assert.AreEqual("Lineaire belastingschematisering stabiliteit [kN/m²]", stabilityLinearLoadModel.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.", stabilityLinearLoadModel.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linearFloodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linearFloodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linearFloodedCulvertStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linearFloodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linearFloodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linearFloodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linearFloodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linearFloodedCulvertStormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadraticFloodedCulvertVolumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[quadraticFloodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficiënt [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficiënt.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadraticFloodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadraticFloodedCulvertLoadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[quadraticFloodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadraticFloodedCulvertLevelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering constructieve sterkte [kN/m]", constructiveStrengthQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.", constructiveStrengthQuadraticLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadraticFloodedCulvertBankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadraticFloodedCulvertEvaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadraticFloodedCulvertVerticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadraticFloodedCulvertShipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadraticFloodedCulvertShipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadraticFloodedCulvertLevellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering stabiliteit [kN/m]", stabilityQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.", stabilityQuadraticLoadModelProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadraticFloodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadraticFloodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadraticFloodedCulvertStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadraticFloodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadraticFloodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadraticFloodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadraticFloodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadraticFloodedCulvertStormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(-10.0, -10.0),
                new Point2D(10.0, 10.0)
            }));

            // Call
            properties.Structure = new TestStabilityPointStructure();

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureTypeUnknown_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibleValidationMethod_LowSillStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibleValidationMethod_FloodedCulvertStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibleValidationMethod_LinearModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibleValidationMethod_QuadraticModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        #region LowSill + Linear Model property Indices

        private const int linearLowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int linearLowSillVolumicWeightWaterPropertyIndex = 1;
        private const int linearLowSillStormDurationPropertyIndex = 2;
        private const int linearLowSillInsideWaterLevelPropertyIndex = 3;
        private const int linearLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearLowSillFlowVelocityStructureClosablePropertyIndex = 5;
        private const int linearLowSillModelFactorSuperCriticalFlowPropertyIndex = 6;
        private const int linearLowSillFactorStormDurationOpenStructurePropertyIndex = 7;
        private const int linearLowSillStructurePropertyIndex = 8;
        private const int linearLowSillStructureLocationPropertyIndex = 9;
        private const int linearLowSillStructureNormalOrientationPropertyIndex = 10;
        private const int linearLowSillInflowModelTypePropertyIndex = 11;
        private const int linearLowSillLoadSchematizationTypePropertyIndex = 12;
        private const int linearLowSillWidthFlowAperturesPropertyIndex = 13;
        private const int linearLowSillFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int linearLowSillStorageStructureAreaPropertyIndex = 15;
        private const int linearLowSillAllowedLevelIncreaseStoragePropertyIndex = 16;
        private const int linearLowSillLevelCrestStructurePropertyIndex = 17;
        private const int linearLowSillThresholdHeightOpenWeirPropertyIndex = 18;
        private const int linearLowSillCriticalOvertoppingDischargePropertyIndex = 19;
        private const int linearLowSillConstructiveStrengthLinearLoadModelPropertyIndex = 20;
        private const int linearLowSillBankWidthPropertyIndex = 21;
        private const int linearLowSillEvaluationLevelPropertyIndex = 22;
        private const int linearLowSillVerticalDistancePropertyIndex = 23;
        private const int linearLowSillFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int linearLowSillFailureCollisionEnergyPropertyIndex = 25;
        private const int linearLowSillShipMassPropertyIndex = 26;
        private const int linearLowSillShipVelocityPropertyIndex = 27;
        private const int linearLowSillLevellingCountPropertyIndex = 28;
        private const int linearLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linearLowSillStabilityLinearLoadModelPropertyIndex = 30;
        private const int linearLowSillFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int linearLowSillForeshoreProfilePropertyIndex = 32;
        private const int linearLowSillUseBreakWaterPropertyIndex = 33;
        private const int linearLowSillUseForeshorePropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Linear Model property Indices

        private const int linearFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int linearFloodedCulvertVolumicWeightWaterPropertyIndex = 1;
        private const int linearFloodedCulvertStormDurationPropertyIndex = 2;
        private const int linearFloodedCulvertInsideWaterLevelPropertyIndex = 3;
        private const int linearFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linearFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 5;
        private const int linearFloodedCulvertDrainCoefficientPropertyIndex = 6;
        private const int linearFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 7;
        private const int linearFloodedCulvertStructurePropertyIndex = 8;
        private const int linearFloodedCulvertStructureLocationPropertyIndex = 9;
        private const int linearFloodedCulvertStructureNormalOrientationPropertyIndex = 10;
        private const int linearFloodedCulvertInflowModelTypePropertyIndex = 11;
        private const int linearFloodedCulvertLoadSchematizationTypePropertyIndex = 12;
        private const int linearFloodedCulvertAreaFlowAperturesPropertyIndex = 13;
        private const int linearFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int linearFloodedCulvertStorageStructureAreaPropertyIndex = 15;
        private const int linearFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 16;
        private const int linearFloodedCulvertLevelCrestStructurePropertyIndex = 17;
        private const int linearFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 18;
        private const int linearFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 19;
        private const int linearFloodedCulvertConstructiveStrengthLinearLoadModelPropertyIndex = 20;
        private const int linearFloodedCulvertBankWidthPropertyIndex = 21;
        private const int linearFloodedCulvertEvaluationLevelPropertyIndex = 22;
        private const int linearFloodedCulvertVerticalDistancePropertyIndex = 23;
        private const int linearFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int linearFloodedCulvertFailureCollisionEnergyPropertyIndex = 25;
        private const int linearFloodedCulvertShipMassPropertyIndex = 26;
        private const int linearFloodedCulvertShipVelocityPropertyIndex = 27;
        private const int linearFloodedCulvertLevellingCountPropertyIndex = 28;
        private const int linearFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linearFloodedCulvertStabilityLinearLoadModelPropertyIndex = 30;
        private const int linearFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int linearFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int linearFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int linearFloodedCulvertUseForeshorePropertyIndex = 34;

        #endregion

        #region LowSill + Quadratic Model property Indices

        private const int quadraticLowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadraticLowSillVolumicWeightWaterPropertyIndex = 1;
        private const int quadraticLowSillStormDurationPropertyIndex = 2;
        private const int quadraticLowSillInsideWaterLevelPropertyIndex = 3;
        private const int quadraticLowSillInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticLowSillFlowVelocityStructureClosablePropertyIndex = 5;
        private const int quadraticLowSillModelFactorSuperCriticalFlowPropertyIndex = 6;
        private const int quadraticLowSillFactorStormDurationOpenStructurePropertyIndex = 7;
        private const int quadraticLowSillStructurePropertyIndex = 8;
        private const int quadraticLowSillStructureLocationPropertyIndex = 9;
        private const int quadraticLowSillStructureNormalOrientationPropertyIndex = 10;
        private const int quadraticLowSillInflowModelTypePropertyIndex = 11;
        private const int quadraticLowSillLoadSchematizationTypePropertyIndex = 12;
        private const int quadraticLowSillWidthFlowAperturesPropertyIndex = 13;
        private const int quadraticLowSillFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int quadraticLowSillStorageStructureAreaPropertyIndex = 15;
        private const int quadraticLowSillAllowedLevelIncreaseStoragePropertyIndex = 16;
        private const int quadraticLowSillLevelCrestStructurePropertyIndex = 17;
        private const int quadraticLowSillThresholdHeightOpenWeirPropertyIndex = 18;
        private const int quadraticLowSillCriticalOvertoppingDischargePropertyIndex = 19;
        private const int quadraticLowSillConstructiveStrengthQuadraticLoadModelPropertyIndex = 20;
        private const int quadraticLowSillBankWidthPropertyIndex = 21;
        private const int quadraticLowSillEvaluationLevelPropertyIndex = 22;
        private const int quadraticLowSillVerticalDistancePropertyIndex = 23;
        private const int quadraticLowSillFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int quadraticLowSillFailureCollisionEnergyPropertyIndex = 25;
        private const int quadraticLowSillShipMassPropertyIndex = 26;
        private const int quadraticLowSillShipVelocityPropertyIndex = 27;
        private const int quadraticLowSillLevellingCountPropertyIndex = 28;
        private const int quadraticLowSillProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadraticLowSillStabilityQuadraticLoadModelPropertyIndex = 30;
        private const int quadraticLowSillFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int quadraticLowSillForeshoreProfilePropertyIndex = 32;
        private const int quadraticLowSillUseBreakWaterPropertyIndex = 33;
        private const int quadraticLowSillUseForeshorePropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Quadratic Model property Indices

        private const int quadraticFloodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadraticFloodedCulvertVolumicWeightWaterPropertyIndex = 1;
        private const int quadraticFloodedCulvertStormDurationPropertyIndex = 2;
        private const int quadraticFloodedCulvertInsideWaterLevelPropertyIndex = 3;
        private const int quadraticFloodedCulvertInsideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadraticFloodedCulvertFlowVelocityStructureClosablePropertyIndex = 5;
        private const int quadraticFloodedCulvertDrainCoefficientPropertyIndex = 6;
        private const int quadraticFloodedCulvertFactorStormDurationOpenStructurePropertyIndex = 7;
        private const int quadraticFloodedCulvertStructurePropertyIndex = 8;
        private const int quadraticFloodedCulvertStructureLocationPropertyIndex = 9;
        private const int quadraticFloodedCulvertStructureNormalOrientationPropertyIndex = 10;
        private const int quadraticFloodedCulvertInflowModelTypePropertyIndex = 11;
        private const int quadraticFloodedCulvertLoadSchematizationTypePropertyIndex = 12;
        private const int quadraticFloodedCulvertAreaFlowAperturesPropertyIndex = 13;
        private const int quadraticFloodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 14;
        private const int quadraticFloodedCulvertStorageStructureAreaPropertyIndex = 15;
        private const int quadraticFloodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 16;
        private const int quadraticFloodedCulvertLevelCrestStructurePropertyIndex = 17;
        private const int quadraticFloodedCulvertThresholdHeightOpenWeirPropertyIndex = 18;
        private const int quadraticFloodedCulvertCriticalOvertoppingDischargePropertyIndex = 19;
        private const int quadraticFloodedCulvertConstructiveStrengthQuadraticLoadModelPropertyIndex = 20;
        private const int quadraticFloodedCulvertBankWidthPropertyIndex = 21;
        private const int quadraticFloodedCulvertEvaluationLevelPropertyIndex = 22;
        private const int quadraticFloodedCulvertVerticalDistancePropertyIndex = 23;
        private const int quadraticFloodedCulvertFailureProbabilityRepairClosurePropertyIndex = 24;
        private const int quadraticFloodedCulvertFailureCollisionEnergyPropertyIndex = 25;
        private const int quadraticFloodedCulvertShipMassPropertyIndex = 26;
        private const int quadraticFloodedCulvertShipVelocityPropertyIndex = 27;
        private const int quadraticFloodedCulvertLevellingCountPropertyIndex = 28;
        private const int quadraticFloodedCulvertProbabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadraticFloodedCulvertStabilityQuadraticLoadModelPropertyIndex = 30;
        private const int quadraticFloodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int quadraticFloodedCulvertForeshoreProfilePropertyIndex = 32;
        private const int quadraticFloodedCulvertUseBreakWaterPropertyIndex = 33;
        private const int quadraticFloodedCulvertUseForeshorePropertyIndex = 34;

        #endregion
    }
}