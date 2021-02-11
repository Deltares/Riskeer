// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.KernelWrapper.SubCalculator;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;
using Riskeer.Piping.Service.Probabilistic;
using Riskeer.Piping.Service.SemiProbabilistic;

namespace Riskeer.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        #region CreateCalculationActivitiesForFailureMechanism

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsPipingCalculationActivitiesWithParametersSet()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var random = new Random(39);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First();
            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(1);
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            var calculation1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation1);
            var calculation2 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation1);
            var calculation3 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation2);
            var calculation4 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation2);

            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculation<PipingInput>[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4
            });

            // Call
            IEnumerable<CalculatableActivity> activities = PipingCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(4, activities.Count());

            CalculatableActivity activity1 = activities.ElementAt(0);
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity1);
            AssertSemiProbabilisticPipingCalculationActivity(activity1, calculation1, hydraulicBoundaryLocationCalculation1);

            CalculatableActivity activity2 = activities.ElementAt(1);
            Assert.IsInstanceOf<ProbabilisticPipingCalculationActivity>(activity2);
            AssertProbabilisticPipingCalculationActivity(activity2, calculation2, hydraulicBoundaryLocation1);

            CalculatableActivity activity3 = activities.ElementAt(2);
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity3);
            AssertSemiProbabilisticPipingCalculationActivity(activity3, calculation3, hydraulicBoundaryLocationCalculation2);

            CalculatableActivity activity4 = activities.ElementAt(3);
            Assert.IsInstanceOf<ProbabilisticPipingCalculationActivity>(activity4);
            AssertProbabilisticPipingCalculationActivity(activity4, calculation4, hydraulicBoundaryLocation2);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_UnsupportedCalculationType_ThrowsNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestPipingCalculation());

            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
            mocks.VerifyAll();
        }

        #endregion

        #region CreateCalculationActivitiesForCalculationGroup

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                        new PipingFailureMechanism(),
                                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                        null,
                                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                        new PipingFailureMechanism(),
                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsPipingCalculationActivitiesWithParametersSet()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var random = new Random(39);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First();
            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(1);
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            var calculation1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation1);
            var calculation2 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation1);
            var calculation3 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation2);
            var calculation4 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation2);

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2,
                    calculation3,
                    calculation4
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = PipingCalculationActivityFactory.CreateCalculationActivities(
                calculationGroup, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(4, activities.Count());

            CalculatableActivity activity1 = activities.ElementAt(0);
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity1);
            AssertSemiProbabilisticPipingCalculationActivity(activity1, calculation1, hydraulicBoundaryLocationCalculation1);

            CalculatableActivity activity2 = activities.ElementAt(1);
            Assert.IsInstanceOf<ProbabilisticPipingCalculationActivity>(activity2);
            AssertProbabilisticPipingCalculationActivity(activity2, calculation2, hydraulicBoundaryLocation1);

            CalculatableActivity activity3 = activities.ElementAt(2);
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity3);
            AssertSemiProbabilisticPipingCalculationActivity(activity3, calculation3, hydraulicBoundaryLocationCalculation2);

            CalculatableActivity activity4 = activities.ElementAt(3);
            Assert.IsInstanceOf<ProbabilisticPipingCalculationActivity>(activity4);
            AssertProbabilisticPipingCalculationActivity(activity4, calculation4, hydraulicBoundaryLocation2);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_UnsupportedCalculationType_ThrowsNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    new TestPipingCalculation()
                }
            };

            // Call
            void Call() => PipingCalculationActivityFactory.CreateCalculationActivities(
                calculationGroup, new PipingFailureMechanism(), assessmentSection);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
            mocks.VerifyAll();
        }

        #endregion

        #region CreateSemiProbabilisticPipingCalculationActivity

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(null,
                                                                                                             new GeneralPipingInput(),
                                                                                                             assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(new TestSemiProbabilisticPipingCalculation(),
                                                                                                             null,
                                                                                                             assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateSemiProbabilisticPipingCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(new TestSemiProbabilisticPipingCalculation(),
                                                                                                             new GeneralPipingInput(),
                                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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

            var calculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    hydraulicBoundaryLocation);

            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculatableActivity activity = PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(calculation,
                                                                                                                              generalPipingInput,
                                                                                                                              assessmentSection);

            // Assert
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculationActivity>(activity);
            AssertSemiProbabilisticPipingCalculationActivity(activity, calculation, hydraulicBoundaryLocationCalculation);
        }

        #endregion

        #region CreateProbabilisticPipingCalculationActivity

        [Test]
        public void CreateProbabilisticPipingCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(null,
                                                                                                         new PipingFailureMechanism(),
                                                                                                         assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateProbabilisticPipingCalculationActivity_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(new TestProbabilisticPipingCalculation(),
                                                                                                         null,
                                                                                                         assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateProbabilisticPipingCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(new TestProbabilisticPipingCalculation(),
                                                                                                         new PipingFailureMechanism(),
                                                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateProbabilisticPipingCalculationActivity_WithValidCalculation_ReturnsActivityWithParametersSet()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(calculation,
                                                                                                                          failureMechanism,
                                                                                                                          assessmentSection);

            // Assert
            Assert.IsInstanceOf<ProbabilisticPipingCalculationActivity>(activity);
            AssertProbabilisticPipingCalculationActivity(activity, calculation, hydraulicBoundaryLocation);
        }

        #endregion

        #region Assert

        private static void AssertSemiProbabilisticPipingCalculationActivity(Activity activity,
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

        private static void AssertProbabilisticPipingCalculationActivity(Activity activity,
                                                                         ProbabilisticPipingCalculation calculation,
                                                                         HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                PipingCalculationInput[] profileSpecificInputs = profileSpecificCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, profileSpecificInputs.Length);
                Assert.AreEqual(hydraulicBoundaryLocation.Id, profileSpecificInputs[0].HydraulicBoundaryLocationId);
                Assert.AreEqual(DerivedPipingInput.GetSeepageLength(calculation.InputParameters).Mean,
                                profileSpecificInputs[0].Variables.First(v => v.VariableId == 48).Parameter1);

                PipingCalculationInput[] sectionSpecificInputs = sectionSpecificCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, sectionSpecificInputs.Length);
                Assert.AreEqual(hydraulicBoundaryLocation.Id, sectionSpecificInputs[0].HydraulicBoundaryLocationId);
                Assert.AreEqual(DerivedPipingInput.GetSeepageLength(calculation.InputParameters).Mean,
                                sectionSpecificInputs[0].Variables.First(v => v.VariableId == 48).Parameter1);
            }

            mocks.VerifyAll();
        }

        #endregion
    }
}