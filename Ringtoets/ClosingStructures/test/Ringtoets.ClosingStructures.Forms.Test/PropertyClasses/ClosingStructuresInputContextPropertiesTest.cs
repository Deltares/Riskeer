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
            Assert.IsInstanceOf<StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput,
                StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>>(properties);
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

            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
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
            Assert.AreSame(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
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
            Assert.AreSame(failureMechanism.ClosingStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 6;
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
            double newFactorStormDurationOpenStructure = random.NextDouble();
            var newInflowModelType = ClosingStructureInflowModelType.FloodedCulvert;
            var newIdenticalApertures = 2;

            // Call
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
            const string modelSettingsCategory = "Modelinstellingen";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(22, dynamicProperties.Count);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[verticalWallInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[verticalWallIdenticalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureNotClosingProperty = dynamicProperties[verticalWallLevelCrestStructureNotClosingPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureNotClosingProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureNotClosingProperty.Category);
            Assert.AreEqual("Kruinhoogte niet gesloten kering [m+NAP]", levelCrestStructureNotClosingProperty.DisplayName);
            Assert.AreEqual("Niveau kruin bij niet gesloten maximaal kerende keermiddelen.", levelCrestStructureNotClosingProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[verticalWallProbabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[verticalWallFailureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[verticalWallFailureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[verticalWallFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[verticalWallStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[verticalWallStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[verticalWallStructureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[verticalWallFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[verticalWallWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[verticalWallStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[verticalWallAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[verticalWallCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[verticalWallFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[verticalWallModelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[verticalWallForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[verticalWallUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[verticalWallUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[verticalWallHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[verticalWallStormDurationPropertyIndex].DisplayName);

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

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[floodedCulvertInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[floodedCulvertInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[floodedCulvertAreaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[floodedCulvertIdenticalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[floodedCulvertProbabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[floodedCulvertFailureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[floodedCulvertFailureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[floodedCulvertDrainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficiënt [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficiënt.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[floodedCulvertFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[floodedCulvertStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[floodedCulvertStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[floodedCulvertFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[floodedCulvertStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[floodedCulvertAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[floodedCulvertCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[floodedCulvertFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[floodedCulvertForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[floodedCulvertUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[floodedCulvertUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[floodedCulvertHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[floodedCulvertStormDurationPropertyIndex].DisplayName);

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

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[lowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[lowSillInflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[lowSillIdenticalAperturesPropertyIndex];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[lowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor probabilityOpenStructureBeforeFloodingProperty = dynamicProperties[lowSillProbabilityOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[lowSillFailureProbabilityOpenStructurePropertyIndex];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[lowSillFailureProbabilityReparationPropertyIndex];
            Assert.IsFalse(failureProbabilityReparationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityReparationProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityReparationProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityReparationProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[lowSillFactorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[lowSillStructurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[lowSillStructureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[lowSillFlowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[lowSillWidthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[lowSillStorageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[lowSillAllowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[lowSillCriticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[lowSillFailureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[lowSillModelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[lowSillForeshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[lowSillUseBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[lowSillUseForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[lowSillHydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[lowSillStormDurationPropertyIndex].DisplayName);

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
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
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

        private const int verticalWallHydraulicBoundaryLocationPropertyIndex = 0;
        private const int verticalWallStormDurationPropertyIndex = 1;
        private const int verticalWallStructurePropertyIndex = 2;
        private const int verticalWallStructureLocationPropertyIndex = 3;
        private const int verticalWallStructureNormalOrientationPropertyIndex = 4;
        private const int verticalWallInflowModelTypePropertyIndex = 5;
        private const int verticalWallWidthFlowAperturesPropertyIndex = 6;
        private const int verticalWallIdenticalAperturesPropertyIndex = 7;
        private const int verticalWallFlowWidthAtBottomProtectionPropertyIndex = 8;
        private const int verticalWallStorageStructureAreaPropertyIndex = 9;
        private const int verticalWallAllowedLevelIncreaseStoragePropertyIndex = 10;
        private const int verticalWallLevelCrestStructureNotClosingPropertyIndex = 11;
        private const int verticalWallCriticalOvertoppingDischargePropertyIndex = 12;
        private const int verticalWallProbabilityOpenStructureBeforeFloodingPropertyIndex = 13;
        private const int verticalWallFailureProbabilityOpenStructurePropertyIndex = 14;
        private const int verticalWallFailureProbabilityReparationPropertyIndex = 15;
        private const int verticalWallFailureProbabilityStructureWithErosionPropertyIndex = 16;
        private const int verticalWallForeshoreProfilePropertyIndex = 17;
        private const int verticalWallUseBreakWaterPropertyIndex = 18;
        private const int verticalWallUseForeshorePropertyIndex = 19;
        private const int verticalWallModelFactorSuperCriticalFlowPropertyIndex = 20;
        private const int verticalWallFactorStormDurationOpenStructurePropertyIndex = 21;

        #endregion

        #region LowSill structures indices

        private const int lowSillHydraulicBoundaryLocationPropertyIndex = 0;
        private const int lowSillStormDurationPropertyIndex = 1;
        private const int lowSillInsideWaterLevelPropertyIndex = 2;
        private const int lowSillStructurePropertyIndex = 3;
        private const int lowSillStructureLocationPropertyIndex = 4;
        private const int lowSillInflowModelTypePropertyIndex = 5;
        private const int lowSillWidthFlowAperturesPropertyIndex = 6;
        private const int lowSillIdenticalAperturesPropertyIndex = 7;
        private const int lowSillFlowWidthAtBottomProtectionPropertyIndex = 8;
        private const int lowSillStorageStructureAreaPropertyIndex = 9;
        private const int lowSillAllowedLevelIncreaseStoragePropertyIndex = 10;
        private const int lowSillThresholdHeightOpenWeirPropertyIndex = 11;
        private const int lowSillCriticalOvertoppingDischargePropertyIndex = 12;
        private const int lowSillProbabilityOpenStructureBeforeFloodingPropertyIndex = 13;
        private const int lowSillFailureProbabilityOpenStructurePropertyIndex = 14;
        private const int lowSillFailureProbabilityReparationPropertyIndex = 15;
        private const int lowSillFailureProbabilityStructureWithErosionPropertyIndex = 16;
        private const int lowSillForeshoreProfilePropertyIndex = 17;
        private const int lowSillUseBreakWaterPropertyIndex = 18;
        private const int lowSillUseForeshorePropertyIndex = 19;
        private const int lowSillModelFactorSuperCriticalFlowPropertyIndex = 20;
        private const int lowSillFactorStormDurationOpenStructurePropertyIndex = 21;

        #endregion

        #region FloodedCulvert structures indices

        private const int floodedCulvertHydraulicBoundaryLocationPropertyIndex = 0;
        private const int floodedCulvertStormDurationPropertyIndex = 1;
        private const int floodedCulvertInsideWaterLevelPropertyIndex = 2;
        private const int floodedCulvertStructurePropertyIndex = 3;
        private const int floodedCulvertStructureLocationPropertyIndex = 4;
        private const int floodedCulvertInflowModelTypePropertyIndex = 5;
        private const int floodedCulvertAreaFlowAperturesPropertyIndex = 6;
        private const int floodedCulvertIdenticalAperturesPropertyIndex = 7;
        private const int floodedCulvertFlowWidthAtBottomProtectionPropertyIndex = 8;
        private const int floodedCulvertStorageStructureAreaPropertyIndex = 9;
        private const int floodedCulvertAllowedLevelIncreaseStoragePropertyIndex = 10;
        private const int floodedCulvertCriticalOvertoppingDischargePropertyIndex = 11;
        private const int floodedCulvertProbabilityOpenStructureBeforeFloodingPropertyIndex = 12;
        private const int floodedCulvertFailureProbabilityOpenStructurePropertyIndex = 13;
        private const int floodedCulvertFailureProbabilityReparationPropertyIndex = 14;
        private const int floodedCulvertFailureProbabilityStructureWithErosionPropertyIndex = 15;
        private const int floodedCulvertForeshoreProfilePropertyIndex = 16;
        private const int floodedCulvertUseBreakWaterPropertyIndex = 17;
        private const int floodedCulvertUseForeshorePropertyIndex = 18;
        private const int floodedCulvertDrainCoefficientPropertyIndex = 19;
        private const int floodedCulvertFactorStormDurationOpenStructurePropertyIndex = 20;

        #endregion

        #endregion
    }
}