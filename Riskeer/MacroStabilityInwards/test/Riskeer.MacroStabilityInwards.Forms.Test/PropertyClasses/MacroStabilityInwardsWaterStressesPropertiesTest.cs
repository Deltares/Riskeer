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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.MacroStabilityInwards.CalculatedInput.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaterStressesPropertiesTest
    {
        private const int expectedWaterLevelRiverAveragePropertyIndex = 0;
        private const int expectedDrainagePropertyIndex = 1;
        private const int expectedMinimumLevelPhreaticLineAtDikeTopRiverPropertyIndex = 2;
        private const int expectedMinimumLevelPhreaticLineAtDikeTopPolderPropertyIndex = 3;
        private const int expecteAdjustPhreaticLine3And4ForUpliftPropertyIndex = 4;
        private const int expectedLeakageLengthOutwardsPhreaticLine3PropertyIndex = 5;
        private const int expectedLeakageLengthInwardsPhreaticLine3PropertyIndex = 6;
        private const int expectedLeakageLengthOutwardsPhreaticLine4PropertyIndex = 7;
        private const int expectedLeakageLengthInwardsPhreaticLine4PropertyIndex = 8;
        private const int expectedPiezometricHeadPhreaticLine2OutwardsPropertyIndex = 9;
        private const int expectedPiezometricHeadPhreaticLine2InwardsPropertyIndex = 10;
        private const int locationExtremePropertyIndex = 11;
        private const int locationDailyPropertyIndex = 12;
        private const int waterStressLinesPropertyIndex = 13;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressesProperties(input,
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                              propertyChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsInput>>(properties);
            Assert.AreSame(input, properties.Data);

            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressesProperties.Drainage));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressesProperties.LocationDaily));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressesProperties.LocationExtreme));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressesProperties.WaterStressLines));

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsWaterStressesProperties(null,
                                                                                       AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                                       propertyChangeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            TestDelegate call = () => new MacroStabilityInwardsWaterStressesProperties(input,
                                                                                       AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("propertyChangeHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressesProperties(input,
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                              propertyChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(14, dynamicProperties.Count);

            const string waterStressesCategory = "Waterspanningen";

            PropertyDescriptor waterLevelRiverAverageProperty = dynamicProperties[expectedWaterLevelRiverAveragePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterLevelRiverAverageProperty,
                waterStressesCategory,
                "Gemiddeld hoog water (GHW) [m+NAP]",
                "Gemiddeld hoog water.");

            PropertyDescriptor drainageProperty = dynamicProperties[expectedDrainagePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainageProperty,
                waterStressesCategory,
                "Drainage",
                "Eigenschappen van drainage constructie.",
                true);

            PropertyDescriptor minimumLevelPhreaticLineAtDikeTopRiverProperty = dynamicProperties[expectedMinimumLevelPhreaticLineAtDikeTopRiverPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                minimumLevelPhreaticLineAtDikeTopRiverProperty,
                waterStressesCategory,
                "PL 1 initiële hoogte onder buitenkruin [m+NAP]",
                "Minimale hoogte van de freatische lijn onder kruin buitentalud.");

            PropertyDescriptor minimumLevelPhreaticLineAtDikeTopPolderProperty = dynamicProperties[expectedMinimumLevelPhreaticLineAtDikeTopPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                minimumLevelPhreaticLineAtDikeTopPolderProperty,
                waterStressesCategory,
                "PL 1 initiële hoogte onder binnenkruin [m+NAP]",
                "Minimale hoogte van de freatische lijn onder kruin binnentalud.");

            PropertyDescriptor adjustPhreaticLine3And4ForUpliftProperty = dynamicProperties[expecteAdjustPhreaticLine3And4ForUpliftPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                adjustPhreaticLine3And4ForUpliftProperty,
                waterStressesCategory,
                "Corrigeer PL 3 en PL 4 voor opbarsten",
                "Corrigeer de stijghoogte in watervoerende zandlaag en tussenzandlaag voor opbarsten?");

            PropertyDescriptor leakageLengthOutwardsPhreaticLine3Property = dynamicProperties[expectedLeakageLengthOutwardsPhreaticLine3PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                leakageLengthOutwardsPhreaticLine3Property,
                waterStressesCategory,
                "Leklengte buitenwaarts PL 3 [m]",
                "Lengtemaat die uitdrukking geeft aan de afstand waarover de stijghoogte verloopt in de diepe watervoerende zandlaag ter hoogte van het voorland.");

            PropertyDescriptor leakageLengthInwardsPhreaticLine3Property = dynamicProperties[expectedLeakageLengthInwardsPhreaticLine3PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                leakageLengthInwardsPhreaticLine3Property,
                waterStressesCategory,
                "Leklengte binnenwaarts PL 3 [m]",
                "Lengtemaat die uitdrukking geeft aan de afstand waarover de stijghoogte verloopt in de diepe watervoerende zandlaag ter hoogte van het achterland.");

            PropertyDescriptor leakageLengthOutwardsPhreaticLine4Property = dynamicProperties[expectedLeakageLengthOutwardsPhreaticLine4PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                leakageLengthOutwardsPhreaticLine4Property,
                waterStressesCategory,
                "Leklengte buitenwaarts PL 4 [m]",
                "Lengtemaat die uitdrukking geeft aan de afstand waarover de stijghoogte verloopt in de tussenzandlaag ter hoogte van het voorland.");

            PropertyDescriptor leakageLengthInwardsPhreaticLine4Property = dynamicProperties[expectedLeakageLengthInwardsPhreaticLine4PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                leakageLengthInwardsPhreaticLine4Property,
                waterStressesCategory,
                "Leklengte binnenwaarts PL 4 [m]",
                "Lengtemaat die uitdrukking geeft aan de afstand waarover de stijghoogte verloopt in de tussenzandlaag ter hoogte van het achterland.");

            PropertyDescriptor piezometricHeadPhreaticLine2OutwardsProperty = dynamicProperties[expectedPiezometricHeadPhreaticLine2OutwardsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                piezometricHeadPhreaticLine2OutwardsProperty,
                waterStressesCategory,
                "Stijghoogte PL 2 buitenwaarts [m+NAP]",
                "Stijghoogte in de indringingslaag buitenwaarts.");

            PropertyDescriptor piezometricHeadPhreaticLine2InwardsProperty = dynamicProperties[expectedPiezometricHeadPhreaticLine2InwardsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                piezometricHeadPhreaticLine2InwardsProperty,
                waterStressesCategory,
                "Stijghoogte PL 2 binnenwaarts [m+NAP]",
                "Stijghoogte in de indringingslaag binnenwaarts.");

            PropertyDescriptor locationExtremeProperty = dynamicProperties[locationExtremePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                locationExtremeProperty,
                waterStressesCategory,
                "Extreme omstandigheden",
                "Eigenschappen van de waterspanningen bij extreme omstandigheden.",
                true);

            PropertyDescriptor locationDailyProperty = dynamicProperties[locationDailyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                locationDailyProperty,
                waterStressesCategory,
                "Dagelijkse omstandigheden",
                "Eigenschappen van de waterspanningen bij dagelijkse omstandigheden.",
                true);

            PropertyDescriptor waterStressLinesProperty = dynamicProperties[waterStressLinesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterStressLinesProperty,
                waterStressesCategory,
                "Waterspanningslijnen",
                "Eigenschappen van de waterspanningslijnen.",
                true);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;

            // Call
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, assessmentLevel, propertyChangeHandler);

            // Assert
            Assert.AreEqual(input.WaterLevelRiverAverage, properties.WaterLevelRiverAverage);
            Assert.AreSame(input.LocationInputExtreme, properties.LocationExtreme.Data);
            Assert.AreSame(input.LocationInputDaily, properties.LocationDaily.Data);

            Assert.AreSame(input, properties.WaterStressLines.Data);

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                MacroStabilityInwardsWaternetProperties waternetProperties = properties.WaterStressLines.WaternetExtreme;

                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                Assert.AreEqual(assessmentLevel, calculatorFactory.LastCreatedWaternetCalculator.Input.AssessmentLevel);
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetCalculator.Output, (MacroStabilityInwardsWaternet) waternetProperties.Data);
            }

            Assert.AreSame(input, properties.Drainage.Data);
            Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopRiver, properties.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopPolder, properties.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(input.AdjustPhreaticLine3And4ForUplift, properties.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine3, properties.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine3, properties.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine4, properties.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine4, properties.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(input.PiezometricHeadPhreaticLine2Outwards, properties.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(input.PiezometricHeadPhreaticLine2Inwards, properties.PiezometricHeadPhreaticLine2Inwards);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var propertyChangeHandler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsWaterStressesProperties(input,
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                              propertyChangeHandler);

            var random = new Random(21);
            double waterLevelRiverAverage = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            double leakageLengthOutwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine4 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine4 = random.NextDouble();
            double piezometricHeadPhreaticLine2Outwards = random.NextDouble();
            double piezometricHeadPhreaticLine2Inwards = random.NextDouble();

            // When
            properties.WaterLevelRiverAverage = (RoundedDouble) waterLevelRiverAverage;
            properties.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopRiver;
            properties.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopPolder;
            properties.AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift;
            properties.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) leakageLengthOutwardsPhreaticLine3;
            properties.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) leakageLengthInwardsPhreaticLine3;
            properties.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) leakageLengthOutwardsPhreaticLine4;
            properties.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) leakageLengthInwardsPhreaticLine4;
            properties.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) piezometricHeadPhreaticLine2Outwards;
            properties.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) piezometricHeadPhreaticLine2Inwards;

            // Then
            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage,
                            input.WaterLevelRiverAverage.GetAccuracy());
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver,
                            input.MinimumLevelPhreaticLineAtDikeTopRiver.GetAccuracy());
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder,
                            input.MinimumLevelPhreaticLineAtDikeTopPolder.GetAccuracy());
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, input.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3,
                            input.LeakageLengthOutwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3,
                            input.LeakageLengthInwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4,
                            input.LeakageLengthOutwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4,
                            input.LeakageLengthInwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, input.PiezometricHeadPhreaticLine2Outwards,
                            input.PiezometricHeadPhreaticLine2Outwards.GetAccuracy());
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, input.PiezometricHeadPhreaticLine2Inwards,
                            input.PiezometricHeadPhreaticLine2Inwards.GetAccuracy());
        }

        [Test]
        public void WaterLevelRiverAverage_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.WaterLevelRiverAverage = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void MinimumLevelPhreaticLineAtDikeTopRiver_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void MinimumLevelPhreaticLineAtDikeTopPolder_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void AdjustPhreaticLine3And4ForUplift_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.AdjustPhreaticLine3And4ForUplift = true, calculation);
        }

        [Test]
        public void LeakageLengthOutwardsPhreaticLine3_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void LeakageLengthInwardsPhreaticLine3_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void LeakageLengthOutwardsPhreaticLine4_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void LeakageLengthInwardsPhreaticLine4_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void PiezometricHeadPhreaticLine2Outwards_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void PiezometricHeadPhreaticLine2Inwards_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var properties = new MacroStabilityInwardsWaterStressesProperties(input,
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                              propertyChangeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<MacroStabilityInwardsWaterStressesProperties> setProperty,
                                                                            MacroStabilityInwardsCalculation calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsInput input = calculation.InputParameters;

            var propertyChangeHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsWaterStressesProperties(input,
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel(),
                                                                              propertyChangeHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(propertyChangeHandler.Called);
            mocks.VerifyAll();
        }
    }
}