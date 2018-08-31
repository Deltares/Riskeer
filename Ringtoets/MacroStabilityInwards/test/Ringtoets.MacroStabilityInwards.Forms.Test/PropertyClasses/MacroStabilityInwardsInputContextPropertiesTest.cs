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
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextPropertiesTest
    {
        private const int expectedSelectedHydraulicBoundaryLocationPropertyIndex = 0;
        private const int expectedAssessmentLevelPropertyIndex = 1;
        private const int expectedUseHydraulicBoundaryLocationPropertyIndex = 2;
        private const int expectedDikeSoilScenarioPropertyIndex = 3;
        private const int expectedWaterStressesPropertyIndex = 4;
        private const int expectedSurfaceLinePropertyIndex = 5;
        private const int expectedStochasticSoilModelPropertyIndex = 6;
        private const int expectedStochasticSoilProfilePropertyIndex = 7;
        private const int expectedSlipPlaneMinimumDepthPropertyIndex = 8;
        private const int expectedSlipPlaneMinimumLengthPropertyIndex = 9;
        private const int expectedMaximumSliceWidthPropertyIndex = 10;
        private const int expectedSlipPlaneSettingsPropertyIndex = 11;
        private const int expectedGridSettingsPropertyIndex = 12;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsInputContextProperties(null,
                                                                                      AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            TestDelegate test = () => new MacroStabilityInwardsInputContextProperties(context, null, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            TestDelegate test = () => new MacroStabilityInwardsInputContextProperties(context,
                                                                                      AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsInputContext>>(properties);
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler)
            {
                UseAssessmentLevelManualInput = false
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(13, dynamicProperties.Count);

            const string hydraulicDataCategory = "\t\tHydraulische gegevens";
            const string schematizationCategory = "\tSchematisatie";
            const string settingsCategory = "Instellingen";

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

            PropertyDescriptor dikeSoilScenarioProperty = dynamicProperties[expectedDikeSoilScenarioPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsInputContextProperties, EnumTypeConverter>(
                nameof(MacroStabilityInwardsInputContextProperties.DikeSoilScenario));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dikeSoilScenarioProperty,
                hydraulicDataCategory,
                "Dijk/bodem materiaal",
                "Dijktype van de geschematiseerde dijk.");

            PropertyDescriptor waterStressesProperty = dynamicProperties[expectedWaterStressesPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsInputContextProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsInputContextProperties.WaterStressesProperties));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterStressesProperty,
                hydraulicDataCategory,
                "Waterspanningen",
                "Eigenschappen van de waterspanning.",
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

            PropertyDescriptor slipPlaneMinimumDepthProperty = dynamicProperties[expectedSlipPlaneMinimumDepthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                slipPlaneMinimumDepthProperty,
                settingsCategory,
                "Minimale glijvlakdiepte [m]",
                "Minimale diepte van het berekende glijvlak ten opzichte van het maaiveld.");

            PropertyDescriptor slipPlaneMinimumLengthProperty = dynamicProperties[expectedSlipPlaneMinimumLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                slipPlaneMinimumLengthProperty,
                settingsCategory,
                "Minimale glijvlaklengte [m]",
                "Minimale lengte van het berekende glijvlak.");

            PropertyDescriptor maximumSliceWidthProperty = dynamicProperties[expectedMaximumSliceWidthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                maximumSliceWidthProperty,
                settingsCategory,
                "Maximale lamelbreedte [m]",
                "Maximale breedte van een lamel.");

            PropertyDescriptor slipPlaneSettingsProperty = dynamicProperties[expectedSlipPlaneSettingsPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsInputContextProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsInputContextProperties.SlipPlaneSettings));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                slipPlaneSettingsProperty,
                settingsCategory,
                "Zonering glijvlak",
                "Eigenschappen van de zonering van het glijvlak.",
                true);

            PropertyDescriptor gridSettingsProperty = dynamicProperties[expectedGridSettingsPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsInputContextProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsInputContextProperties.GridSettings));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                gridSettingsProperty,
                settingsCategory,
                "Rekengrids",
                "Eigenschappen van de rekengrids.",
                true);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler)
            {
                UseAssessmentLevelManualInput = useManualAssessmentLevelInput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            const string hydraulicDataCategory = "\t\tHydraulische gegevens";
            if (!useManualAssessmentLevelInput)
            {
                Assert.AreEqual(13, dynamicProperties.Count);

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
                Assert.AreEqual(12, dynamicProperties.Count);

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

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, random.NextDouble(), new[]
            {
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
                {
                    Data =
                    {
                        IsAquifer = true
                    }
                }
            }));
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("StochasticSoilModelName",
                                                                                                   new[]
                                                                                                   {
                                                                                                       stochasticSoilProfile
                                                                                                   });

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var calculationItem = new MacroStabilityInwardsCalculationScenario
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
            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Assert
            RoundedDouble expectedAssessmentLevel = useManualAssessmentLevelInput
                                                        ? inputParameters.AssessmentLevel
                                                        : AssessmentSectionHelper.GetTestAssessmentLevel();
            Assert.AreEqual(expectedAssessmentLevel, properties.AssessmentLevel);
            Assert.AreSame(hydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(inputParameters.UseAssessmentLevelManualInput, properties.UseAssessmentLevelManualInput);

            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(stochasticSoilProfile, properties.StochasticSoilProfile);
            Assert.AreSame(stochasticSoilModel, properties.StochasticSoilModel);
            Assert.AreEqual(inputParameters.DikeSoilScenario, properties.DikeSoilScenario);

            Assert.AreSame(inputParameters, properties.WaterStressesProperties.Data);

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                MacroStabilityInwardsWaternetProperties waternetProperties = properties.WaterStressesProperties.WaterStressLines.WaternetExtreme;

                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                Assert.AreEqual(expectedAssessmentLevel, calculatorFactory.LastCreatedWaternetCalculator.Input.AssessmentLevel);
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetCalculator.Output, (MacroStabilityInwardsWaternet) waternetProperties.Data);
            }

            Assert.AreEqual(inputParameters.SlipPlaneMinimumDepth, properties.SlipPlaneMinimumDepth);
            Assert.AreEqual(inputParameters.SlipPlaneMinimumLength, properties.SlipPlaneMinimumLength);
            Assert.AreEqual(inputParameters.MaximumSliceWidth, properties.MaximumSliceWidth);

            Assert.AreSame(inputParameters, properties.GridSettings.Data);

            Assert.AreSame(inputParameters, properties.SlipPlaneSettings.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            var random = new Random(21);
            const double assessmentLevel = 0.36;
            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            MacroStabilityInwardsStochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            MacroStabilityInwardsStochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            const MacroStabilityInwardsDikeSoilScenario dikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand;
            double slipPlaneMinimumDepth = random.NextDouble();
            double slipPlaneMinimumLength = random.NextDouble();
            double maximumSliceWidth = random.NextDouble();

            // When
            properties.AssessmentLevel = (RoundedDouble) assessmentLevel;
            properties.SurfaceLine = surfaceLine;
            properties.StochasticSoilModel = soilModel;
            properties.StochasticSoilProfile = soilProfile;
            properties.DikeSoilScenario = dikeSoilScenario;
            properties.SlipPlaneMinimumDepth = (RoundedDouble) slipPlaneMinimumDepth;
            properties.SlipPlaneMinimumLength = (RoundedDouble) slipPlaneMinimumLength;
            properties.MaximumSliceWidth = (RoundedDouble) maximumSliceWidth;

            // Then
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel, inputParameters.AssessmentLevel.GetAccuracy());
            Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
            Assert.AreSame(soilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(soilProfile, inputParameters.StochasticSoilProfile);
            Assert.AreEqual(dikeSoilScenario, inputParameters.DikeSoilScenario);
            Assert.AreEqual(slipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(slipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(maximumSliceWidth, inputParameters.MaximumSliceWidth, inputParameters.MaximumSliceWidth.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(p => p.SurfaceLine = newSurfaceLine, calculation);
        }

        [Test]
        public void StochasticSoilModel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel newSoilModel = ValidStochasticSoilModel(0.0, 4.0);
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.StochasticSoilModel = newSoilModel, calculation);
        }

        [Test]
        public void StochasticSoilProfile_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile newSoilProfile = ValidStochasticSoilModel(0.0, 4.0).StochasticSoilProfiles.First();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.StochasticSoilProfile = newSoilProfile, calculation);
        }

        [Test]
        public void AssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newAssessmentLevel = new Random(21).NextRoundedDouble();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.AssessmentLevel = newAssessmentLevel, calculation);
        }

        [Test]
        public void UseAssessmentLevelManualInput_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.UseAssessmentLevelManualInput = true,
                                                            calculation);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.SelectedHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), null),
                                                            calculation);
        }

        [Test]
        public void DikeSoilScenario_SetNewValue_SetsValuesAndUpdatesObservers()
        {
            // Setup
            var newDikeSoilScenario = new Random(21).NextEnumValue<MacroStabilityInwardsDikeSoilScenario>();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.DikeSoilScenario = newDikeSoilScenario,
                                                            calculation);
        }

        [Test]
        public void SlipPlaneMinimumDepth_SetNewValue_SetsValuesAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.SlipPlaneMinimumDepth = (RoundedDouble) 1,
                                                            calculation);
        }

        [Test]
        public void SlipPlaneMinimumLength_SetNewValue_SetsValuesAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.SlipPlaneMinimumLength = (RoundedDouble) 1,
                                                            calculation);
        }

        [Test]
        public void MaximumSliceWidth_SetNewValue_SetsValuesAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.MaximumSliceWidth = (RoundedDouble) 1,
                                                            calculation);
        }

        [Test]
        public void SurfaceLine_NewSurfaceLine_StochasticSoilModelAndSoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationItem = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0)
                }
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0, 2);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            inputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);

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

            MacroStabilityInwardsSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("StochasticSoilModelName", new[]
                {
                    stochasticSoilProfile
                });

            var calculationItem = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = testSurfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                new[]
                                                                {
                                                                    stochasticSoilModel
                                                                },
                                                                failureMechanism,
                                                                assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("StochasticSoilModelName",
                                                                                                   new[]
                                                                                                   {
                                                                                                       stochasticSoilProfile
                                                                                                   });
            var calculationItem = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0, 4),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                new[]
                                                                {
                                                                    stochasticSoilModel
                                                                },
                                                                failureMechanism,
                                                                assessmentSection);

            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0, 2);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Call
            properties.SurfaceLine = newSurfaceLine;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void StochasticSoilProfile_DifferentStochasticSoilModelWithOneProfile_SetsSoilProfileSetToProfileOfNewModel()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            MacroStabilityInwardsSoilProfile1D soilProfile1 = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile1);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel1 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("StochasticSoilModel1Name",
                                                                                                   new[]
                                                                                                   {
                                                                                                       stochasticSoilProfile1
                                                                                                   });

            MacroStabilityInwardsSoilProfile1D soilProfile2 = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile2);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel2 =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("StochasticSoilModel2Name",
                                                                                                   new[]
                                                                                                   {
                                                                                                       stochasticSoilProfile2
                                                                                                   });

            var calculationItem = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = testSurfaceLine,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilProfile1
                }
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsInput inputParameters = calculationItem.InputParameters;

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Call
            properties.StochasticSoilModel = stochasticSoilModel2;

            // Assert
            Assert.AreSame(stochasticSoilProfile2, inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAvailableSurfaceLines_Always_ReturnAllMacroStabilityInwardsSurfaceLines()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Call
            IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines = properties.GetAvailableSurfaceLines();

            // Assert
            Assert.AreSame(context.AvailableMacroStabilityInwardsSurfaceLines, surfaceLines);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Precondition:
            Assert.IsNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels = properties.GetAvailableStochasticSoilModels();

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

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var soilModels = new[]
            {
                new MacroStabilityInwardsStochasticSoilModel("A", new[]
                {
                    new Point2D(2, -1),
                    new Point2D(2, 1)
                }, new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(
                        0.2, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
                }),
                new MacroStabilityInwardsStochasticSoilModel("C", new[]
                {
                    new Point2D(-2, -1),
                    new Point2D(-2, 1)
                }, new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(
                        0.3, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
                }),
                new MacroStabilityInwardsStochasticSoilModel("E", new[]
                {
                    new Point2D(6, -1),
                    new Point2D(6, 1)
                }, new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(
                        0.3, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
                })
            };
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Precondition:
            Assert.IsNotNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> availableStochasticSoilModels = properties.GetAvailableStochasticSoilModels();

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Precondition
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A", new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(1.0, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
                });

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = model
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Precondition
            Assert.IsNotNull(calculation.InputParameters.StochasticSoilModel);

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> profiles = properties.GetAvailableStochasticSoilProfiles();

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            mockRepository.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsStochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            MacroStabilityInwardsStochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = ValidSurfaceLine(0, 4.0),
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = soilProfile
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);

            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0.0, 5.0);
            newSurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 190);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useAssessmentLevelManualInput
                }
            };

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario();

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useAssessmentLevelManualInput
                }
            };

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario();

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("prop");

            // Assert
            Assert.IsFalse(result);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(
            Action<MacroStabilityInwardsInputContextProperties> setProperty,
            MacroStabilityInwardsCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsInput inputParameters = calculation.InputParameters;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                             handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static MacroStabilityInwardsStochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("StochasticSoilModelName", new[]
            {
                new Point2D(xMin, 1.0),
                new Point2D(xMax, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile)
            });
            return stochasticSoilModel;
        }

        private static MacroStabilityInwardsSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}