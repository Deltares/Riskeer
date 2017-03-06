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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresInputContextPropertiesTest
    {
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
            TestDelegate test = () => new ClosingStructuresInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Setup
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
                                                                 assessmentSection);

            // Call
            TestDelegate test = () => new ClosingStructuresInputContextProperties(inputContext, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithDataAndHandler_ExpectedValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput,
                StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>>(properties);
            Assert.AreSame(inputContext, properties.Data);

            ClosingStructuresInput input = calculation.InputParameters;
            string expectedProbabilityOrFrequencyOpenStructureBeforeFlooding = ProbabilityFormattingHelper.Format(input.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            string expectedFailureProbabilityOpenStructure = ProbabilityFormattingHelper.Format(input.FailureProbabilityOpenStructure);
            string expectedFailureProbabilityReparation = ProbabilityFormattingHelper.Format(input.FailureProbabilityReparation);

            Assert.AreSame(input.ModelFactorSuperCriticalFlow, properties.ModelFactorSuperCriticalFlow.Data);
            Assert.AreEqual(input.StructureNormalOrientation, properties.StructureNormalOrientation);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreEqual(input.IdenticalApertures, properties.IdenticalApertures);
            Assert.AreSame(input.LevelCrestStructureNotClosing, properties.LevelCrestStructureNotClosing.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreEqual(expectedProbabilityOrFrequencyOpenStructureBeforeFlooding, properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            Assert.AreEqual(expectedFailureProbabilityOpenStructure, properties.FailureProbabilityOpenStructure);
            Assert.AreEqual(expectedFailureProbabilityReparation, properties.FailureProbabilityReparation);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_VerticalWallStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
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

            PropertyDescriptor probabilityOrFrequencyOpenStructureBeforeFloodingProperty = dynamicProperties[verticalWallProbabilityOrFrequencyOpenStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOrFrequencyOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Description);

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
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
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

            PropertyDescriptor probabilityOrFrequencyOpenStructureBeforeFloodingProperty = dynamicProperties[floodedCulvertProbabilityOpenOrFrequencyStructureBeforeFloodingPropertyIndex];
            Assert.IsFalse(probabilityOrFrequencyOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Description);

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
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(22, dynamicProperties.Count);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[lowSillInsideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[5];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor identicalAperturesProperty = dynamicProperties[7];
            Assert.IsFalse(identicalAperturesProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, identicalAperturesProperty.Category);
            Assert.AreEqual("Aantal identieke doorstroomopeningen [-]", identicalAperturesProperty.DisplayName);
            Assert.AreEqual("Aantal identieke doorstroomopeningen.", identicalAperturesProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[lowSillThresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor probabilityOrFrequencyOpenStructureBeforeFloodingProperty = dynamicProperties[13];
            Assert.IsFalse(probabilityOrFrequencyOpenStructureBeforeFloodingProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Category);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater [1/jaar]", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.DisplayName);
            Assert.AreEqual("Kans op open staan bij naderend hoogwater.", probabilityOrFrequencyOpenStructureBeforeFloodingProperty.Description);

            PropertyDescriptor failureProbabilityOpenStructureProperty = dynamicProperties[14];
            Assert.IsFalse(failureProbabilityOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityOpenStructureProperty.Category);
            Assert.AreEqual("Kans mislukken sluiting [1/jaar]", failureProbabilityOpenStructureProperty.DisplayName);
            Assert.AreEqual("Kans op mislukken sluiting van geopend kunstwerk.", failureProbabilityOpenStructureProperty.Description);

            PropertyDescriptor failureProbabilityReparationProperty = dynamicProperties[15];
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
        public void Constructor_WithoutStructure_CorrectReadOnlyForStructureDependentProperties()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var inputContext = new ClosingStructuresInputContext(calculation.InputParameters,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            // Call
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            PropertyDescriptor inflowModelType = dynamicProperties[6];
            Assert.IsTrue(inflowModelType.IsReadOnly);

            PropertyDescriptor identicalApertures = dynamicProperties[9];
            Assert.IsTrue(identicalApertures.IsReadOnly);

            PropertyDescriptor probabilityOrFrequencyOpenStructureBeforeFlooding = dynamicProperties[16];
            Assert.IsTrue(probabilityOrFrequencyOpenStructureBeforeFlooding.IsReadOnly);

            PropertyDescriptor failureProbabilityOpenStructure = dynamicProperties[17];
            Assert.IsTrue(failureProbabilityOpenStructure.IsReadOnly);

            PropertyDescriptor failureProbabilityReparation = dynamicProperties[18];
            Assert.IsTrue(failureProbabilityReparation.IsReadOnly);

            AssertPropertiesInState(properties.ThresholdHeightOpenWeir, true);
            AssertPropertiesInState(properties.AreaFlowApertures, true);
            AssertPropertiesInState(properties.LevelCrestStructureNotClosing, true);
            AssertPropertiesInState(properties.InsideWaterLevel, true);
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

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
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<ClosingStructure> availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.ClosingStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void FactorStormDurationOpenStructure_Always_InputChangedAndObsevablesNotified()
        {
            RoundedDouble factor = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.FactorStormDurationOpenStructure = factor);
        }

        [Test]
        public void InflowModelType_Always_InputChangedAndObsevablesNotified()
        {
            var inflowModelType = new Random(21).NextEnumValue<ClosingStructureInflowModelType>();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.InflowModelType = inflowModelType);
        }

        [Test]
        public void ProbabilityOrFrequencyOpenStructureBeforeFlooding_Always_InputChangedAndObsevablesNotified()
        {
            string probability = new Random(21).NextDouble().ToString(CultureInfo.CurrentCulture);
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = probability);
        }

        [Test]
        public void FailureProbabilityOpenStructure_Always_InputChangedAndObsevablesNotified()
        {
            string probability = new Random(21).NextDouble().ToString(CultureInfo.CurrentCulture);
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.FailureProbabilityOpenStructure = probability);
        }

        [Test]
        public void FailureProbabilityReparation_Always_InputChangedAndObsevablesNotified()
        {
            string probability = new Random(21).NextDouble().ToString(CultureInfo.CurrentCulture);
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.FailureProbabilityReparation = probability);
        }

        [Test]
        public void IdenticalApertures_Always_InputChangedAndObsevablesNotified()
        {
            int propertiesIdenticalApertures = new Random(21).Next();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.IdenticalApertures = propertiesIdenticalApertures);
        }

        [Test]
        public void InsideWaterLevel_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.InsideWaterLevel.Mean = newMean);
        }

        [Test]
        public void ModelFactorSuperCriticalFlow_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.ModelFactorSuperCriticalFlow.Mean = newMean);
        }

        [Test]
        public void WidthFlowApertures_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.WidthFlowApertures.Mean = newMean);
        }

        [Test]
        public void ThresholdHeightOpenWeir_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.ThresholdHeightOpenWeir.Mean = newMean);
        }

        [Test]
        public void AreaFlowApertures_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.AreaFlowApertures.Mean = newMean);
        }

        [Test]
        public void DrainCoefficient_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.DrainCoefficient.Mean = newMean);
        }

        [Test]
        public void LevelCrestStructureNotClosing_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble newMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.LevelCrestStructureNotClosing.Mean = newMean);
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void ProbabilityOrFrequencyOpenStructureBeforeFlooding_InvalidDoubleValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            const int overflow = 1;
            string newProbabilityString = string.Concat(newValue.ToString("r", CultureInfo.CurrentCulture), overflow);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void ProbabilityOrFrequencyOpenStructureBeforeFlooding_InvalidValues_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            string newProbabilityString = newValue.ToString("r", CultureInfo.CurrentCulture);
            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void ProbabilityOrFrequencyOpenStructureBeforeFlooding_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ProbabilityOrFrequencyOpenStructureBeforeFlooding_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void FailureProbabilityOpenStructure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            const int overflow = 1;
            string newProbabilityString = string.Concat(newValue.ToString("r", CultureInfo.CurrentCulture), overflow);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityOpenStructure = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void FailureProbabilityOpenStructure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityOpenStructure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureProbabilityOpenStructure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

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
        public void FailureProbabilityReparation_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            const int overflow = 1;
            string newProbabilityString = string.Concat(newValue.ToString("r", CultureInfo.CurrentCulture), overflow);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = newProbabilityString;

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void FailureProbabilityReparation_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureProbabilityReparation_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSectionStub);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call
            TestDelegate call = () => properties.FailureProbabilityReparation = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Structure_StructureInSection_UpdateSectionResults()
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

            var newStructure = new TestClosingStructure();
            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

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
        public void DynamicVisibleValidationMethod_StructureIsVerticalWall_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsLowSill_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        public void DynamicVisibleValidationMethod_StructureIsFloodedCulvert_ReturnExpectedVisibility()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

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
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var properties = new ClosingStructuresInputContextProperties(inputContext, handler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.InsideWaterLevel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.StructureNormalOrientation)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.ThresholdHeightOpenWeir)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.LevelCrestStructureNotClosing)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(ClosingStructuresInputContextProperties.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(Action<ClosingStructuresInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            ClosingStructuresInput input = calculation.InputParameters;
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Structure = new TestClosingStructure();

            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
                {
                    observable
                });

            var inputContext = new ClosingStructuresInputContext(input,
                                                                 calculation,
                                                                 failureMechanism,
                                                                 assessmentSection);
            var properties = new ClosingStructuresInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private static void AssertPropertiesInState(object properties, bool expectedReadOnly)
        {
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(expectedReadOnly, dynamicProperties[1].IsReadOnly);
            Assert.AreEqual(expectedReadOnly, dynamicProperties[2].IsReadOnly);
        }

        #region Property indices

//        #region No structure indices
//
//        private const int verticalWallHydraulicBoundaryLocationPropertyIndex = 0;
//        private const int verticalWallStormDurationPropertyIndex = 1;
//        private const int verticalWallStructurePropertyIndex = 2;
//        private const int verticalWallStructureLocationPropertyIndex = 3;
//        private const int verticalWallStructureNormalOrientationPropertyIndex = 4;
//        private const int verticalWallInflowModelTypePropertyIndex = 5;
//        private const int verticalWallWidthFlowAperturesPropertyIndex = 6;
//        private const int verticalWallIdenticalAperturesPropertyIndex = 7;
//        private const int verticalWallFlowWidthAtBottomProtectionPropertyIndex = 8;
//        private const int verticalWallStorageStructureAreaPropertyIndex = 9;
//        private const int verticalWallAllowedLevelIncreaseStoragePropertyIndex = 10;
//        private const int verticalWallLevelCrestStructureNotClosingPropertyIndex = 11;
//        private const int verticalWallCriticalOvertoppingDischargePropertyIndex = 12;
//        private const int verticalWallProbabilityOrFrequencyOpenStructureBeforeFloodingPropertyIndex = 13;
//        private const int verticalWallFailureProbabilityOpenStructurePropertyIndex = 14;
//        private const int verticalWallFailureProbabilityReparationPropertyIndex = 15;
//        private const int verticalWallFailureProbabilityStructureWithErosionPropertyIndex = 16;
//        private const int verticalWallForeshoreProfilePropertyIndex = 17;
//        private const int verticalWallUseBreakWaterPropertyIndex = 18;
//        private const int verticalWallUseForeshorePropertyIndex = 19;
//        private const int verticalWallModelFactorSuperCriticalFlowPropertyIndex = 20;
//        private const int verticalWallFactorStormDurationOpenStructurePropertyIndex = 21;
//
//        #endregion

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
        private const int verticalWallProbabilityOrFrequencyOpenStructureBeforeFloodingPropertyIndex = 13;
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
        private const int lowSillWidthFlowAperturesPropertyIndex = 6;
        private const int lowSillFlowWidthAtBottomProtectionPropertyIndex = 8;
        private const int lowSillStorageStructureAreaPropertyIndex = 9;
        private const int lowSillAllowedLevelIncreaseStoragePropertyIndex = 10;
        private const int lowSillThresholdHeightOpenWeirPropertyIndex = 11;
        private const int lowSillCriticalOvertoppingDischargePropertyIndex = 12;
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
        private const int floodedCulvertProbabilityOpenOrFrequencyStructureBeforeFloodingPropertyIndex = 12;
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