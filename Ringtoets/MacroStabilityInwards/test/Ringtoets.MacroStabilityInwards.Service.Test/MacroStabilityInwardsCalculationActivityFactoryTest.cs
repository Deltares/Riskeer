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
using System.Linq;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationActivityFactoryTest
    {
        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivity(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();

            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivity(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsMacroStabilityInwardsCalculationActivityWithParametersSet()
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

            MacroStabilityInwardsCalculationScenario calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivity(calculation, assessmentSection);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsCalculationActivity>(activity);
            AssertMacroStabilityInwardsCalculationActivity(activity, calculation, hydraulicBoundaryLocationCalculation);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities((CalculationGroup) null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsMacroStabilityInwardsCalculationActivitiesWithParametersSet()
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

            MacroStabilityInwardsCalculationScenario calculation1 = CreateValidCalculation(hydraulicBoundaryLocation1);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateValidCalculation(hydraulicBoundaryLocation2);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(
                calculations, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(MacroStabilityInwardsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertMacroStabilityInwardsCalculationActivity(activities.First(), calculation1, hydraulicBoundaryLocationCalculation1);
            AssertMacroStabilityInwardsCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryLocationCalculation2);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities((MacroStabilityInwardsFailureMechanism) null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsMacroStabilityInwardsCalculationActivitiesWithParametersSet()
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

            MacroStabilityInwardsCalculationScenario calculation1 = CreateValidCalculation(hydraulicBoundaryLocation1);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateValidCalculation(hydraulicBoundaryLocation2);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(MacroStabilityInwardsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertMacroStabilityInwardsCalculationActivity(activities.First(), calculation1, hydraulicBoundaryLocationCalculation1);
            AssertMacroStabilityInwardsCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryLocationCalculation2);
        }

        private static MacroStabilityInwardsCalculationScenario CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
            calculation.InputParameters.LeakageLengthInwardsPhreaticLine3 = new Random(39).NextRoundedDouble();
            return calculation;
        }

        private static void AssertMacroStabilityInwardsCalculationActivity(Activity activity,
                                                                           MacroStabilityInwardsCalculationScenario calculation,
                                                                           HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                activity.Run();

                var testFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                UpliftVanCalculatorInput upliftVanCalculatorInput = testFactory.LastCreatedUpliftVanCalculator.Input;

                Assert.AreEqual(calculation.InputParameters.LeakageLengthInwardsPhreaticLine3, upliftVanCalculatorInput.LeakageLengthInwardsPhreaticLine3);
                Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, upliftVanCalculatorInput.AssessmentLevel);
            }
        }
    }
}