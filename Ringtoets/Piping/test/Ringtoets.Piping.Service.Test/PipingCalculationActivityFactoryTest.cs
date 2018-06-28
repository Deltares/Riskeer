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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityFactoryTest
    {
        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivity(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithInvalidInput();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivity(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsPipingCalculationActivityWithCorrectAssessmentLevelSet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Single();
            hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new Random(39).NextDouble());

            // Call
            CalculatableActivity activity = PipingCalculationActivityFactory.CreateCalculationActivity(
                PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation),
                assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingCalculationActivity>(activity);

            AssertPipingCalculationActivityAssessmentLevel(activity, hydraulicBoundaryLocationCalculation);
        }

        private static void AssertPipingCalculationActivityAssessmentLevel(CalculatableActivity activity, HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            using (new PipingSubCalculatorFactoryConfig())
            {
                activity.Run();

                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, testFactory.LastCreatedSellmeijerCalculator.HRiver);
            }
        }

        [Test]
        public void CreateCalculationActivities_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivities_WithValidCalculations_ReturnsPipingCalculationActivitiesWithCorrectAssessmentLevelsSet()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            var random = new Random(39);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First();
            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(1);
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            var calculations = new CalculationGroup
            {
                Children =
                {
                    PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation1),
                    PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation2)
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = PipingCalculationActivityFactory.CreateCalculationActivities(
                calculations, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(PipingCalculationActivity));

            AssertPipingCalculationActivityAssessmentLevel(activities.First(), hydraulicBoundaryLocationCalculation1);
            AssertPipingCalculationActivityAssessmentLevel(activities.ElementAt(1), hydraulicBoundaryLocationCalculation2);
        }
    }
}