﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                   new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                                   assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(
                new GrassCoverErosionInwardsCalculation(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsGrassCoverErosionInwardsCalculationActivityWithParametersSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);

            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = CreateValidCalculation();

            // Call
            CalculatableActivity activity = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                         failureMechanism,
                                                                                                                         assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculationActivity>(activity);
            AssertGrassCoverErosionInwardsCalculationActivity(activity, calculation, assessmentSection.HydraulicBoundaryData);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                     new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                     null,
                                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                     new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsGrassCoverErosionInwardsCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);

            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation1 = CreateValidCalculation();
            GrassCoverErosionInwardsCalculation calculation2 = CreateValidCalculation();

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(GrassCoverErosionInwardsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertGrassCoverErosionInwardsCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertGrassCoverErosionInwardsCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsGrassCoverErosionInwardsCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);

            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation1 = CreateValidCalculation();
            GrassCoverErosionInwardsCalculation calculation2 = CreateValidCalculation();

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(GrassCoverErosionInwardsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertGrassCoverErosionInwardsCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertGrassCoverErosionInwardsCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
            mocks.VerifyAll();
        }

        private static GrassCoverErosionInwardsCalculation CreateValidCalculation()
        {
            return new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0),
                                                  new RoughnessPoint[0],
                                                  new Point2D[0],
                                                  new BreakWater(BreakWaterType.Dam, new Random(39).NextDouble()),
                                                  new DikeProfile.ConstructionProperties
                                                  {
                                                      Id = "id"
                                                  })
                }
            };
        }

        private static void AssertGrassCoverErosionInwardsCalculationActivity(Activity activity,
                                                                              GrassCoverErosionInwardsCalculation calculation,
                                                                              HydraulicBoundaryData hydraulicBoundaryData)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         hydraulicBoundaryData,
                                         calculation.InputParameters.HydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(testCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                Assert.AreEqual(calculation.InputParameters.BreakWater.Height, testCalculator.ReceivedInputs.Single().BreakWater.Height);
            }

            mocks.VerifyAll();
        }
    }
}