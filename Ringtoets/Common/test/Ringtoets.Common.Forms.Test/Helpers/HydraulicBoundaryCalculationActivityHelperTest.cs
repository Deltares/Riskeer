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
using System.IO;
using System.Linq;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class HydraulicBoundaryCalculationActivityHelperTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void CreateWaveHeightCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<ICalculationMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraulicBoundaryCalculationActivityHelper.CreateWaveHeightCalculationActivities(
                string.Empty,
                string.Empty,
                null,
                new Random(12).NextDouble(),
                messageProvider);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_MesageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicBoundaryCalculationActivityHelper.CreateWaveHeightCalculationActivities(
                string.Empty,
                string.Empty,
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                new Random(12).NextDouble(),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("messageProvider", paramName);
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_WithValidData_ReturnsExpectedActivity()
        {
            // Setup
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                Converged = true
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(calculator);
            
            var calculationMessageProvider = mocks.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);

            var messageProvider = mocks.Stub<ICalculationMessageProvider>();
            messageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            
            mocks.ReplayAll();
            
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName);

            // Call
            IEnumerable<WaveHeightCalculationActivity> activities = HydraulicBoundaryCalculationActivityHelper.CreateWaveHeightCalculationActivities(
                validFilePath,
                validPreprocessorDirectory,
                new []
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                },
                norm,
                messageProvider);

            // Assert
            WaveHeightCalculationActivity activity = activities.Single();
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => activity.Run();

                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(6, messages.Length);
                    Assert.AreEqual($"{activityDescription} is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
                });
                WaveHeightCalculationInput waveHeightCalculationInput = calculator.ReceivedInputs.Single();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), waveHeightCalculationInput.Beta);
            }

            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<ICalculationMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HydraulicBoundaryCalculationActivityHelper.CreateDesignWaterLevelCalculationActivities(
                string.Empty,
                string.Empty,
                null,
                new Random(12).NextDouble(),
                messageProvider);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_MesageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicBoundaryCalculationActivityHelper.CreateDesignWaterLevelCalculationActivities(
                string.Empty,
                string.Empty,
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                new Random(12).NextDouble(),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("messageProvider", paramName);
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_WithValidData_ReturnsExpectedActivity()
        {
            // Setup
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";
            const double norm = 1.0 / 30;

            var calculator = new TestDesignWaterLevelCalculator
            {
                Converged = true
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, validPreprocessorDirectory)).Return(calculator);
            
            var calculationMessageProvider = mocks.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);

            var messageProvider = mocks.Stub<ICalculationMessageProvider>();
            messageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            
            mocks.ReplayAll();
            
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName);

            // Call
            IEnumerable<DesignWaterLevelCalculationActivity> activities = HydraulicBoundaryCalculationActivityHelper.CreateDesignWaterLevelCalculationActivities(
                validFilePath,
                validPreprocessorDirectory,
                new []
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                },
                norm,
                messageProvider);

            // Assert
            DesignWaterLevelCalculationActivity activity = activities.Single();
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => activity.Run();

                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(6, messages.Length);
                    Assert.AreEqual($"{activityDescription} is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                    StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
                });
                AssessmentLevelCalculationInput assessmentLevelCalculationInput = calculator.ReceivedInputs.Single();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, assessmentLevelCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), assessmentLevelCalculationInput.Beta);
            }

            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }
    }
}