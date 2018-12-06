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
using Ringtoets.HydraRing.Calculation.Data.Input;
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

                calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestOvertoppingCalculator());

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, ""))
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(9);

                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, ""))
                                 .Return(new TestDesignWaterLevelCalculator()).Repeat.Times(3);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, ""))
                                 .Return(new TestWaveHeightCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, ""))
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
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
        [TestCaseSource(nameof(GetFailureMechanismTestCases))]
        public void CreateActivities_FailureMechanismNotIrrelevant_NoActivitiesCreated(
            Action<AssessmentSection> setFailureMechanismIrrelevantAction,
            Action<AssessmentSection> addValidCalculationToFailureMechanismAction)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            setFailureMechanismIrrelevantAction(assessmentSection);

            addValidCalculationToFailureMechanismAction(assessmentSection);

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionCalculationActivityFactory.CreateActivities(assessmentSection);

            // Assert
            CollectionAssert.IsEmpty(activities);
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismTestCases()
        {
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.Piping.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddPipingCalculationScenario(section, new TestHydraulicBoundaryLocation())))
                .SetName("Piping");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.GrassCoverErosionInwards.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddGrassCoverErosionInwardsCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("GrassCoverErosionInwards");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.MacroStabilityInwards.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddMacroStabilityInwardsCalculationScenario(section, new TestHydraulicBoundaryLocation())))
                .SetName("MacroStabilityInwards");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.StabilityStoneCover.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddStabilityStoneCoverCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("StabilityStoneCover");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.WaveImpactAsphaltCover.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddWaveImpactAsphaltCoverCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("WaveImpactAsphaltCover");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.GrassCoverErosionOutwards.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddGrassCoverErosionOutwardsCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("GrassCoverErosionOutwards");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.HeightStructures.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddHeightStructuresCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("HeightStructures");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.ClosingStructures.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddClosingStructuresCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("ClosingStructures");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.StabilityPointStructures.IsRelevant = false),
                                          new Action<AssessmentSection>(section => AddStabilityPointStructuresCalculation(section, new TestHydraulicBoundaryLocation())))
                .SetName("StabilityPointStructures");
            yield return new TestCaseData(new Action<AssessmentSection>(section => section.DuneErosion.IsRelevant = false),
                                          new Action<AssessmentSection>(AddDuneLocationCalculation))
                .SetName("DuneErosion");
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            return new AssessmentSection(AssessmentSectionComposition.DikeAndDune)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };
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
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
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
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
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