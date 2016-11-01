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
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresInputContextPropertiesTest
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
            var properties = new ClosingStructuresInputContextProperties();

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput, StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var properties = new ClosingStructuresInputContextProperties();

            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            ClosingStructuresInput input = calculation.InputParameters;
            var expectedProbabilityOpenStructureBeforeFlooding = ProbabilityFormattingHelper.Format(input.ProbabilityOpenStructureBeforeFlooding);
            var expectedFailureProbabilityOpenStructure = ProbabilityFormattingHelper.Format(input.FailureProbabilityOpenStructure);
            var expectedFailureProbabilityReparation = ProbabilityFormattingHelper.Format(input.FailureProbabilityReparation);

            Assert.AreEqual(input.DeviationWaveDirection, properties.DeviationWaveDirection);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreEqual(input.IdenticalApertures, properties.IdenticalApertures);
            Assert.AreSame(input.LevelCrestStructureNotClosing, properties.LevelCrestStructureNotClosing.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreEqual(expectedProbabilityOpenStructureBeforeFlooding, properties.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(expectedFailureProbabilityOpenStructure, properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedFailureProbabilityReparation, properties.FailureProbabilityReparation);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                }
            };
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            var availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            CollectionAssert.AreEqual(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableStructures_SetInputContextInstanceWithStructures_ReturnStructures()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                ClosingStructures = 
                {
                    new TestClosingStructure()
                }
            };
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            var availableStructures = properties.GetAvailableStructures();

            // Assert
            CollectionAssert.AreEqual(failureMechanism.ClosingStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 7;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            input.Attach(observerMock);

            var random = new Random(100);
            double newDeviationWaveDirection = random.NextDouble();
            double newFactorStormDurationOpenStructure = random.NextDouble();
            var newInflowModelType = ClosingStructureInflowModelType.FloodedCulvert;
            var newIdenticalApertures = 2;

            // Call
            properties.DeviationWaveDirection = (RoundedDouble) newDeviationWaveDirection;
            properties.FactorStormDurationOpenStructure = (RoundedDouble) newFactorStormDurationOpenStructure;
            properties.InflowModelType = newInflowModelType;
            properties.FailureProbabilityOpenStructure = "1e-2";
            properties.FailureProbabilityReparation = "1e-3";
            properties.IdenticalApertures = newIdenticalApertures;
            properties.ProbabilityOpenStructureBeforeFlooding = "1e-4";

            // Assert
            var expectedProbabilityOpenStructureBeforeFlooding = ProbabilityFormattingHelper.Format(0.01);
            var expectedFailureProbabilityOpenStructure = ProbabilityFormattingHelper.Format(0.001);
            var expectedFailureProbabilityReparation = ProbabilityFormattingHelper.Format(0.0001);

            Assert.AreEqual(newDeviationWaveDirection, properties.DeviationWaveDirection, properties.DeviationWaveDirection.GetAccuracy());
            Assert.AreEqual(newFactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure.GetAccuracy());
            Assert.AreEqual(newInflowModelType, properties.InflowModelType);
            Assert.AreEqual(expectedProbabilityOpenStructureBeforeFlooding, properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedFailureProbabilityOpenStructure, properties.FailureProbabilityReparation);
            Assert.AreEqual(newIdenticalApertures, properties.IdenticalApertures);
            Assert.AreEqual(expectedFailureProbabilityReparation, properties.ProbabilityOpenStructureBeforeFlooding);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetProbabilityOpenStructureBeforeFlooding_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityOpenStructureBeforeFlooding = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetProbabilityOpenStructureBeforeFlooding_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityOpenStructureBeforeFlooding = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProbabilityOpenStructureBeforeFlooding_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityOpenStructureBeforeFlooding = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityOpenStructure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOpenStructure = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityOpenStructure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOpenStructure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityOpenStructure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityOpenStructure = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityReparation_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityReparation_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityReparation_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_VerticalWallStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            // Call
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(23, dynamicProperties.Count);

            PropertyDescriptor deviationWaveDirectionProperty = dynamicProperties[VerticalWall_deviationWaveDirectionPropertyIndex];
            Assert.IsFalse(deviationWaveDirectionProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, deviationWaveDirectionProperty.Category);
            Assert.AreEqual("Afwijking golfrichting [°]", deviationWaveDirectionProperty.DisplayName);
            Assert.AreEqual("Afwijking van de golfrichting.", deviationWaveDirectionProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[VerticalWall_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[VerticalWall_identicalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureNotClosingProperty = dynamicProperties[VerticalWall_levelCrestStructureNotClosingPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureNotClosingProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureNotClosingProperty.Category);
            Assert.AreEqual("Kruinhoogte niet gesloten kering [m+NAP]", levelCrestStructureNotClosingProperty.DisplayName);
            Assert.AreEqual("Niveau kruin bij niet gesloten maximaal kerende keermiddelen.", levelCrestStructureNotClosingProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[VerticalWall_probabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[VerticalWall_failureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[VerticalWall_failureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[VerticalWall_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[VerticalWall_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[VerticalWall_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[VerticalWall_structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[VerticalWall_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[VerticalWall_widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[VerticalWall_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[VerticalWall_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[VerticalWall_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[VerticalWall_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[VerticalWall_modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[VerticalWall_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[VerticalWall_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[VerticalWall_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[VerticalWall_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[VerticalWall_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_FloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.FloodedCulvert)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            // Call
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(21, dynamicProperties.Count);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[FloodedCulvert_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[FloodedCulvert_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[FloodedCulvert_areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[FloodedCulvert_identicalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[FloodedCulvert_probabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[FloodedCulvert_failureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[FloodedCulvert_failureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[FloodedCulvert_drainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficient [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficient.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[FloodedCulvert_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[FloodedCulvert_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[FloodedCulvert_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[FloodedCulvert_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[FloodedCulvert_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[FloodedCulvert_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[FloodedCulvert_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[FloodedCulvert_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[FloodedCulvert_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[FloodedCulvert_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.LowSill)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            // Call
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(22, dynamicProperties.Count);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[LowSill_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[LowSill_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[LowSill_identicalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[LowSill_thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[LowSill_probabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[LowSill_failureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[LowSill_failureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[LowSill_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[LowSill_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[LowSill_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[LowSill_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[LowSill_widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[LowSill_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[LowSill_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[LowSill_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[LowSill_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[LowSill_modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[LowSill_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[LowSill_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[LowSill_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[LowSill_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[LowSill_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(-10.0, -10.0),
                new Point2D(10.0, 10.0)
            }));

            // Call
            properties.Structure = new TestClosingStructure();

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsVerticalWall_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.VerticalWall)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DeviationWaveDirection)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsLowSill_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.LowSill)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.InsideWaterLevel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DeviationWaveDirection)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsFloodedCulvert_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(ClosingStructureInflowModelType.FloodedCulvert)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.InsideWaterLevel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DeviationWaveDirection)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.LevelCrestStructureNotClosing)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureTypeUnknown_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(0)
                }
            };
            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);
            var properties = new ClosingStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DeviationWaveDirection)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<ClosingStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        #region Property indices

        #region VerticalWall structures indices

        private const int VerticalWall_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int VerticalWall_stormDurationPropertyIndex = 1;
        private const int VerticalWall_deviationWaveDirectionPropertyIndex = 2;
        private const int VerticalWall_structurePropertyIndex = 3;
        private const int VerticalWall_structureLocationPropertyIndex = 4;
        private const int VerticalWall_structureNormalOrientationPropertyIndex = 5;
        private const int VerticalWall_inflowModelTypePropertyIndex = 6;
        private const int VerticalWall_widthFlowAperturesPropertyIndex = 7;
        private const int VerticalWall_identicalAperturesPropertyIndex = 8;
        private const int VerticalWall_flowWidthAtBottomProtectionPropertyIndex = 9;
        private const int VerticalWall_storageStructureAreaPropertyIndex = 10;
        private const int VerticalWall_allowedLevelIncreaseStoragePropertyIndex = 11;
        private const int VerticalWall_levelCrestStructureNotClosingPropertyIndex = 12;
        private const int VerticalWall_criticalOvertoppingDischargePropertyIndex = 13;
        private const int VerticalWall_probabilityOpenStructureBeforeFloodingPropertyIndex = 14;
        private const int VerticalWall_failureProbabilityOpenStructurePropertyIndex = 15;
        private const int VerticalWall_failureProbabilityReparationPropertyIndex = 16;
        private const int VerticalWall_failureProbabilityStructureWithErosionPropertyIndex = 17;
        private const int VerticalWall_foreshoreProfilePropertyIndex = 18;
        private const int VerticalWall_useBreakWaterPropertyIndex = 19;
        private const int VerticalWall_useForeshorePropertyIndex = 20;
        private const int VerticalWall_modelFactorSuperCriticalFlowPropertyIndex = 21;
        private const int VerticalWall_factorStormDurationOpenStructurePropertyIndex = 22;

        #endregion

        #region LowSill structures indices

        private const int LowSill_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int LowSill_stormDurationPropertyIndex = 1;
        private const int LowSill_insideWaterLevelPropertyIndex = 2;
        private const int LowSill_structurePropertyIndex = 3;
        private const int LowSill_structureLocationPropertyIndex = 4;
        private const int LowSill_inflowModelTypePropertyIndex = 5;
        private const int LowSill_widthFlowAperturesPropertyIndex = 6;
        private const int LowSill_identicalAperturesPropertyIndex = 7;
        private const int LowSill_flowWidthAtBottomProtectionPropertyIndex = 8;
        private const int LowSill_storageStructureAreaPropertyIndex = 9;
        private const int LowSill_allowedLevelIncreaseStoragePropertyIndex = 10;
        private const int LowSill_thresholdHeightOpenWeirPropertyIndex = 11;
        private const int LowSill_criticalOvertoppingDischargePropertyIndex = 12;
        private const int LowSill_probabilityOpenStructureBeforeFloodingPropertyIndex = 13;
        private const int LowSill_failureProbabilityOpenStructurePropertyIndex = 14;
        private const int LowSill_failureProbabilityReparationPropertyIndex = 15;
        private const int LowSill_failureProbabilityStructureWithErosionPropertyIndex = 16;
        private const int LowSill_foreshoreProfilePropertyIndex = 17;
        private const int LowSill_useBreakWaterPropertyIndex = 18;
        private const int LowSill_useForeshorePropertyIndex = 19;
        private const int LowSill_modelFactorSuperCriticalFlowPropertyIndex = 20;
        private const int LowSill_factorStormDurationOpenStructurePropertyIndex = 21;

        #endregion

        #region FloodedCulvert structures indices

        private const int FloodedCulvert_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int FloodedCulvert_stormDurationPropertyIndex = 1;
        private const int FloodedCulvert_insideWaterLevelPropertyIndex = 2;
        private const int FloodedCulvert_structurePropertyIndex = 3;
        private const int FloodedCulvert_structureLocationPropertyIndex = 4;
        private const int FloodedCulvert_inflowModelTypePropertyIndex = 5;
        private const int FloodedCulvert_areaFlowAperturesPropertyIndex = 6;
        private const int FloodedCulvert_identicalAperturesPropertyIndex = 7;
        private const int FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex = 8;
        private const int FloodedCulvert_storageStructureAreaPropertyIndex = 9;
        private const int FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex = 10;
        private const int FloodedCulvert_criticalOvertoppingDischargePropertyIndex = 11;
        private const int FloodedCulvert_probabilityOpenStructureBeforeFloodingPropertyIndex = 12;
        private const int FloodedCulvert_failureProbabilityOpenStructurePropertyIndex = 13;
        private const int FloodedCulvert_failureProbabilityReparationPropertyIndex = 14;
        private const int FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex = 15;
        private const int FloodedCulvert_foreshoreProfilePropertyIndex = 16;
        private const int FloodedCulvert_useBreakWaterPropertyIndex = 17;
        private const int FloodedCulvert_useForeshorePropertyIndex = 18;
        private const int FloodedCulvert_drainCoefficientPropertyIndex = 19;
        private const int FloodedCulvert_factorStormDurationOpenStructurePropertyIndex = 20;

        #endregion

        #endregion
    }
}