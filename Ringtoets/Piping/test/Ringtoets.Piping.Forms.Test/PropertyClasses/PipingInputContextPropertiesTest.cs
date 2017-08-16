// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
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
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingInputContextProperties(null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            TestDelegate test = () => new PipingInputContextProperties(context, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("handler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            var properties = new PipingInputContextProperties(context, handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.AreSame(context, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new PipingInputContextProperties(context, handler)
            {
                UseAssessmentLevelManualInput = false
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(18, dynamicProperties.Count);

            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string schematizationCategory = "Schematisatie";

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[expectedSelectedHydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                hydraulicBoundaryLocationProperty,
                hydraulicDataCategory,
                "Locatie met hydraulische randvoorwaarden",
                "De locatie met hydraulische randvoorwaarden waarvan het berekende toetspeil wordt gebruikt.");

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                assessmentLevelProperty,
                hydraulicDataCategory,
                "Toetspeil [m+NAP]",
                "Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.",
                true);

            PropertyDescriptor useHydraulicBoundaryLocationProperty = dynamicProperties[expectedUseHydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                useHydraulicBoundaryLocationProperty,
                hydraulicDataCategory,
                "Handmatig toetspeil invoeren",
                "Sta toe om het toetspeil handmatig te specificeren?");

            PropertyDescriptor dampingsFactorExitProperty = dynamicProperties[expectedDampingFactorExitPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dampingsFactorExitProperty,
                hydraulicDataCategory,
                "Dempingsfactor bij uittredepunt [-]",
                "Dempingsfactor relateert respons van stijghoogte bij binnenteen aan buitenwaterstand.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(dampingsFactorExitProperty.Converter);

            PropertyDescriptor phreaticLevelExitProperty = dynamicProperties[expectedPhreaticLevelExitPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                phreaticLevelExitProperty,
                hydraulicDataCategory,
                "Polderpeil [m+NAP]",
                "Polderpeil.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(phreaticLevelExitProperty.Converter);

            PropertyDescriptor piezometricHeadExitProperty = dynamicProperties[expectedPiezometricHeadExitPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                piezometricHeadExitProperty,
                hydraulicDataCategory,
                "Stijghoogte bij uittredepunt [m+NAP]",
                "Stijghoogte bij uittredepunt.",
                true);

            PropertyDescriptor surfaceLineProperty = dynamicProperties[expectedSurfaceLinePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                surfaceLineProperty,
                schematizationCategory,
                "Profielschematisatie",
                "De schematisatie van de hoogte van het dwarsprofiel.");

            PropertyDescriptor stochasticSoilModelProperty = dynamicProperties[expectedStochasticSoilModelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stochasticSoilModelProperty,
                schematizationCategory,
                "Stochastisch ondergrondmodel",
                "De verschillende opbouwen van de ondergrond en hun respectieve kansen van voorkomen.");

            PropertyDescriptor stochasticSoilProfileProperty = dynamicProperties[expectedStochasticSoilProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                stochasticSoilProfileProperty,
                schematizationCategory,
                "Ondergrondschematisatie",
                "De opbouw van de ondergrond.");

            PropertyDescriptor seepageLengthProperty = dynamicProperties[expectedSeepageLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                seepageLengthProperty,
                schematizationCategory,
                "Kwelweglengte [m]",
                "De horizontale afstand tussen intrede- en uittredepunt die het kwelwater ondergronds aflegt voordat het weer aan de oppervlakte komt.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(seepageLengthProperty.Converter);

            PropertyDescriptor thicknessCoverageLayerProperty = dynamicProperties[expectedThicknessCoverageLayerPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thicknessCoverageLayerProperty,
                schematizationCategory,
                "Totale deklaagdikte bij uittredepunt [m]",
                "Totale deklaagdikte bij uittredepunt.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(thicknessCoverageLayerProperty.Converter);

            PropertyDescriptor effectiveThicknessCoverageLayerProperty = dynamicProperties[expectedEffectiveThicknessCoverageLayerPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                effectiveThicknessCoverageLayerProperty,
                schematizationCategory,
                "Effectieve deklaagdikte bij uittredepunt [m]",
                "Effectieve deklaagdikte bij uittredepunt.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(effectiveThicknessCoverageLayerProperty.Converter);

            PropertyDescriptor thicknessAquiferLayerProperty = dynamicProperties[expectedThicknessAquiferLayerPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                thicknessAquiferLayerProperty,
                schematizationCategory,
                "Dikte watervoerend pakket [m]",
                "De dikte van de bovenste voor doorlatendheid te onderscheiden zandlaag of combinatie van zandlagen.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(thicknessAquiferLayerProperty.Converter);

            PropertyDescriptor darcyPermeabilityProperty = dynamicProperties[expectedDarcyPermeabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                darcyPermeabilityProperty,
                schematizationCategory,
                "Doorlatendheid aquifer [m/s]",
                "Darcy-snelheid waarmee water door de eerste voor doorlatendheid te onderscheiden zandlaag loopt.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(darcyPermeabilityProperty.Converter);

            PropertyDescriptor diameter70Property = dynamicProperties[expectedDiameter70PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                diameter70Property,
                schematizationCategory,
                "De d70 in de bovenste zandlaag [m]",
                "Zeefmaat waar 70 gewichtsprocent van de korrels uit een zandlaag doorheen gaat. Hier de korreldiameter van het bovenste gedeelte van de voor doorlatendheid te onderscheiden zandlaag, bepaald zonder fijne fractie (< 63µm).",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(diameter70Property.Converter);

            PropertyDescriptor saturatedVolumicWeightOfCoverageLayerProperty = dynamicProperties[expectedSaturatedVolumicWeightOfCoverageLayerPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                saturatedVolumicWeightOfCoverageLayerProperty,
                schematizationCategory,
                "Verzadigd gewicht deklaag [kN/m³]",
                "Verzadigd gewicht deklaag.",
                true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(saturatedVolumicWeightOfCoverageLayerProperty.Converter);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithOrWithoutSurfaceLine_EntryAndExitPointPropertyReadOnlyWithoutSurfaceLine(bool withSurfaceLine)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            if (withSurfaceLine)
            {
                var surfaceLine = new PipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(0, 0, 0),
                    new Point3D(2, 0, 2)
                });
                inputParameters.SurfaceLine = surfaceLine;
            }

            // Call
            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new PipingInputContextProperties(context, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            const string schematizationCategory = "Schematisatie";

            PropertyDescriptor entryPointLProperty = dynamicProperties[expectedEntryPointLPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                entryPointLProperty,
                schematizationCategory,
                "Intredepunt",
                "De positie in het dwarsprofiel van het intredepunt.",
                !withSurfaceLine);

            PropertyDescriptor exitPointLProperty = dynamicProperties[expectedExitPointLPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                exitPointLProperty,
                schematizationCategory,
                "Uittredepunt",
                "De positie in het dwarsprofiel van het uittredepunt.",
                !withSurfaceLine);

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

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new PipingInputContextProperties(context, handler)
            {
                UseAssessmentLevelManualInput = useManualAssessmentLevelInput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            const string hydraulicDataCategory = "Hydraulische gegevens";
            if (!useManualAssessmentLevelInput)
            {
                Assert.AreEqual(18, dynamicProperties.Count);

                PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[expectedSelectedHydraulicBoundaryLocationPropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                    hydraulicBoundaryLocationProperty,
                    hydraulicDataCategory,
                    "Locatie met hydraulische randvoorwaarden",
                    "De locatie met hydraulische randvoorwaarden waarvan het berekende toetspeil wordt gebruikt.");

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                    assessmentLevelProperty,
                    hydraulicDataCategory,
                    "Toetspeil [m+NAP]",
                    "Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.",
                    true);
            }
            else
            {
                Assert.AreEqual(17, dynamicProperties.Count);

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[0];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                    assessmentLevelProperty,
                    hydraulicDataCategory,
                    "Toetspeil [m+NAP]",
                    "Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.");
            }
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var random = new Random(22);

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, random.NextDouble(), new[]
                {
                    new PipingSoilLayer(random.NextDouble())
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D)
            );
            var stochasticSoilModel = new PipingStochasticSoilModel("StochasticSoilModelName")
            {
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile
                }
            };

            HydraulicBoundaryLocation testHydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(0.0);

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation,
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile,
                UseAssessmentLevelManualInput = false
            };

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // Call & Assert
            Assert.AreEqual(inputParameters.PhreaticLevelExit.Mean, properties.PhreaticLevelExit.Mean);
            Assert.AreEqual(inputParameters.PhreaticLevelExit.StandardDeviation, properties.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(inputParameters.DampingFactorExit.Mean, properties.DampingFactorExit.Mean);
            Assert.AreEqual(inputParameters.DampingFactorExit.StandardDeviation, properties.DampingFactorExit.StandardDeviation);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.Mean, properties.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.StandardDeviation, properties.ThicknessCoverageLayer.StandardDeviation);
            Assert.AreEqual(inputParameters.EffectiveThicknessCoverageLayer.Mean, properties.EffectiveThicknessCoverageLayer.Mean);
            Assert.AreEqual(inputParameters.EffectiveThicknessCoverageLayer.StandardDeviation, properties.EffectiveThicknessCoverageLayer.StandardDeviation);
            Assert.AreEqual(inputParameters.Diameter70.Mean, properties.Diameter70.Mean);
            Assert.AreEqual(inputParameters.Diameter70.CoefficientOfVariation, properties.Diameter70.CoefficientOfVariation);
            Assert.AreEqual(inputParameters.DarcyPermeability.Mean, properties.DarcyPermeability.Mean);
            Assert.AreEqual(inputParameters.DarcyPermeability.CoefficientOfVariation, properties.DarcyPermeability.CoefficientOfVariation);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.Mean, properties.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.StandardDeviation, properties.ThicknessAquiferLayer.StandardDeviation);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean,
                properties.SaturatedVolumicWeightOfCoverageLayer.Mean);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                properties.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift,
                properties.SaturatedVolumicWeightOfCoverageLayer.Shift);

            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExit);

            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.SeepageLength.Mean);
            Assert.AreEqual(inputParameters.SeepageLength.CoefficientOfVariation, properties.SeepageLength.CoefficientOfVariation);
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

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new PipingInputContextProperties(context, handler);

            const double assessmentLevel = 0.36;
            const double entryPointL = 0.12;
            const double exitPointL = 0.44;
            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingStochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            PipingStochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
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
            properties.DampingFactorExit.Mean = dampingFactorExit.Distribution.Mean;
            properties.DampingFactorExit.StandardDeviation = dampingFactorExit.Distribution.StandardDeviation;
            properties.PhreaticLevelExit.Mean = phreaticLevelExit.Distribution.Mean;
            properties.PhreaticLevelExit.StandardDeviation = phreaticLevelExit.Distribution.StandardDeviation;

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
        public void SurfaceLine_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            PipingSurfaceLine newSurfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(p => p.SurfaceLine = newSurfaceLine, calculation);
        }

        [Test]
        public void StochasticSoilModel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            PipingStochasticSoilModel newSoilModel = ValidStochasticSoilModel(0.0, 4.0);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.StochasticSoilModel = newSoilModel, calculation);
        }

        [Test]
        public void StochasticSoilProfile_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            PipingStochasticSoilProfile newSoilProfile = ValidStochasticSoilModel(0.0, 4.0).StochasticSoilProfiles.First();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.StochasticSoilProfile = newSoilProfile, calculation);
        }

        [Test]
        public void AssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newAssessmentLevel = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = true
                }
            };

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.AssessmentLevel = newAssessmentLevel, calculation);
        }

        [Test]
        public void DampingFactorExitMean_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var mean = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.DampingFactorExit.Mean = mean, calculation);
        }

        [Test]
        public void DampingFactorExitStandardDeviation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var standardDeviation = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.DampingFactorExit.StandardDeviation = standardDeviation, calculation);
        }

        [Test]
        public void PhreaticLevelExitMean_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var mean = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.PhreaticLevelExit.Mean = mean, calculation);
        }

        [Test]
        public void PhreaticLevelExitStandardDeviation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var standardDeviation = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.PhreaticLevelExit.StandardDeviation = standardDeviation, calculation);
        }

        [Test]
        public void EntryPoinL_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newEntryPointL = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.EntryPointL = newEntryPointL, calculation);
        }

        [Test]
        public void ExitPointL_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newExitPointL = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.ExitPointL = newExitPointL, calculation);
        }

        [Test]
        public void UseCustomAssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.UseAssessmentLevelManualInput = true,
                                                                    calculation);
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
            const int numberOfChangedProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.Attach(inputObserver);

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new PipingInputContextProperties(context, handler)
            {
                ExitPointL = (RoundedDouble) exitPoint,
                EntryPointL = (RoundedDouble) entryPoint
            };

            // Assert
            Assert.AreEqual(seepageLength, properties.SeepageLength.Mean, 1e-6);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Mean, inputParameters.SeepageLength.Mean);

            mocks.VerifyAll();
        }

        [Test]
        public void SeepageLength_EntryPointAndThenExitPointSet_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            const int numberOfChangedProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.Attach(inputObserver);

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new PipingInputContextProperties(context, handler)
            {
                EntryPointL = (RoundedDouble) 0.5,
                ExitPointL = (RoundedDouble) 2
            };

            // Assert
            Assert.AreEqual(1.5, properties.SeepageLength.Mean.Value);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Mean, inputParameters.SeepageLength.Mean);

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
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                EntryPointL = (RoundedDouble) 2.0
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var newExitPointL = (RoundedDouble) newExitPoint;
            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            TestDelegate call = () => properties.ExitPointL = newExitPointL;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
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
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var entryPoint = (RoundedDouble) newEntryPoint;

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                ExitPointL = (RoundedDouble) 2.0
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            TestDelegate call = () => properties.EntryPointL = entryPoint;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [SetCulture("nl-NL")]
        public void EntryPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                ExitPointL = (RoundedDouble) 2.0
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var entryPointL = (RoundedDouble) (-15.0);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            TestDelegate call = () => properties.EntryPointL = entryPointL;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 4,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ExitPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                EntryPointL = (RoundedDouble) 2.0
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var exitPointL = (RoundedDouble) 10.0;

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            TestDelegate call = () => properties.ExitPointL = exitPointL;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 4,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelIsNaN_AssessmentLevelSetToNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var testLevel = (RoundedDouble) new Random(21).NextDouble();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                testLevel);
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

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

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var random = new Random(21);
            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(50)
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new PipingInputContextProperties(context, handler)
            {
                UseAssessmentLevelManualInput = false
            };

            var testLevel = (RoundedDouble) random.NextDouble();

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
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            properties.AssessmentLevel = (RoundedDouble) testLevel;

            // Assert
            Assert.AreEqual(2, properties.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(testLevel, properties.AssessmentLevel, properties.AssessmentLevel.GetAccuracy());
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentLevelSetWithoutHydraulicBoundaryLocation_WhenUseAndSetNewLocation_UpdateAssessmentLevelWithLocationValues()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var random = new Random(21);
            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true,
                AssessmentLevel = (RoundedDouble) random.NextDouble()
            };

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new PipingInputContextProperties(context, handler);

            var testLevel = (RoundedDouble) random.NextDouble();
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

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0)
                }
            };
            var failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            PipingSurfaceLine newSurfaceLine = ValidSurfaceLine(0, 2);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new PipingInputContextProperties(context, handler);

            inputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, new TestPipingSoilProfile());

            // Call
            properties.SurfaceLine = newSurfaceLine;

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            PipingSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.0, new TestPipingSoilProfile());
            var stochasticSoilModel = new PipingStochasticSoilModel("StochasticSoilModelName")
            {
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile
                }
            };

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 new[]
                                                 {
                                                     stochasticSoilModel
                                                 },
                                                 failureMechanism,
                                                 assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

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

            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.0, new TestPipingSoilProfile());
            var stochasticSoilModel = new PipingStochasticSoilModel("StochasticSoilModelName")
            {
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile
                }
            };
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0, 4),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            PipingInput inputParameters = calculationItem.InputParameters;
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 new[]
                                                 {
                                                     stochasticSoilModel
                                                 },
                                                 failureMechanism,
                                                 assessmentSection);

            PipingSurfaceLine newSurfaceLine = ValidSurfaceLine(0, 2);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            properties.SurfaceLine = newSurfaceLine;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var contextProperties = new PipingInputContextProperties(context, handler);
            inputParameters.HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.0);

            NormalDistributionDesignVariableProperties phreaticLevelExitProperty = contextProperties.PhreaticLevelExit;
            mocks.ReplayAll();

            // When
            if (propertyIndexToChange == 1)
            {
                phreaticLevelExitProperty.Mean = (RoundedDouble) 2.3;
            }
            else if (propertyIndexToChange == 2)
            {
                phreaticLevelExitProperty.StandardDeviation = (RoundedDouble) 2.3;
            }

            // Then
            Assert.IsFalse(double.IsNaN(inputParameters.PiezometricHeadExit));
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableSurfaceLines_Always_ReturnAllPipingSurfaceLines()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context, handler);

            // Call
            IEnumerable<PipingSurfaceLine> surfaceLines = properties.GetAvailableSurfaceLines();

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context, handler);

            // Precondition:
            Assert.IsNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<PipingStochasticSoilModel> soilModels = properties.GetAvailableStochasticSoilModels();

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            var failureMechanism = new PipingFailureMechanism();
            var soilModels = new[]
            {
                new PipingStochasticSoilModel("A")
                {
                    Geometry =
                    {
                        new Point2D(2, -1),
                        new Point2D(2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                    }
                },
                new PipingStochasticSoilModel("C")
                {
                    Geometry =
                    {
                        new Point2D(-2, -1),
                        new Point2D(-2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                    }
                },
                new PipingStochasticSoilModel("E")
                {
                    Geometry =
                    {
                        new Point2D(6, -1),
                        new Point2D(6, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                    }
                }
            };
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
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
            var properties = new PipingInputContextProperties(context, handler);

            // Precondition:
            Assert.IsNotNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<PipingStochasticSoilModel> availableStochasticSoilModels = properties.GetAvailableStochasticSoilModels();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.StochasticSoilModels[0],
                failureMechanism.StochasticSoilModels[2]
            }, availableStochasticSoilModels);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableStochasticSoilProfiles_NoStochasticSoilModel_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context, handler);

            // Precondition
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<PipingStochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var model = new PipingStochasticSoilModel("A")
            {
                StochasticSoilProfiles =
                {
                    new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
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
            var properties = new PipingInputContextProperties(context, handler);

            // Precondition
            Assert.IsNotNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<PipingStochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context, handler);

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
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            PipingStochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            PipingStochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = ValidSurfaceLine(0, 4.0),
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = soilProfile
                }
            };
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // When
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Then
            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context, handler);

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);
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
            var properties = new PipingInputContextProperties(context, handler);

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
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            PipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);
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

            PipingSurfaceLine newSurfaceLine = ValidSurfaceLine(0.0, 5.0);
            newSurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 190);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
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
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("AssessmentLevel");

            // Assert
            Assert.AreNotEqual(useCustomAssessmentLevel, result);
        }

        [Test]
        public void DynamicReadOnlyValidationMethod_AnyOtherProperty_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("prop");

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_SelectedHydraulicBoundaryLocation_DependsOnUseCustomAssessmentLevel(bool useCustomAssessmentLevel)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
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
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("SelectedHydraulicBoundaryLocation");

            // Assert
            Assert.AreNotEqual(useCustomAssessmentLevel, result);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("prop");

            // Assert
            Assert.IsFalse(result);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
            Action<PipingInputContextProperties> setProperty,
            PipingCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            PipingInput inputParameters = calculation.InputParameters;

            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(inputParameters,
                                                 calculation,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new PipingInputContextProperties(context, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static PipingStochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            return new PipingStochasticSoilModel("StochasticSoilModelName")
            {
                Geometry =
                {
                    new Point2D(xMin, 1.0),
                    new Point2D(xMax, 0.0)
                },
                StochasticSoilProfiles =
                {
                    new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                }
            };
        }

        private static PipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}