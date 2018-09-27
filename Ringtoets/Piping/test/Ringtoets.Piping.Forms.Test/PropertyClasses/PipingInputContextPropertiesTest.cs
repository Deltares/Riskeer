// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;

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
            TestDelegate test = () => new PipingInputContextProperties(null,
                                                                       AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                                       handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetAssessmentLevelFuncNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            TestDelegate test = () => new PipingInputContextProperties(context, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            TestDelegate test = () => new PipingInputContextProperties(context,
                                                                       AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("propertyChangeHandler", exception.ParamName);
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

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.AreSame(context, properties.Data);

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.DampingFactorExit);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.DampingFactorExit));

            Assert.IsInstanceOf<NormalDistributionDesignVariableProperties>(properties.PhreaticLevelExit);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.PhreaticLevelExit));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.SeepageLength);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.SeepageLength));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.ThicknessCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.ThicknessCoverageLayer));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.EffectiveThicknessCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.EffectiveThicknessCoverageLayer));

            Assert.IsInstanceOf<LogNormalDistributionDesignVariableProperties>(properties.ThicknessAquiferLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.ThicknessAquiferLayer));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.DarcyPermeability);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.DarcyPermeability));

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariableProperties>(properties.Diameter70);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.Diameter70));

            Assert.IsInstanceOf<ShiftedLogNormalDistributionDesignVariableProperties>(properties.SaturatedVolumicWeightOfCoverageLayer);
            TestHelper.AssertTypeConverter<PipingInputContextProperties, ExpandableObjectConverter>(
                nameof(PipingInputContextProperties.SaturatedVolumicWeightOfCoverageLayer));

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

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler)
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
                "Hydraulische belastingenlocatie",
                "De hydraulische belastingenlocatie waarvan de berekende waterstand wordt gebruikt.");

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                assessmentLevelProperty,
                hydraulicDataCategory,
                "Waterstand [m+NAP]",
                "Waterstand met een overschrijdingsfrequentie gelijk aan de norm van het dijktraject.",
                true);

            PropertyDescriptor useHydraulicBoundaryLocationProperty = dynamicProperties[expectedUseHydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                useHydraulicBoundaryLocationProperty,
                hydraulicDataCategory,
                "Handmatig waterstand invoeren",
                "Sta toe om de waterstand handmatig te specificeren?");

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
                "De verschillende opbouwen van de ondergrond en hun respectievelijke kansen van voorkomen.");

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            if (withSurfaceLine)
            {
                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(0, 0, 0),
                    new Point3D(2, 0, 2)
                });
                calculationItem.InputParameters.SurfaceLine = surfaceLine;
            }

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            // Call
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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

            var context = new PipingInputContext(calculationItem.InputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler)
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
                    "Hydraulische belastingenlocatie",
                    "De hydraulische belastingenlocatie waarvan de berekende waterstand wordt gebruikt.");

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[expectedAssessmentLevelPropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                    assessmentLevelProperty,
                    hydraulicDataCategory,
                    "Waterstand [m+NAP]",
                    "Waterstand met een overschrijdingsfrequentie gelijk aan de norm van het dijktraject.",
                    true);
            }
            else
            {
                Assert.AreEqual(17, dynamicProperties.Count);

                PropertyDescriptor assessmentLevelProperty = dynamicProperties[0];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                    assessmentLevelProperty,
                    hydraulicDataCategory,
                    "Waterstand [m+NAP]",
                    "Waterstand met een overschrijdingsfrequentie gelijk aan de norm van het dijktraject.");
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetProperties_WithData_ReturnExpectedValues(bool useManualAssessmentLevelInput)
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
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName", new[]
            {
                stochasticSoilProfile
            });

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useManualAssessmentLevelInput,
                    AssessmentLevel = random.NextRoundedDouble(),
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
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

            // Call
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Assert
            Assert.AreEqual(inputParameters.PhreaticLevelExit.Mean, properties.PhreaticLevelExit.Mean);
            Assert.AreEqual(inputParameters.PhreaticLevelExit.StandardDeviation, properties.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(inputParameters.DampingFactorExit.Mean, properties.DampingFactorExit.Mean);
            Assert.AreEqual(inputParameters.DampingFactorExit.StandardDeviation, properties.DampingFactorExit.StandardDeviation);

            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(inputParameters);
            Assert.AreEqual(thicknessCoverageLayer.Mean, properties.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(thicknessCoverageLayer.StandardDeviation, properties.ThicknessCoverageLayer.StandardDeviation);

            LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(inputParameters);
            Assert.AreEqual(effectiveThicknessCoverageLayer.Mean, properties.EffectiveThicknessCoverageLayer.Mean);
            Assert.AreEqual(effectiveThicknessCoverageLayer.StandardDeviation, properties.EffectiveThicknessCoverageLayer.StandardDeviation);

            VariationCoefficientLogNormalDistribution diameterD70 = DerivedPipingInput.GetDiameterD70(inputParameters);
            Assert.AreEqual(diameterD70.Mean, properties.Diameter70.Mean);
            Assert.AreEqual(diameterD70.CoefficientOfVariation, properties.Diameter70.CoefficientOfVariation);

            VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(inputParameters);
            Assert.AreEqual(darcyPermeability.Mean, properties.DarcyPermeability.Mean);
            Assert.AreEqual(darcyPermeability.CoefficientOfVariation, properties.DarcyPermeability.CoefficientOfVariation);

            LogNormalDistribution thicknessAquiferLayer = DerivedPipingInput.GetThicknessAquiferLayer(inputParameters);
            Assert.AreEqual(thicknessAquiferLayer.Mean, properties.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(thicknessAquiferLayer.StandardDeviation, properties.ThicknessAquiferLayer.StandardDeviation);

            LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLayer.Mean,
                            properties.SaturatedVolumicWeightOfCoverageLayer.Mean);
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                            properties.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation);
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLayer.Shift,
                            properties.SaturatedVolumicWeightOfCoverageLayer.Shift);

            RoundedDouble expectedAssessmentLevel = useManualAssessmentLevelInput
                                                        ? inputParameters.AssessmentLevel
                                                        : AssessmentSectionTestHelper.GetTestAssessmentLevel();

            Assert.AreEqual(DerivedPipingInput.GetPiezometricHeadExit(inputParameters, expectedAssessmentLevel), properties.PiezometricHeadExit);

            Assert.AreEqual(DerivedPipingInput.GetSeepageLength(inputParameters).Mean, properties.SeepageLength.Mean);
            Assert.AreEqual(DerivedPipingInput.GetSeepageLength(inputParameters).CoefficientOfVariation, properties.SeepageLength.CoefficientOfVariation);
            Assert.AreEqual(DerivedPipingInput.GetSeepageLength(inputParameters).Mean, properties.ExitPointL - properties.EntryPointL);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(stochasticSoilProfile, properties.StochasticSoilProfile);
            Assert.AreSame(stochasticSoilModel, properties.StochasticSoilModel);

            Assert.AreSame(hydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedAssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(inputParameters.UseAssessmentLevelManualInput, properties.UseAssessmentLevelManualInput);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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

            // When
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

            // Then
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel,
                            inputParameters.AssessmentLevel.GetAccuracy());
            Assert.AreEqual(entryPointL, inputParameters.EntryPointL,
                            inputParameters.EntryPointL.GetAccuracy());
            Assert.AreEqual(exitPointL, inputParameters.ExitPointL,
                            inputParameters.ExitPointL.GetAccuracy());
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
            SetPropertyAndVerifyNotificationsForCalculation(p => p.SurfaceLine = newSurfaceLine, calculation);
        }

        [Test]
        public void StochasticSoilModel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            PipingStochasticSoilModel newSoilModel = ValidStochasticSoilModel(0.0, 4.0);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.StochasticSoilModel = newSoilModel, calculation);
        }

        [Test]
        public void StochasticSoilProfile_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            PipingStochasticSoilProfile newSoilProfile = ValidStochasticSoilModel(0.0, 4.0).StochasticSoilProfiles.First();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.StochasticSoilProfile = newSoilProfile, calculation);
        }

        [Test]
        public void AssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newAssessmentLevel = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.AssessmentLevel = newAssessmentLevel, calculation);
        }

        [Test]
        public void DampingFactorExitMean_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var mean = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.DampingFactorExit.Mean = mean, calculation);
        }

        [Test]
        public void DampingFactorExitStandardDeviation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var standardDeviation = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.DampingFactorExit.StandardDeviation = standardDeviation, calculation);
        }

        [Test]
        public void PhreaticLevelExitMean_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var mean = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLevelExit.Mean = mean, calculation);
        }

        [Test]
        public void PhreaticLevelExitStandardDeviation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var standardDeviation = new RoundedDouble(2, 2);
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLevelExit.StandardDeviation = standardDeviation, calculation);
        }

        [Test]
        public void EntryPointL_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newEntryPointL = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.EntryPointL = newEntryPointL, calculation);
        }

        [Test]
        public void ExitPointL_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newExitPointL = new Random(21).NextRoundedDouble();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.ExitPointL = newExitPointL, calculation);
        }

        [Test]
        public void UseAssessmentLevelManualInput_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.UseAssessmentLevelManualInput = true,
                                                            calculation);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.SelectedHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), null),
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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler)
            {
                ExitPointL = (RoundedDouble) exitPoint,
                EntryPointL = (RoundedDouble) entryPoint
            };

            // Assert
            Assert.AreEqual(seepageLength, properties.SeepageLength.Mean, 1e-6);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler)
            {
                EntryPointL = (RoundedDouble) 0.5,
                ExitPointL = (RoundedDouble) 2
            };

            // Assert
            Assert.AreEqual(1.5, properties.SeepageLength.Mean.Value);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

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
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 2.0
                }
            };
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(calculationItem.InputParameters,
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    ExitPointL = (RoundedDouble) 2.0
                }
            };
            var failureMechanism = new PipingFailureMechanism();

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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    ExitPointL = (RoundedDouble) 2.0
                }
            };
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(calculationItem.InputParameters,
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0),
                    EntryPointL = (RoundedDouble) 2.0
                }
            };
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingInputContext(calculationItem.InputParameters,
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            TestDelegate call = () => properties.ExitPointL = exitPointL;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 4,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            inputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

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
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName", new[]
            {
                stochasticSoilProfile
            });

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = testSurfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };
            var failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;

            var context = new PipingInputContext(inputParameters,
                                                 calculationItem,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 new[]
                                                 {
                                                     stochasticSoilModel
                                                 },
                                                 failureMechanism,
                                                 assessmentSection);

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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

            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName", new[]
            {
                stochasticSoilProfile
            });
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            properties.SurfaceLine = newSurfaceLine;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            var failureMechanism = new PipingFailureMechanism();
            var soilModels = new[]
            {
                new PipingStochasticSoilModel("A", new[]
                {
                    new Point2D(2, -1),
                    new Point2D(2, 1)
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                }),
                new PipingStochasticSoilModel("C", new[]
                {
                    new Point2D(-2, -1),
                    new Point2D(-2, 1)
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                }),
                new PipingStochasticSoilModel("E", new[]
                {
                    new Point2D(6, -1),
                    new Point2D(6, 1)
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
                })
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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A", new[]
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
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

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                 failureMechanism, assessmentSection);
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
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

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

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
            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
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

            observable.Expect(o => o.NotifyObservers());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

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
        public void DynamicReadOnlyValidationMethod_AssessmentLevel_DependsOnUseAssessmentLevelManualInput(bool useAssessmentLevelManualInput)
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
                    UseAssessmentLevelManualInput = useAssessmentLevelManualInput
                }
            };

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("AssessmentLevel");

            // Assert
            Assert.AreNotEqual(useAssessmentLevelManualInput, result);
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("prop");

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_SelectedHydraulicBoundaryLocation_DependsOnUseAssessmentLevelManualInput(bool useAssessmentLevelManualInput)
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
                    UseAssessmentLevelManualInput = useAssessmentLevelManualInput
                }
            };

            var context = new PipingInputContext(calculation.InputParameters, calculation,
                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                 failureMechanism, assessmentSection);

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("SelectedHydraulicBoundaryLocation");

            // Assert
            Assert.AreNotEqual(useAssessmentLevelManualInput, result);
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("prop");

            // Assert
            Assert.IsFalse(result);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(
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

            var properties = new PipingInputContextProperties(context,
                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                              handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static PipingStochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            return new PipingStochasticSoilModel("StochasticSoilModelName", new[]
            {
                new Point2D(xMin, 1.0),
                new Point2D(xMax, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });
        }

        private static PipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}