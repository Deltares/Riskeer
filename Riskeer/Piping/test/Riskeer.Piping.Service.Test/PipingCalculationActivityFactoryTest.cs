// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.KernelWrapper.SubCalculator;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;
using Riskeer.Piping.Service.SemiProbabilistic;

namespace Riskeer.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityFactoryTest
    {
        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(null,
                                                                                                                        new GeneralPipingInput(),
                                                                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Setup
            SemiProbabilisticPipingCalculation calculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<TestSemiProbabilisticPipingCalculation>();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(calculation,
                                                                                                                        null,
                                                                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            SemiProbabilisticPipingCalculation calculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<TestSemiProbabilisticPipingCalculation>();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(calculation,
                                                                                                                        new GeneralPipingInput(),
                                                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_WithValidCalculation_ReturnsActivityWithParametersSet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var random = new Random(39);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Single();
            hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            SemiProbabilisticPipingCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculatableActivity activity = PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(calculation,
                                                                                                                              generalPipingInput,
                                                                                                                              assessmentSection);

            // Assert
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity);
            AssertPipingCalculationActivity(activity, calculation, hydraulicBoundaryLocationCalculation);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                   new PipingFailureMechanism(),
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
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                   new PipingFailureMechanism(),
                                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsPipingCalculationActivitiesWithParametersSet()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            var pipingFailureMechanism = new PipingFailureMechanism();

            var random = new Random(39);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First();
            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(1);
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            SemiProbabilisticPipingCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation1);
            SemiProbabilisticPipingCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation2);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = PipingCalculationActivityFactory.CreateCalculationActivities(
                calculations, pipingFailureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(SemiProbabilisticPipingCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertPipingCalculationActivity(activities.First(), calculation1, hydraulicBoundaryLocationCalculation1);
            AssertPipingCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryLocationCalculation2);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationActivityFactory.CreateCalculationActivities(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsPipingCalculationActivitiesWithParametersSet()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };
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

            SemiProbabilisticPipingCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation1);
            SemiProbabilisticPipingCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation2);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = PipingCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(SemiProbabilisticPipingCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertPipingCalculationActivity(activities.First(), calculation1, hydraulicBoundaryLocationCalculation1);
            AssertPipingCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryLocationCalculation2);
        }

        private static SemiProbabilisticPipingCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            SemiProbabilisticPipingCalculation calculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(hydraulicBoundaryLocation);
            calculation.InputParameters.ExitPointL = new Random(39).NextRoundedDouble(0.5, 1.0);
            return calculation;
        }

        private static void AssertPipingCalculationActivity(Activity activity,
                                                            SemiProbabilisticPipingCalculation calculation,
                                                            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            using (new PipingSubCalculatorFactoryConfig())
            {
                activity.Run();

                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                Assert.AreEqual(calculation.InputParameters.ExitPointL, testFactory.LastCreatedEffectiveThicknessCalculator.ExitPointXCoordinate);
                Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, testFactory.LastCreatedSellmeijerCalculator.HRiver);
            }
        }
    }
}