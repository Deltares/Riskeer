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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class AssessmentSectionCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void CreateActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionCalculationActivityFactory.CreateActivities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateActivities_WithValidDataAndAllFailureMechanismsRelevant_ExpectedActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                hydraulicBoundaryLocation
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            AddGrassCoverErosionInwardsCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddPipingCalculationScenario(assessmentSection, hydraulicBoundaryLocation);
            AddMacroStabilityInwardsCalculationScenario(assessmentSection, hydraulicBoundaryLocation);
            AddStabilityStoneCoverCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddWaveImpactAsphaltCoverCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddGrassCoverErosionOutwardsCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddHeightStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddClosingStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddStabilityPointStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddDuneLocationCalculation(assessmentSection);

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();

            using (mocks.Ordered())
            {
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, ""))
                                 .Return(new TestDesignWaterLevelCalculator
                                 {
                                     DesignWaterLevel = 2.0
                                 }).Repeat.Times(4);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, ""))
                                 .Return(new TestWaveHeightCalculator()).Repeat.Times(4);

                calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, ""))
                                 .Return(new TestOvertoppingCalculator());

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, ""))
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(9);

                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, ""))
                                 .Return(new TestDesignWaterLevelCalculator()).Repeat.Times(3);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, ""))
                                 .Return(new TestWaveHeightCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, ""))
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, ""))
                                 .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, ""))
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, ""))
                                 .Return(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, ""))
                                 .Return(new TestDunesBoundaryConditionsCalculator()).Repeat.Times(5);
            }

            mocks.ReplayAll();

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(28, activities.Count());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            using (new PipingSubCalculatorFactoryConfig())
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                // Run hydraulic boundary location calculations first
                activities.Take(8).ForEachElementDo(activity => activity.Run());

                var pipingTestFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                var macroStabilityTestFactory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                Assert.IsFalse(pipingTestFactory.LastCreatedUpliftCalculator.Calculated);
                Assert.IsFalse(macroStabilityTestFactory.LastCreatedUpliftVanKernel.Calculated);

                activities.Skip(8).ForEachElementDo(activity => activity.Run());

                Assert.IsTrue(pipingTestFactory.LastCreatedUpliftCalculator.Calculated);
                Assert.IsTrue(macroStabilityTestFactory.LastCreatedUpliftVanKernel.Calculated);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateActivities_PipingNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.Piping.IsRelevant = false;

            AddPipingCalculationScenario(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_GrassCoverErosionInwardsNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.GrassCoverErosionInwards.IsRelevant = false;

            AddGrassCoverErosionInwardsCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_MacroStabilityInwardsNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.MacroStabilityInwards.IsRelevant = false;

            AddMacroStabilityInwardsCalculationScenario(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_StabilityStoneCoverNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.StabilityStoneCover.IsRelevant = false;

            AddStabilityStoneCoverCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_WaveImpactAsphaltCoverNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.WaveImpactAsphaltCover.IsRelevant = false;

            AddWaveImpactAsphaltCoverCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_GrassCoverErosionOutwardsNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.GrassCoverErosionOutwards.IsRelevant = false;

            AddGrassCoverErosionOutwardsCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_HeightStructuresNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.HeightStructures.IsRelevant = false;

            AddHeightStructuresCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_ClosingStructuresNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.ClosingStructures.IsRelevant = false;

            AddClosingStructuresCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_StabilityPointStructuresNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.StabilityPointStructures.IsRelevant = false;

            AddStabilityPointStructuresCalculation(assessmentSection, new TestHydraulicBoundaryLocation());

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        [Test]
        public void CreateActivities_DuneErosionNotRelevant_NoActivitiesCreated()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.DuneErosion.IsRelevant = false;

            AddDuneLocationCalculation(assessmentSection);

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            Assert.AreEqual(0, activities.Count());
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.DikeAndDune);

            assessmentSection.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            return assessmentSection;
        }

        private static void AddGrassCoverErosionInwardsCalculation(AssessmentSection assessmentSection,
                                                                   HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });
        }

        private static void AddPipingCalculationScenario(AssessmentSection assessmentSection,
                                                         HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
            pipingCalculationScenario.InputParameters.UseAssessmentLevelManualInput = true;
            pipingCalculationScenario.InputParameters.AssessmentLevel = new Random(39).NextRoundedDouble();
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculationScenario);
        }

        private static void AddMacroStabilityInwardsCalculationScenario(AssessmentSection assessmentSection,
                                                                        HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
            macroStabilityInwardsCalculationScenario.InputParameters.UseAssessmentLevelManualInput = true;
            macroStabilityInwardsCalculationScenario.InputParameters.AssessmentLevel = new Random(39).NextRoundedDouble();
            assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(macroStabilityInwardsCalculationScenario);
        }

        private static void AddStabilityStoneCoverCalculation(AssessmentSection assessmentSection,
                                                              HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3
                }
            });
        }

        private static void AddWaveImpactAsphaltCoverCalculation(AssessmentSection assessmentSection,
                                                                 HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3
                }
            });
        }

        private static void AddGrassCoverErosionOutwardsCalculation(AssessmentSection assessmentSection,
                                                                    HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
                }
            });
        }

        private static void AddHeightStructuresCalculation(AssessmentSection assessmentSection,
                                                           HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddClosingStructuresCalculation(AssessmentSection assessmentSection,
                                                            HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddStabilityPointStructuresCalculation(AssessmentSection assessmentSection,
                                                                   HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddDuneLocationCalculation(AssessmentSection assessmentSection)
        {
            var duneLocation = new TestDuneLocation();
            assessmentSection.DuneErosion.SetDuneLocations(new[]
            {
                duneLocation
            });
        }
    }
}