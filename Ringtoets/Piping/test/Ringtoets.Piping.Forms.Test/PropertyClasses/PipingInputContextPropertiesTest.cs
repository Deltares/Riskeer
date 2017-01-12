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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingInputContextPropertiesTest
    {
        private const int expectedSelectedHydraulicBoundaryLocationPropertyIndex = 0;
        private const int expectedAssessmentLevelPropertyIndex = 1;
        private const int expectedUseHydraulicBoundaryLocationPropertyIndex = 2;
        private const int expectedDampingFactorExitPropertyIndex = 3;
        private const int expectedPhreaticLevelExitPropertyIndex = 4;
        private const int expectedPiezometricHeadExitPropertyIndex = 5;
        private const int expectedSurfaceLinePropertyIndex = 6;
        private const int expectedStochasticSoilModelPropertyIndex = 7;
        private const int expectedStochasticSoilProfilePropertyIndex = 8;
        private const int expectedEntryPointLPropertyIndex = 9;
        private const int expectedExitPointLPropertyIndex = 10;
        private const int expectedSeepageLengthPropertyIndex = 11;
        private const int expectedThicknessCoverageLayerPropertyIndex = 12;
        private const int expectedEffectiveThicknessCoverageLayerPropertyIndex = 13;
        private const int expectedThicknessAquiferLayerPropertyIndex = 14;
        private const int expectedDarcyPermeabilityPropertyIndex = 15;
        private const int expectedDiameter70PropertyIndex = 16;
        private const int expectedSaturatedVolumicWeightOfCoverageLayerPropertyIndex = 17;

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            PipingInputContextProperties properties = new PipingInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                UseAssessmentLevelManualInput = false
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(18, dynamicProperties.Count);

            var hydraulicDataCategory = "Hydraulische gegevens";
            var schematizationCategory = "Schematisatie";

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[expectedSelectedHydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(hydraulicBoundaryLocationProperty);
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden waarvan het berekende toetspeil wordt gebruikt.", hydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
            Assert.IsNotNull(assessmentLevelProperty);
            Assert.IsTrue(assessmentLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, assessmentLevelProperty.Category);
            Assert.AreEqual("Toetspeil [m+NAP]", assessmentLevelProperty.DisplayName);
            Assert.AreEqual("Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.", assessmentLevelProperty.Description);

            PropertyDescriptor useHydraulicBoundaryLocationProperty = dynamicProperties[expectedUseHydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(useHydraulicBoundaryLocationProperty);
            Assert.AreEqual(hydraulicDataCategory, useHydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Handmatig toetspeil invoeren", useHydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("Sta toe om het toetspeil handmatig te specificeren?", useHydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor dampingsFactorExitProperty = dynamicProperties[expectedDampingFactorExitPropertyIndex];
            Assert.IsNotNull(dampingsFactorExitProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(dampingsFactorExitProperty.Converter);
            Assert.IsFalse(dampingsFactorExitProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, dampingsFactorExitProperty.Category);
            Assert.AreEqual("Dempingsfactor bij uittredepunt [-]", dampingsFactorExitProperty.DisplayName);
            Assert.AreEqual("Dempingsfactor relateert respons van stijghoogte bij binnenteen aan buitenwaterstand.", dampingsFactorExitProperty.Description);

            PropertyDescriptor phreaticLevelExitProperty = dynamicProperties[expectedPhreaticLevelExitPropertyIndex];
            Assert.IsNotNull(phreaticLevelExitProperty);
            Assert.IsInstanceOf<NormalDistributionDesignVariableTypeConverter>(phreaticLevelExitProperty.Converter);
            Assert.IsFalse(phreaticLevelExitProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, phreaticLevelExitProperty.Category);
            Assert.AreEqual("Polderpeil [m+NAP]", phreaticLevelExitProperty.DisplayName);
            Assert.AreEqual("Polderpeil.", phreaticLevelExitProperty.Description);

            PropertyDescriptor piezometricHeadExitProperty = dynamicProperties[expectedPiezometricHeadExitPropertyIndex];
            Assert.IsNotNull(piezometricHeadExitProperty);
            Assert.IsTrue(piezometricHeadExitProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, piezometricHeadExitProperty.Category);
            Assert.AreEqual("Stijghoogte bij uittredepunt [m+NAP]", piezometricHeadExitProperty.DisplayName);
            Assert.AreEqual("Stijghoogte bij uittredepunt.", piezometricHeadExitProperty.Description);

            PropertyDescriptor surfaceLineProperty = dynamicProperties[expectedSurfaceLinePropertyIndex];
            Assert.IsNotNull(surfaceLineProperty);
            Assert.IsFalse(surfaceLineProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, surfaceLineProperty.Category);
            Assert.AreEqual("Profielschematisatie", surfaceLineProperty.DisplayName);
            Assert.AreEqual("De schematisatie van de hoogte van het dwarsprofiel.", surfaceLineProperty.Description);

            PropertyDescriptor stochasticSoilModelProperty = dynamicProperties[expectedStochasticSoilModelPropertyIndex];
            Assert.IsNotNull(stochasticSoilModelProperty);
            Assert.IsFalse(stochasticSoilModelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, stochasticSoilModelProperty.Category);
            Assert.AreEqual("Stochastisch ondergrondmodel", stochasticSoilModelProperty.DisplayName);
            Assert.AreEqual("De verschillende opbouwen van de ondergrond en hun respectieve kansen van voorkomen.", stochasticSoilModelProperty.Description);

            PropertyDescriptor stochasticSoilProfileProperty = dynamicProperties[expectedStochasticSoilProfilePropertyIndex];
            Assert.IsNotNull(stochasticSoilProfileProperty);
            Assert.IsFalse(stochasticSoilProfileProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, stochasticSoilProfileProperty.Category);
            Assert.AreEqual("Ondergrondschematisatie", stochasticSoilProfileProperty.DisplayName);
            Assert.AreEqual("De opbouw van de ondergrond.", stochasticSoilProfileProperty.Description);

            PropertyDescriptor entryPointLProperty = dynamicProperties[expectedEntryPointLPropertyIndex];
            Assert.IsNotNull(entryPointLProperty);
            Assert.IsFalse(entryPointLProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, entryPointLProperty.Category);
            Assert.AreEqual("Intredepunt", entryPointLProperty.DisplayName);
            Assert.AreEqual("De positie in het dwarsprofiel van het intredepunt.", entryPointLProperty.Description);

            PropertyDescriptor exitPointLProperty = dynamicProperties[expectedExitPointLPropertyIndex];
            Assert.IsNotNull(exitPointLProperty);
            Assert.IsFalse(exitPointLProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, exitPointLProperty.Category);
            Assert.AreEqual("Uittredepunt", exitPointLProperty.DisplayName);
            Assert.AreEqual("De positie in het dwarsprofiel van het uittredepunt.", exitPointLProperty.Description);

            PropertyDescriptor seepageLengthProperty = dynamicProperties[expectedSeepageLengthPropertyIndex];
            Assert.IsNotNull(seepageLengthProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(seepageLengthProperty.Converter);
            Assert.IsTrue(seepageLengthProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, seepageLengthProperty.Category);
            Assert.AreEqual("Kwelweglengte [m]", seepageLengthProperty.DisplayName);
            Assert.AreEqual("De horizontale afstand tussen intrede- en uittredepunt die het kwelwater ondergronds aflegt voordat het weer aan de oppervlakte komt.", seepageLengthProperty.Description);

            PropertyDescriptor thicknessCoverageLayerProperty = dynamicProperties[expectedThicknessCoverageLayerPropertyIndex];
            Assert.IsNotNull(thicknessCoverageLayerProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(thicknessCoverageLayerProperty.Converter);
            Assert.IsTrue(thicknessCoverageLayerProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, thicknessCoverageLayerProperty.Category);
            Assert.AreEqual("Totale deklaagdikte bij uittredepunt [m]", thicknessCoverageLayerProperty.DisplayName);
            Assert.AreEqual("Totale deklaagdikte bij uittredepunt.", thicknessCoverageLayerProperty.Description);

            PropertyDescriptor effectiveThicknessCoverageLayerProperty = dynamicProperties[expectedEffectiveThicknessCoverageLayerPropertyIndex];
            Assert.IsNotNull(effectiveThicknessCoverageLayerProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(effectiveThicknessCoverageLayerProperty.Converter);
            Assert.IsTrue(effectiveThicknessCoverageLayerProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, effectiveThicknessCoverageLayerProperty.Category);
            Assert.AreEqual("Effectieve deklaagdikte bij uittredepunt [m]", effectiveThicknessCoverageLayerProperty.DisplayName);
            Assert.AreEqual("Effectieve deklaagdikte bij uittredepunt.", effectiveThicknessCoverageLayerProperty.Description);

            PropertyDescriptor thicknessAquiferLayerProperty = dynamicProperties[expectedThicknessAquiferLayerPropertyIndex];
            Assert.IsNotNull(thicknessAquiferLayerProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(thicknessAquiferLayerProperty.Converter);
            Assert.IsTrue(thicknessAquiferLayerProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, thicknessAquiferLayerProperty.Category);
            Assert.AreEqual("Dikte watervoerend pakket [m]", thicknessAquiferLayerProperty.DisplayName);
            Assert.AreEqual("De dikte van de bovenste voor doorlatendheid te onderscheiden zandlaag of combinatie van zandlagen.", thicknessAquiferLayerProperty.Description);

            PropertyDescriptor darcyPermeabilityProperty = dynamicProperties[expectedDarcyPermeabilityPropertyIndex];
            Assert.IsNotNull(darcyPermeabilityProperty);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(darcyPermeabilityProperty.Converter);
            Assert.IsTrue(darcyPermeabilityProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, darcyPermeabilityProperty.Category);
            Assert.AreEqual("Doorlatendheid aquifer [m/s]", darcyPermeabilityProperty.DisplayName);
            Assert.AreEqual("Darcy-snelheid waarmee water door de eerste voor doorlatendheid te onderscheiden zandlaag loopt.", darcyPermeabilityProperty.Description);

            PropertyDescriptor diameter70Property = dynamicProperties[expectedDiameter70PropertyIndex];
            Assert.IsNotNull(diameter70Property);
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableTypeConverter>(diameter70Property.Converter);
            Assert.IsTrue(diameter70Property.IsReadOnly);
            Assert.AreEqual(schematizationCategory, diameter70Property.Category);
            Assert.AreEqual("De d70 in de bovenste zandlaag [m]", diameter70Property.DisplayName);
            Assert.AreEqual("Zeefmaat waar 70 gewichtsprocent van de korrels uit een zandlaag doorheen gaat. Hier de korreldiameter van het bovenste gedeelte van de voor doorlatendheid te onderscheiden zandlaag, bepaald zonder fijne fractie (< 63µm).", diameter70Property.Description);

            PropertyDescriptor saturatedVolumicWeightOfCoverageLayerProperty = dynamicProperties[expectedSaturatedVolumicWeightOfCoverageLayerPropertyIndex];
            Assert.IsNotNull(saturatedVolumicWeightOfCoverageLayerProperty);
            Assert.IsInstanceOf<ShiftedLogNormalDistributionDesignVariableTypeConverter>(saturatedVolumicWeightOfCoverageLayerProperty.Converter);
            Assert.IsTrue(saturatedVolumicWeightOfCoverageLayerProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, saturatedVolumicWeightOfCoverageLayerProperty.Category);
            Assert.AreEqual("Verzadigd gewicht deklaag [kN/m³]", saturatedVolumicWeightOfCoverageLayerProperty.DisplayName);
            Assert.AreEqual("Verzadigd gewicht deklaag.", saturatedVolumicWeightOfCoverageLayerProperty.Description);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetProperties_UseAssessmentLevelManualInput_ReturnsExpectedAttributeValues(bool useManualAssessmentLevelInput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                UseAssessmentLevelManualInput = useManualAssessmentLevelInput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            var hydraulicDataCategory = "Hydraulische gegevens";
            if (!useManualAssessmentLevelInput)
            {
                Assert.AreEqual(18, dynamicProperties.Count);

                PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[expectedSelectedHydraulicBoundaryLocationPropertyIndex];
                Assert.IsNotNull(hydraulicBoundaryLocationProperty);
                Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
                Assert.AreEqual(hydraulicDataCategory, hydraulicBoundaryLocationProperty.Category);
                Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
                Assert.AreEqual("De locatie met hydraulische randvoorwaarden waarvan het berekende toetspeil wordt gebruikt.", hydraulicBoundaryLocationProperty.Description);

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
                Assert.IsNotNull(assessmentLevelProperty);
                Assert.IsTrue(assessmentLevelProperty.IsReadOnly);
                Assert.AreEqual(hydraulicDataCategory, assessmentLevelProperty.Category);
                Assert.AreEqual("Toetspeil [m+NAP]", assessmentLevelProperty.DisplayName);
                Assert.AreEqual("Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.", assessmentLevelProperty.Description);
            }
            else
            {
                Assert.AreEqual(17, dynamicProperties.Count);

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[0];
                Assert.IsNotNull(assessmentLevelProperty);
                Assert.IsFalse(assessmentLevelProperty.IsReadOnly);
                Assert.AreEqual(hydraulicDataCategory, assessmentLevelProperty.Category);
                Assert.AreEqual("Toetspeil [m+NAP]", assessmentLevelProperty.DisplayName);
                Assert.AreEqual("Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.", assessmentLevelProperty.Description);
            }
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(22);

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            StochasticSoilProfile stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, random.NextDouble(), new[]
                {
                    new PipingSoilLayer(random.NextDouble())
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            HydraulicBoundaryLocation testHydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(0.0);

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation,
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = (stochasticSoilProfile),
                UseAssessmentLevelManualInput = false
            };

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call & Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExit.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExit.Distribution);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.Mean, properties.ThicknessCoverageLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.StandardDeviation, properties.ThicknessCoverageLayer.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.EffectiveThicknessCoverageLayer.Mean, properties.EffectiveThicknessCoverageLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.EffectiveThicknessCoverageLayer.StandardDeviation, properties.EffectiveThicknessCoverageLayer.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.Diameter70.Mean, properties.Diameter70.Distribution.Mean);
            Assert.AreEqual(inputParameters.Diameter70.StandardDeviation, properties.Diameter70.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.DarcyPermeability.Mean, properties.DarcyPermeability.Distribution.Mean);
            Assert.AreEqual(inputParameters.DarcyPermeability.StandardDeviation, properties.DarcyPermeability.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.Mean, properties.ThicknessAquiferLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.StandardDeviation, properties.ThicknessAquiferLayer.Distribution.StandardDeviation);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean,
                properties.SaturatedVolumicWeightOfCoverageLayer.Distribution.Mean);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                properties.SaturatedVolumicWeightOfCoverageLayer.Distribution.StandardDeviation);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift,
                properties.SaturatedVolumicWeightOfCoverageLayer.Distribution.Shift);

            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExit);

            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.SeepageLength.Distribution.Mean);
            Assert.AreEqual(inputParameters.SeepageLength.StandardDeviation, properties.SeepageLength.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.ExitPointL - properties.EntryPointL);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(stochasticSoilProfile, properties.StochasticSoilProfile);
            Assert.AreSame(stochasticSoilModel, properties.StochasticSoilModel);
            Assert.AreSame(testHydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);

            Assert.AreEqual(inputParameters.UseAssessmentLevelManualInput, properties.UseAssessmentLevelManualInput);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_PropertiesOnInputSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            const double assessmentLevel = 0.36;
            const double entryPointL = 0.12;
            const double exitPointL = 0.44;
            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            StochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            StochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            var dampingFactorExit = new LogNormalDistributionDesignVariable(
                new LogNormalDistribution(3)
                {
                    Mean = (RoundedDouble) 1.55,
                    StandardDeviation = (RoundedDouble) 0.22
                });
            var phreaticLevelExit = new NormalDistributionDesignVariable(
                new NormalDistribution(3)
                {
                    Mean = (RoundedDouble) 1.55,
                    StandardDeviation = (RoundedDouble) 0.22
                });

            // Call
            properties.UseAssessmentLevelManualInput = true;
            properties.AssessmentLevel = (RoundedDouble) assessmentLevel;
            properties.SurfaceLine = surfaceLine;
            properties.EntryPointL = (RoundedDouble) entryPointL;
            properties.ExitPointL = (RoundedDouble) exitPointL;
            properties.StochasticSoilModel = soilModel;
            properties.StochasticSoilProfile = soilProfile;
            properties.DampingFactorExit = dampingFactorExit;
            properties.PhreaticLevelExit = phreaticLevelExit;

            // Assert
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel.Value);
            Assert.AreEqual(entryPointL, inputParameters.EntryPointL.Value);
            Assert.AreEqual(exitPointL, inputParameters.ExitPointL.Value);
            Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
            Assert.AreSame(soilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(soilProfile, inputParameters.StochasticSoilProfile);
            DistributionAssert.AreEqual(dampingFactorExit.Distribution, inputParameters.DampingFactorExit);
            DistributionAssert.AreEqual(phreaticLevelExit.Distribution, inputParameters.PhreaticLevelExit);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SurfaceLine_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.SurfaceLine = ValidSurfaceLine(0.0, 4.0));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void StochasticSoilModel_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {

            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.StochasticSoilModel = ValidStochasticSoilModel(0.0, 4.0));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void StochasticSoilProfile_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.StochasticSoilProfile = ValidStochasticSoilModel(0.0, 4.0).StochasticSoilProfiles.First());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLevel_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                hasOutput,
                properties => properties.AssessmentLevel = new Random(21).NextRoundedDouble(),
                new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = true
                    }
                });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DampingFactorExit_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput, 
                properties => properties.DampingFactorExit = new LogNormalDistributionDesignVariable(new LogNormalDistribution(3)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void PhreaticLevelExit_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput, 
                properties => properties.PhreaticLevelExit = new NormalDistributionDesignVariable(new NormalDistribution(3)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void EntryPoinL_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.EntryPointL = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ExitPointL_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.ExitPointL = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseCustomAssessmentLevel_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.UseAssessmentLevelManualInput = true);
        }

        [Test]
        [TestCase(0, 3, 3)]
        [TestCase(2, 4, 2)]
        [TestCase(1e-2, 4, 4 - 1e-2)]
        [TestCase(1e-2, 3, 3 - 1e-2)]
        [TestCase(1, 1 + 1e-2, 1e-2)]
        public void SeepageLength_ExitPointAndEntryPointSet_ExpectedValue(double entryPoint, double exitPoint, double seepageLength)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberOfChangedProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            inputParameters.Attach(inputObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                ExitPointL = (RoundedDouble) exitPoint,
                EntryPointL = (RoundedDouble) entryPoint
            };

            // Call & Assert
            Assert.AreEqual(seepageLength, properties.SeepageLength.Distribution.Mean, 1e-6);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.VerifyAll();
        }

        [Test]
        public void SeepageLength_EntryPointAndThenExitPointSet_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberOfChangedProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            inputParameters.Attach(inputObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                EntryPointL = (RoundedDouble) 0.5,
                ExitPointL = (RoundedDouble) 2
            };

            // Call & Assert
            Assert.AreEqual(1.5, properties.SeepageLength.Distribution.Mean.Value);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(-5.0)]
        public void ExitPointL_InvalidValue_ThrowsArgumentOutOfRangeException(double newExitPoint)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                EntryPointL = (RoundedDouble) 2.0
            };

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.ExitPointL = (RoundedDouble) newExitPoint;

            // Assert
            var expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(5.0)]
        public void EntryPointL_InvalidValue_ThrowsArgumentOutOfRangeException(double newEntryPoint)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                ExitPointL = (RoundedDouble) 2.0
            };

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.EntryPointL = (RoundedDouble) newEntryPoint;

            // Assert
            var expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void EntryPointL_NotOnSurfaceline_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                ExitPointL = (RoundedDouble) 2.0
            };

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.EntryPointL = (RoundedDouble) (-15.0);

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 4]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void ExitPointL_NotOnSurfaceline_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                EntryPointL = (RoundedDouble) 2.0
            };

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.ExitPointL = (RoundedDouble) 10.0;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 4]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelIsNaN_AssessmentLevelSetToNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            RoundedDouble assessmentLevel = (RoundedDouble) new Random(21).NextDouble();
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                    assessmentLevel)
            };
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            // Call
            properties.SelectedHydraulicBoundaryLocation = selectableHydraulicBoundaryLocation;

            // Assert
            Assert.IsNaN(properties.AssessmentLevel.Value);

            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelSet_SetsAssessmentLevelToDesignWaterLevelAndNotifiesOnce()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            RoundedDouble testLevel = (RoundedDouble) new Random(21).NextDouble();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                testLevel);
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            // Call
            properties.SelectedHydraulicBoundaryLocation = selectableHydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(testLevel, properties.AssessmentLevel, properties.AssessmentLevel.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void GivenHydraulicBoundaryLocationAndUseHydraulicBoundaryLocation_WhenUnuseLocationAndSetNewAssessmentLevel_UpdateAssessmentLevelAndRemovesLocation()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            var random = new Random(21);
            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(50)
            };

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
                UseAssessmentLevelManualInput = false
            };

            RoundedDouble testLevel = (RoundedDouble) random.NextDouble();

            // When
            properties.UseAssessmentLevelManualInput = true;
            properties.AssessmentLevel = testLevel;

            // Then
            Assert.AreEqual(2, properties.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(testLevel, properties.AssessmentLevel, properties.AssessmentLevel.GetAccuracy());
            Assert.IsNull(properties.SelectedHydraulicBoundaryLocation);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        [TestCase(1234)]
        public void AssessmentLevel_SetNewValue_UpdateDataAndNotifyObservers(double testLevel)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true
            };
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call
            properties.AssessmentLevel = (RoundedDouble) testLevel;

            // Assert
            Assert.AreEqual(2, properties.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(testLevel, properties.AssessmentLevel, properties.AssessmentLevel.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentLevelSetWithoutHydraulicBoundaryLocation_WhenUseAndSetNewLocation_UpdateAssessmentLevelWithLocationValues()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            var random = new Random(21);
            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true,
                AssessmentLevel = (RoundedDouble) random.NextDouble()
            };

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection),
            };

            RoundedDouble testLevel = (RoundedDouble) random.NextDouble();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                testLevel);
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            // When
            properties.UseAssessmentLevelManualInput = false;
            properties.SelectedHydraulicBoundaryLocation = selectableHydraulicBoundaryLocation;

            // Then
            Assert.AreEqual(2, properties.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreSame(hydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(testLevel, properties.AssessmentLevel, properties.AssessmentLevel.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_NewSurfaceLine_StochasticSoilModelAndSoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0)
                }
            };
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };
            inputParameters.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_SameSurfaceLine_SoilProfileUnchanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            StochasticSoilProfile stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              new[]
                                              {
                                                  stochasticSoilModel
                                              },
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call
            properties.SurfaceLine = testSurfaceLine;

            // Assert
            Assert.AreSame(stochasticSoilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(stochasticSoilProfile, inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_DifferentSurfaceLine_StochasticSoilModelAndSoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            StochasticSoilProfile testPipingSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(testPipingSoilProfile);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0, 4),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = testPipingSoilProfile
                }
            };

            PipingInput inputParameters = calculationItem.InputParameters;
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              new[]
                                              {
                                                  stochasticSoilModel
                                              },
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void StochasticSoilProfile_DifferentStochasticSoilModel_SoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            StochasticSoilProfile stochasticSoilProfile1 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel1 = new StochasticSoilModel(0, "StochasticSoilModel1Name", "StochasticSoilModelSegment1Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile1);

            StochasticSoilProfile stochasticSoilProfile2 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel2 = new StochasticSoilModel(0, "StochasticSoilModel2Name", "StochasticSoilModelSegment2Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel1,
                StochasticSoilProfile = stochasticSoilProfile1
            };
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call
            properties.StochasticSoilModel = stochasticSoilModel2;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenCompletePipingInputContextProperties_WhenPhreaticLevelExitPropertiesSetThroughProperties_ThenPiezometricHeadExitUpdated(int propertyIndexToChange)
        {
            // Given
            var mocks = new MockRepository();
            var typeDescriptorContext = mocks.StrictMock<ITypeDescriptorContext>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            PipingInputContextProperties contextProperties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };
            inputParameters.HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.0);

            DesignVariable<NormalDistribution> phreaticLevelExitProperty = contextProperties.PhreaticLevelExit;
            DynamicPropertyBag dynamicPropertyBag = new DynamicPropertyBag(contextProperties);
            typeDescriptorContext.Expect(tdc => tdc.Instance).Return(dynamicPropertyBag).Repeat.Twice();
            typeDescriptorContext.Stub(tdc => tdc.PropertyDescriptor).Return(dynamicPropertyBag.GetProperties()["PhreaticLevelExit"]);
            mocks.ReplayAll();

            PropertyDescriptorCollection properties = new NormalDistributionDesignVariableTypeConverter().GetProperties(typeDescriptorContext, phreaticLevelExitProperty);
            Assert.NotNull(properties);

            // When
            properties[propertyIndexToChange].SetValue(phreaticLevelExitProperty, (RoundedDouble) 2.3);

            // Then
            Assert.IsFalse(double.IsNaN(inputParameters.PiezometricHeadExit));
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableSurfaceLines_Always_ReturnAllRingtoetsPipingSurfaceLines()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = properties.GetAvailableSurfaceLines();

            // Assert
            Assert.AreSame(context.AvailablePipingSurfaceLines, surfaceLines);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableStochasticSoilModels_NoSurfaceLineAssigned_ReturnAllStochasticSoilModels()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Precondition:
            Assert.IsNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<StochasticSoilModel> soilModels = properties.GetAvailableStochasticSoilModels();

            // Assert
            Assert.AreSame(context.AvailableStochasticSoilModels, soilModels);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableStochasticSoilModels_SurfaceLineAssigned_ReturnMatchingSubsetOfStochasticSoilModels()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            var failureMechanism = new PipingFailureMechanism
            {
                StochasticSoilModels =
                {
                    new StochasticSoilModel(1, "A", "B")
                    {
                        Geometry =
                        {
                            new Point2D(2, -1),
                            new Point2D(2, 1)
                        },
                        StochasticSoilProfiles =
                        {
                            new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1)
                        }
                    },
                    new StochasticSoilModel(2, "C", "D")
                    {
                        Geometry =
                        {
                            new Point2D(-2, -1),
                            new Point2D(-2, 1)
                        },
                        StochasticSoilProfiles =
                        {
                            new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 2)
                        }
                    },
                    new StochasticSoilModel(3, "E", "F")
                    {
                        Geometry =
                        {
                            new Point2D(6, -1),
                            new Point2D(6, 1)
                        },
                        StochasticSoilProfiles =
                        {
                            new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 3)
                        }
                    }
                }
            };
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Precondition:
            Assert.IsNotNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<StochasticSoilModel> soilModels = properties.GetAvailableStochasticSoilModels();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.StochasticSoilModels[0],
                failureMechanism.StochasticSoilModels[2]
            }, soilModels);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableStochasticSoilProfiles_NoStochasticSoilModel_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Precondition
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<StochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

            // Assert
            CollectionAssert.IsEmpty(profiles);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableStochasticSoilProfiles_StochasticSoilModel_ReturnAssignedSoilModelProfiles()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var model = new StochasticSoilModel(1, "A", "B")
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                }
            };
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    StochasticSoilModel = model
                }
            };
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Precondition
            Assert.IsNotNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<StochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

            // Assert
            CollectionAssert.AreEqual(model.StochasticSoilProfiles, profiles);
            mocks.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithSurfaceLineAndLocations_WhenSelectingLocation_ThenSelectedLocationDistanceSameAsLocationItem()
        {
            // Given
            var mockRepository = new MockRepository();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            mockRepository.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                     failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                     failureMechanism, assessmentSection);

            StochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            StochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            var properties = new PipingInputContextProperties
            {
                Data = context,
                SurfaceLine = ValidSurfaceLine(0, 4.0),
                StochasticSoilModel = soilModel,
                StochasticSoilProfile = soilProfile
            };

            // When
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Then
            var hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            Assert.AreEqual(selectedLocation.Distance, hydraulicBoundaryLocationItem.Distance,
                            hydraulicBoundaryLocationItem.Distance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_WithLocationsNoSurfaceLine_ReturnLocationsSortedById()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 1),
                    new HydraulicBoundaryLocation(4, "C", 0, 2),
                    new HydraulicBoundaryLocation(3, "D", 0, 3),
                    new HydraulicBoundaryLocation(2, "B", 0, 4)
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties
            {
                Data = context
            };

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null))
                                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, selectableHydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_WithLocationsAndSurfaceLine_ReturnLocationsSortedByDistanceThenById()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);
            var properties = new PipingInputContextProperties
            {
                Data = context,
                SurfaceLine = surfaceLine
            };

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations.Select(hbl => new SelectableHydraulicBoundaryLocation(
                                                                      hbl, surfaceLine.ReferenceLineIntersectionWorldPoint))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, selectableHydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingSurfaceLine_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);
            var properties = new PipingInputContextProperties
            {
                Data = context,
                SurfaceLine = surfaceLine
            };

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

            var newSurfaceLine = ValidSurfaceLine(0.0, 5.0);
            newSurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 190);

            // When
            properties.SurfaceLine = newSurfaceLine;

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryDatabase.Locations
                                         .Select(hbl =>
                                                 new SelectableHydraulicBoundaryLocation(hbl,
                                                                                         properties.SurfaceLine.ReferenceLineIntersectionWorldPoint))
                                         .OrderBy(hbl => hbl.Distance)
                                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnlyValidationMethod_AssessmentLevel_DependsOnUseCustomAssessmentLevel(bool useCustomAssessmentLevel)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useCustomAssessmentLevel
                }
            };

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties
            {
                Data = context
            };


            // Call
            var result = properties.DynamicReadOnlyValidationMethod("AssessmentLevel");

            // Assert
            Assert.AreNotEqual(useCustomAssessmentLevel, result);
        }

        [Test]
        public void DynamicReadOnlyValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            // Setup
            var properties = new PipingInputContextProperties();

            // Call
            var result = properties.DynamicReadOnlyValidationMethod("prop");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_SelectedHydraulicBoundaryLocation_DependsOnUseCustomAssessmentLevel(bool useCustomAssessmentLevel)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useCustomAssessmentLevel
                }
            };

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties
            {
                Data = context
            };


            // Call
            var result = properties.DynamicVisibleValidationMethod("SelectedHydraulicBoundaryLocation");

            // Assert
            Assert.AreNotEqual(useCustomAssessmentLevel, result);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsFalse()
        {
            // Setup
            var properties = new PipingInputContextProperties();

            // Call
            var result = properties.DynamicVisibleValidationMethod("prop");

            // Assert
            Assert.IsFalse(result);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(bool hasOutput, Action<PipingInputContextProperties> setProperty)
        {
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                hasOutput,
                setProperty,
                new PipingCalculationScenario(new GeneralPipingInput()));
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
            bool hasOutput,
            Action<PipingInputContextProperties> setProperty, 
            PipingCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculationObserver = mocks.StrictMock<IObserver>();
            int numberOfChangedProperties = hasOutput ? 1 : 0;
            calculationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            if (hasOutput)
            {
                calculation.Output = new TestPipingOutput();
            }
            calculation.Attach(calculationObserver);

            PipingInput inputParameters = calculation.InputParameters;
            inputParameters.Attach(inputObserver);

            var failureMechanism = new PipingFailureMechanism();

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculation,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSection)
            };

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mocks.VerifyAll();
        }

        private static StochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1234)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            stochasticSoilModel.Geometry.Add(new Point2D(xMin, 1.0));
            stochasticSoilModel.Geometry.Add(new Point2D(xMax, 0.0));
            return stochasticSoilModel;
        }

        private static RingtoetsPipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}