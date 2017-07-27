﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextPropertiesTest
    {
        private const int expectedSelectedHydraulicBoundaryLocationPropertyIndex = 0;
        private const int expectedAssessmentLevelPropertyIndex = 1;
        private const int expectedUseHydraulicBoundaryLocationPropertyIndex = 2;
        private const int expectedSurfaceLinePropertyIndex = 3;
        private const int expectedStochasticSoilModelPropertyIndex = 4;
        private const int expectedStochasticSoilProfilePropertyIndex = 5;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsInputContextProperties(null, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            TestDelegate test = () => new MacroStabilityInwardsInputContextProperties(context, null);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler)
            {
                UseAssessmentLevelManualInput = false
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(6, dynamicProperties.Count);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            // Call
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler)
            {
                UseAssessmentLevelManualInput = useManualAssessmentLevelInput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            const string hydraulicDataCategory = "Hydraulische gegevens";
            if (!useManualAssessmentLevelInput)
            {
                Assert.AreEqual(6, dynamicProperties.Count);

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
                Assert.AreEqual(5, dynamicProperties.Count);

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

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(string.Empty, random.NextDouble(), new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };
            var stochasticSoilModel = new StochasticSoilModel("StochasticSoilModelName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            HydraulicBoundaryLocation testHydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(0.0);

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation,
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile,
                UseAssessmentLevelManualInput = false
            };

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Call & Assert
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevel);
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            const double assessmentLevel = 0.36;
            const double entryPointL = 0.12;
            const double exitPointL = 0.44;
            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
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
            properties.StochasticSoilModel = soilModel;
            properties.StochasticSoilProfile = soilProfile;

            // Assert
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel.Value);
            Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
            Assert.AreSame(soilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(soilProfile, inputParameters.StochasticSoilProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0.0, 4.0);
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(p => p.SurfaceLine = newSurfaceLine, calculation);
        }

        [Test]
        public void StochasticSoilModel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            StochasticSoilModel newSoilModel = ValidStochasticSoilModel(0.0, 4.0);
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.StochasticSoilModel = newSoilModel, calculation);
        }

        [Test]
        public void StochasticSoilProfile_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            StochasticSoilProfile newSoilProfile = ValidStochasticSoilModel(0.0, 4.0).StochasticSoilProfiles.First();
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.StochasticSoilProfile = newSoilProfile, calculation);
        }

        [Test]
        public void AssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            RoundedDouble newAssessmentLevel = new Random(21).NextRoundedDouble();
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
        public void UseCustomAssessmentLevel_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.UseAssessmentLevelManualInput = true,
                                                                    calculation);
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var testLevel = (RoundedDouble) new Random(21).NextDouble();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                testLevel);
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(hydraulicBoundaryLocation, null);

            var context = new MacroStabilityInwardsInputContext(calculationItem.InputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var random = new Random(21);
            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(50)
            };

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler)
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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                UseAssessmentLevelManualInput = true
            };

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var random = new Random(21);
            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                UseAssessmentLevelManualInput = true,
                AssessmentLevel = (RoundedDouble) random.NextDouble()
            };

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            MacroStabilityInwardsSurfaceLine newSurfaceLine = ValidSurfaceLine(0, 2);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            inputParameters.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            };

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
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            };
            var stochasticSoilModel = new StochasticSoilModel("StochasticSoilModelName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                new[]
                                                                {
                                                                    stochasticSoilModel
                                                                },
                                                                failureMechanism,
                                                                assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var soilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            };
            var stochasticSoilModel = new StochasticSoilModel("StochasticSoilModelName");
            stochasticSoilModel.StochasticSoilProfiles.Add(soilProfile);
            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0, 4),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = soilProfile
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

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Call
            properties.SurfaceLine = newSurfaceLine;

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
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            var stochasticSoilProfile1 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            };
            var stochasticSoilModel1 = new StochasticSoilModel("StochasticSoilModel1Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile1);

            var stochasticSoilProfile2 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            };
            var stochasticSoilModel2 = new StochasticSoilModel("StochasticSoilModel2Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel1,
                StochasticSoilProfile = stochasticSoilProfile1
            };
            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Call
            properties.StochasticSoilModel = stochasticSoilModel2;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilProfile);
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
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var surfaceLine = new MacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var soilModels = new[]
            {
                new StochasticSoilModel("A")
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
                new StochasticSoilModel("C")
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
                new StochasticSoilModel("E")
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
            };
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Precondition:
            Assert.IsNotNull(calculation.InputParameters.SurfaceLine);

            // Call
            IEnumerable<StochasticSoilModel> availableStochasticSoilModels = properties.GetAvailableStochasticSoilModels();

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
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var model = new StochasticSoilModel("A")
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                }
            };
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    StochasticSoilModel = model
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            StochasticSoilModel soilModel = ValidStochasticSoilModel(0.0, 4.0);
            StochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles.First();
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
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

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                failureMechanism.SurfaceLines, failureMechanism.StochasticSoilModels,
                                                                failureMechanism, assessmentSection);
            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            MacroStabilityInwardsSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);
            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
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

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useCustomAssessmentLevel
                }
            };

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput)
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = useCustomAssessmentLevel
                }
            };

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var calculation = new MacroStabilityInwardsCalculationScenario(failureMechanism.GeneralInput);

            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation,
                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism, assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Call
            bool result = properties.DynamicVisibleValidationMethod("prop");

            // Assert
            Assert.IsFalse(result);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
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
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSection);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsInputContextProperties(context, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static StochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            var stochasticSoilModel = new StochasticSoilModel("StochasticSoilModelName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1234)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile()
            });
            stochasticSoilModel.Geometry.Add(new Point2D(xMin, 1.0));
            stochasticSoilModel.Geometry.Add(new Point2D(xMax, 0.0));
            return stochasticSoilModel;
        }

        private static MacroStabilityInwardsSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}