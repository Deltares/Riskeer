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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Service.Test
{
    [TestFixture]
    public class RiskeerDataSynchronizationServiceTest
    {
        [Test]
        public void ClearFailureMechanismCalculationOutputs_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearFailureMechanismCalculationOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var expectedAffectedItems = new List<IObservable>();

            expectedAffectedItems.AddRange(assessmentSection.ClosingStructures.Calculations
                                                            .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionInwards.Calculations
                                                            .Cast<GrassCoverErosionInwardsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                            .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.HeightStructures.Calculations
                                                            .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
                                                            .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                            .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
                                                            .OfType<ProbabilisticPipingCalculationScenario>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityPointStructures.Calculations
                                                            .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                            .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                            .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                            .Cast<MacroStabilityInwardsCalculation>()
                                                            .Where(c => c.HasOutput));

            // Call
            IEnumerable<IObservable> affectedItems = RiskeerDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(assessmentSection.ClosingStructures.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionInwards.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.HeightStructures.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.Piping.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>()
                                           .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.Piping.Calculations.OfType<ProbabilisticPipingCalculationScenario>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityPointStructures.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.MacroStabilityInwards.Calculations.Cast<MacroStabilityInwardsCalculation>()
                                           .All(c => !c.InputParameters.UseAssessmentLevelManualInput && !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearAllSemiProbabilisticCalculationOutput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllSemiProbabilisticCalculationOutput_VariousCalculations_ClearsCalculationsOutputAndReturnsAffectedObjects()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = true
                    },
                    Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                },
                new SemiProbabilisticPipingCalculationScenario(),
                new ProbabilisticPipingCalculationScenario(),
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                }
            });

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();
            macroStabilityInwardsFailureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario(),
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = true
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                }
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                pipingFailureMechanism,
                macroStabilityInwardsFailureMechanism
            });
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationScenario[] expectedAffectedMacroStabilityInwardsCalculations = macroStabilityInwardsFailureMechanism.Calculations
                                                                                                                                                .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                                                                .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput)
                                                                                                                                                .ToArray();
            SemiProbabilisticPipingCalculationScenario[] expectedAffectedPipingCalculations = pipingFailureMechanism.Calculations
                                                                                                                    .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                                                    .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput)
                                                                                                                    .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = RiskeerDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutput(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(macroStabilityInwardsFailureMechanism.Calculations
                                                               .OfType<MacroStabilityInwardsCalculationScenario>()
                                                               .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                                               .All(c => !c.HasOutput));
            Assert.IsTrue(pipingFailureMechanism.Calculations
                                                .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                                .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedPipingCalculations.Concat<ICalculationScenario>(expectedAffectedMacroStabilityInwardsCalculations),
                                           affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_VariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var expectedAffectedItems = new List<IObservable>();
            expectedAffectedItems.AddRange(assessmentSection.ClosingStructures.Calculations
                                                            .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.ClosingStructures.Calculations
                                                            .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionInwards.Calculations
                                                            .Cast<GrassCoverErosionInwardsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionInwards.Calculations
                                                            .Cast<GrassCoverErosionInwardsCalculation>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                            .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                            .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.HeightStructures.Calculations
                                                            .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.HeightStructures.Calculations
                                                            .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
                                                            .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                            .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
                                                            .OfType<ProbabilisticPipingCalculationScenario>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
                                                            .Cast<IPipingCalculationScenario<PipingInput>>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.StabilityPointStructures.Calculations
                                                            .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityPointStructures.Calculations
                                                            .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                            .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                            .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                            .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                            .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));
            expectedAffectedItems.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                            .Cast<MacroStabilityInwardsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                            .Cast<MacroStabilityInwardsCalculation>()
                                                            .Select(c => c.InputParameters)
                                                            .Where(i => i.HydraulicBoundaryLocation != null));

            // Call
            IEnumerable<IObservable> affectedItems = RiskeerDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(assessmentSection.ClosingStructures.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionInwards.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.HeightStructures.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.Piping.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>()
                                           .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.Piping.Calculations.OfType<ProbabilisticPipingCalculationScenario>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.Piping.Calculations.Cast<IPipingCalculationScenario<PipingInput>>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null));
            Assert.IsTrue(assessmentSection.StabilityPointStructures.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.MacroStabilityInwards.Calculations.Cast<MacroStabilityInwardsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput((IAssessmentSection) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_HydraulicBoundaryAndDuneLocations_ClearDataAndReturnAffectedObjects()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            var duneLocation1 = new TestDuneLocation();
            var duneLocation2 = new TestDuneLocation();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                }
            };

            var waterLevelCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            var waveHeightCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                waterLevelCalculationsForTargetProbability
            });

            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                waveHeightCalculationsForTargetProbability
            });

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;

            var duneLocationCalculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability(0.1);
            var duneLocationCalculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability(0.01);

            duneErosionFailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                duneLocationCalculationsForTargetProbability1,
                duneLocationCalculationsForTargetProbability2
            });

            duneErosionFailureMechanism.SetDuneLocations(new[]
            {
                duneLocation1,
                duneLocation2
            });

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForSignalingNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation3 = waterLevelCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                                                                   .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation4 = waveHeightCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                                                                   .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));

            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation3.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation4.Output = new TestHydraulicBoundaryLocationCalculationOutput();

            DuneLocationCalculation duneLocationCalculation1 = duneLocationCalculationsForTargetProbability1.DuneLocationCalculations.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));
            DuneLocationCalculation duneLocationCalculation2 = duneLocationCalculationsForTargetProbability2.DuneLocationCalculations.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));

            duneLocationCalculation1.Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculation2.Output = new TestDuneLocationCalculationOutput();

            var expectedAffectedItems = new IObservable[]
            {
                hydraulicBoundaryLocationCalculation1,
                hydraulicBoundaryLocationCalculation2,
                hydraulicBoundaryLocationCalculation3,
                hydraulicBoundaryLocationCalculation4,
                duneLocationCalculation1,
                duneLocationCalculation2
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);

            Assert.IsFalse(hydraulicBoundaryLocationCalculation1.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation2.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation3.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation4.HasOutput);

            Assert.IsNull(duneLocationCalculation1.Output);
            Assert.IsNull(duneLocationCalculation2.Output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_HydraulicBoundaryLocationCalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput((HydraulicBoundaryLocationCalculationsForTargetProbability) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_CalculationsForTargetProbabilityWithAndWithoutOutput_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var random = new Random(21);

            var calculationWithOutput1 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };

            var calculationWithOutput2 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                    calculationWithOutput1,
                    new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                    calculationWithOutput2,
                    new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                }
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculationsForTargetProbability);

            // Assert
            Assert.IsTrue(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.All(c => c.Output == null));
            CollectionAssert.AreEqual(new[]
            {
                calculationWithOutput1,
                calculationWithOutput2
            }, affectedCalculations);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null, NormType.LowerLimit);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_InvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(new AssessmentSectionStub(), normType);

            // Assert
            var expectedMessage = $"The value of argument 'normType' ({normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_WithData_ClearsOutputAndReturnsAffectedObjects()
        {
            // Setup
            const NormType normType = NormType.LowerLimit;
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            waveConditionsCalculations.ForEachElementDo(c => c.InputParameters.WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit);

            IEnumerable<ICalculation<WaveConditionsInput>> expectedAffectedItems = waveConditionsCalculations.Where(c => c.HasOutput)
                                                                                                             .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(assessmentSection, normType);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithHydraulicBoundaryLocationCalculationsForTargetProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null, new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithHydraulicBoundaryLocationCalculationsForTargetProbability_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithHydraulicBoundaryLocationCalculationsForTargetProbability_WithData_ClearsOutputAndReturnsAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            waveConditionsCalculations.ForEachElementDo(c =>
            {
                c.InputParameters.WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability;
                c.InputParameters.CalculationsTargetProbability = calculationsForTargetProbability;
            });

            IEnumerable<ICalculation<WaveConditionsInput>> expectedAffectedItems = waveConditionsCalculations.Where(c => c.HasOutput)
                                                                                                             .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(assessmentSection, calculationsForTargetProbability);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(assessmentSection.GrassCoverErosionOutwards.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities_AssessmentSectionWithCalculations_ClearsIllustrationPointsAndReturnsAffectedObjects()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForNormTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForNormTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                          .ToArray();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities(assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities_AssessmentSectionWithCalculations_ClearsIllustrationPointsAndReturnsAffectedObjects()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForNormTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForNormTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                          .ToArray();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities(assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities_AssessmentSectionWithCalculations_ClearsIllustrationPointsAndReturnsAffectedObjects()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForNormTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForNormTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                          .ToArray();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities(assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations_AssessmentSectionWithCalculations_ClearsIllustrationPointsAndReturnsAffectedObjects()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForNormTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForNormTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                          .ToArray();

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput =
                GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints =
                waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                 .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations(assessmentSection);

            // Assert
            HydraulicBoundaryLocationCalculation[] calculationsWithIllustrationPoints =
                waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints
                    .Concat(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints)
                    .Concat(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints)
                    .ToArray();

            CollectionAssert.AreEquivalent(calculationsWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForNormTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearReferenceLine_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_AllReferenceLineDependentDataCleared()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            // Call
            RiskeerDataSynchronizationService.ClearReferenceLineDependentData(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            CollectionAssert.IsEmpty(pipingFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(pipingFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.SectionResults);

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.SurfaceLines);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            CollectionAssert.IsEmpty(macroStabilityOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macroStabilityOutwardsFailureMechanism.SectionResults);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.Sections);
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.SectionResults);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.SectionResults);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.SectionResults);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.SectionResults);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.SectionResults);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.SectionResults);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.Sections);
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.SectionResults);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_ClearResultsContainAllAffectedObjectsAndAllRemovedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            IEnumerable<object> expectedRemovedObjects = GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection);

            // Call
            ClearResults results = RiskeerDataSynchronizationService.ClearReferenceLineDependentData(assessmentSection);

            // Assert
            AssertChangedObjects(results, assessmentSection);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
            CollectionAssert.DoesNotContain(results.RemovedObjects, null);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((StabilityStoneCoverFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityStoneCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = TestDataGenerator.GetStabilityStoneCoverFailureMechanismWithAllCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StabilityStoneCoverWaveConditionsCalculation[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            StabilityStoneCoverWaveConditionsCalculation[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((WaveImpactAsphaltCoverFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredWaveImpactAsphaltCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = TestDataGenerator.GetWaveImpactAsphaltCoverFailureMechanismWithAllCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((GrassCoverErosionOutwardsFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredGrassCoverErosionOutwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = TestDataGenerator.GetGrassCoverErosionOutwardsFailureMechanismWithAllCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((HeightStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredHeightStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = TestDataGenerator.GetHeightStructuresFailureMechanismWithAlLCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<HeightStructuresInput>[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<HeightStructuresInput>>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            StructuresCalculation<HeightStructuresInput>[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((ClosingStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredClosingStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = TestDataGenerator.GetClosingStructuresFailureMechanismWithAllCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<ClosingStructuresInput>[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            StructuresCalculation<ClosingStructuresInput>[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile((StabilityPointStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityPointStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = TestDataGenerator.GetStabilityPointStructuresFailureMechanismWithAllCalculationConfigurations();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
            Assert.AreEqual(expectedAffectedObjectCount, array.Length);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (ICalculation calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
                CollectionAssert.Contains(array, calculation);
            }
        }

        [Test]
        public void RemoveAllForeshoreProfiles_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveAllForeshoreProfiles<ICalculationInput>(null, new ForeshoreProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void RemoveAllForeshoreProfiles_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveAllForeshoreProfiles(Enumerable.Empty<ICalculation<ICalculationInput>>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void RemoveAllForeshoreProfiles_FullyConfiguredClosingStructuresFailureMechanism_RemovesAllForeshoreProfilesAndDependentData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            TestCalculationWithForeshoreProfile calculationWithForeshoreProfileAndOutput =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(foreshoreProfile);
            TestCalculationWithForeshoreProfile calculationWithForeshoreProfile =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(foreshoreProfile);
            TestCalculationWithForeshoreProfile calculationWithoutForeshoreProfile =
                TestCalculationWithForeshoreProfile.CreateDefaultCalculation();

            var calculations = new List<ICalculation<ICalculationInput>>
            {
                calculationWithForeshoreProfileAndOutput,
                calculationWithForeshoreProfile,
                calculationWithoutForeshoreProfile
            };

            TestCalculationWithForeshoreProfile[] calculationsWithForeshoreProfile =
            {
                calculationWithForeshoreProfileAndOutput,
                calculationWithForeshoreProfile
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.RemoveAllForeshoreProfiles(calculations, foreshoreProfiles);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(foreshoreProfiles);
            Assert.IsFalse(calculationWithForeshoreProfileAndOutput.HasOutput);
            Assert.IsTrue(calculationsWithForeshoreProfile.All(calc => calc.InputParameters.ForeshoreProfile == null));

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfile.Select(calc => calc.InputParameters)
                                                .Concat(new IObservable[]
                                                {
                                                    foreshoreProfiles,
                                                    calculationWithForeshoreProfileAndOutput
                                                });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            DikeProfile profile = DikeProfileTestFactory.CreateDikeProfile();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveDikeProfile(null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => RiskeerDataSynchronizationService.RemoveDikeProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveDikeProfile_FullyConfiguredGrassCoverErosionInwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = TestDataGenerator.GetGrassCoverErosionInwardsFailureMechanismWithAllCalculationConfigurations();
            DikeProfile profile = failureMechanism.DikeProfiles[0];
            GrassCoverErosionInwardsCalculation[] calculations = failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                                                                 .Where(c => ReferenceEquals(c.InputParameters.DikeProfile, profile))
                                                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = RiskeerDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, profile);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.DikeProfile);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + (calculations.Length * 2), array.Length);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation);
                CollectionAssert.Contains(array, calculation.InputParameters);
                Assert.IsFalse(calculation.HasOutput);
            }
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetWaterLevelCalculationsForNormTargetProbabilitiesWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForSignalingNorm
                                    .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                    .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetWaterLevelCalculationsForUserDefinedTargetProbabilitiesWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                    .SelectMany(wlc => wlc.HydraulicBoundaryLocationCalculations)
                                    .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetWaveHeightCalculationsForUserDefinedTargetProbabilitiesWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                    .SelectMany(whc => whc.HydraulicBoundaryLocationCalculations)
                                    .Where(calc => calc.HasOutput);
        }

        private static void AssertChangedObjects(ClearResults results, AssessmentSection assessmentSection)
        {
            IObservable[] changedObjects = results.ChangedObjects.ToArray();
            Assert.AreEqual(59, changedObjects.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.Contains(changedObjects, duneErosionFailureMechanism);
            CollectionAssert.Contains(changedObjects, duneErosionFailureMechanism.SectionResults);

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            CollectionAssert.Contains(changedObjects, macroStabilityInwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, macroStabilityInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(changedObjects, macroStabilityInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, macroStabilityInwardsFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(changedObjects, macroStabilityInwardsFailureMechanism.SurfaceLines);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            CollectionAssert.Contains(changedObjects, macroStabilityOutwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, macroStabilityOutwardsFailureMechanism.SectionResults);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.Contains(changedObjects, microstabilityFailureMechanism);
            CollectionAssert.Contains(changedObjects, microstabilityFailureMechanism.SectionResults);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.Contains(changedObjects, waterPressureAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, waterPressureAsphaltCoverFailureMechanism.SectionResults);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffOutwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffOutwardsFailureMechanism.SectionResults);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffInwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffInwardsFailureMechanism.SectionResults);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.Contains(changedObjects, stabilityLengthwiseConstructionFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityLengthwiseConstructionFailureMechanism.SectionResults);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.Contains(changedObjects, pipingStructureFailureMechanism);
            CollectionAssert.Contains(changedObjects, pipingStructureFailureMechanism.SectionResults);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.Contains(changedObjects, technicalInnovationFailureMechanism);
            CollectionAssert.Contains(changedObjects, technicalInnovationFailureMechanism.SectionResults);
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(AssessmentSection assessmentSection)
        {
            var expectedRemovedObjects = new List<object>();
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.Piping));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.GrassCoverErosionInwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.MacroStabilityInwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.MacroStabilityOutwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.Microstability));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.StabilityStoneCover));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.WaveImpactAsphaltCover));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.WaterPressureAsphaltCover));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.GrassCoverErosionOutwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.GrassCoverSlipOffOutwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.GrassCoverSlipOffInwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.HeightStructures));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.ClosingStructures));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.PipingStructure));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.StabilityPointStructures));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.StrengthStabilityLengthwiseConstruction));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.DuneErosion));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.TechnicalInnovation));
            return expectedRemovedObjects;
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(PipingFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<PipingFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (PipingStochasticSoilModel stochasticSoilModel in failureMechanism.StochasticSoilModels)
            {
                yield return stochasticSoilModel;
            }

            foreach (PipingSurfaceLine surfaceLine in failureMechanism.SurfaceLines)
            {
                yield return surfaceLine;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<MacroStabilityInwardsFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (MacroStabilityInwardsStochasticSoilModel stochasticSoilModel in failureMechanism.StochasticSoilModels)
            {
                yield return stochasticSoilModel;
            }

            foreach (MacroStabilityInwardsSurfaceLine surfaceLine in failureMechanism.SurfaceLines)
            {
                yield return surfaceLine;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<GrassCoverErosionInwardsFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (DikeProfile profile in failureMechanism.DikeProfiles)
            {
                yield return profile;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<StabilityStoneCoverFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<WaveImpactAsphaltCoverFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<GrassCoverErosionOutwardsFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(HeightStructuresFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<HeightStructuresFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }

            foreach (HeightStructure structure in failureMechanism.HeightStructures)
            {
                yield return structure;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(ClosingStructuresFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<ClosingStructuresFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }

            foreach (ClosingStructure structure in failureMechanism.ClosingStructures)
            {
                yield return structure;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<StabilityPointStructuresFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }

            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }

            foreach (ForeshoreProfile profile in failureMechanism.ForeshoreProfiles)
            {
                yield return profile;
            }

            foreach (StabilityPointStructure structure in failureMechanism.StabilityPointStructures)
            {
                yield return structure;
            }
        }

        private static IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine<T>(T failureMechanism)
            where T : IFailureMechanism, IHasSectionResults<FailureMechanismSectionResult>
        {
            foreach (FailureMechanismSection section in failureMechanism.Sections)
            {
                yield return section;
            }

            foreach (FailureMechanismSectionResult sectionResult in failureMechanism.SectionResults)
            {
                yield return sectionResult;
            }
        }

        private static IAssessmentSection GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations()
        {
            var assessmentSection = new AssessmentSectionStub();

            var hydraulicBoundaryLocations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, true);

            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);

            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations);

            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations);

            return assessmentSection;
        }

        private static void SetHydraulicBoundaryLocationCalculationOutputConfigurations(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            var random = new Random(21);
            calculations.ElementAt(0).Output = null;
            calculations.ElementAt(2).Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());
        }
    }
}