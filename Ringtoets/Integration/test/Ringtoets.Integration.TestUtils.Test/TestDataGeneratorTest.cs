// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.TestUtils.Test
{
    [TestFixture]
    public class TestDataGeneratorTest
    {
        [Test]
        public void GetAssessmentSectionWithAllCalculationConfigurations_NoArguments_ReturnsWithAllPossibleCalculationConfigurations()
        {
            // Call
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            // Assert
            Assert.AreEqual(AssessmentSectionComposition.Dike, assessmentSection.Composition);
            AssertFailureMechanismHasAllPossibleCalculationConfigurations(assessmentSection);
            Assert.True(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.All(loc => loc.DesignWaterLevelOutput != null
                                                                                                          && loc.WaveHeightOutput != null));

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();
            AssertDuneErosionFailureMechanismCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void GetAssessmentSectionWithAllCalculationConfigurations_CompositionGiven_ReturnWithCompositionAndAllPossibleCalculationConfigurations(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismHasAllPossibleCalculationConfigurations(assessmentSection);
            Assert.True(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.All(loc => loc.DesignWaterLevelOutput != null
                                                                                                          && loc.WaveHeightOutput != null));

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                           .OfType<DuneErosionFailureMechanism>()
                                                                           .First();
            AssertDuneErosionFailureMechanismCalculationConfigurationsWithoutOutputs(duneErosionFailureMechanism);
            AssertDuneErosionFailureMechanismCalculationConfigurationsWithOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_CompositionGiven_ReturnWithOnlyCalculationOutputsAndComposition(Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_CompositionGiven_ReturnWithOnlyCalculationOutputsAndComposition(DikeAndDune)")]
        [TestCase(AssessmentSectionComposition.Dune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_CompositionGiven_ReturnWithOnlyCalculationOutputsAndComposition(Dune)")]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput_CompositionGiven_ReturnWithOnlyCalculationOutputsAndComposition(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismHasAllPossibleCalculationConfigurations(assessmentSection);
            Assert.True(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.All(loc => loc.DesignWaterLevelOutput == null
                                                                                                          && loc.WaveHeightOutput == null));
            Assert.True(assessmentSection.DuneErosion.DuneLocations.All(dl => dl.Output == null));
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_ReturnWithCompositionAndOnlyHydraulicOutputs(Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_ReturnWithCompositionAndOnlyHydraulicOutputs(DikeAndDune)")]
        [TestCase(AssessmentSectionComposition.Dune, TestName = "GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_ReturnWithCompositionAndOnlyHydraulicOutputs(Dune)")]
        public void GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput_CompositionGiven_ReturnWithOnlyHydraulicBoundaryLocationAndDuneLocationOutputsAndComposition(
            AssessmentSectionComposition composition)
        {
            // Call 
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(composition);

            // Assert
            Assert.AreEqual(composition, assessmentSection.Composition);
            AssertFailureMechanismHasAllCalculationConfigurationsWithoutCalculationOutputs(assessmentSection);
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(calc => calc.HasOutput));
            Assert.True(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.All(loc => loc.DesignWaterLevelOutput != null
                                                                                                          && loc.WaveHeightOutput != null));
        }

        private static void AssertFailureMechanismHasAllPossibleCalculationConfigurations(IAssessmentSection assessmentSection)
        {
            bool containsClosingStructuresFailureMechanism = false;
            bool containsGrassCoverErosionInwardsFailureMechanism = false;
            bool containsGrassCoverErosionOutwardsFailureMechanism = false;
            bool containsHeightStructuresFailureMechanism = false;
            bool containsPipingFailureMechanism = false;
            bool containsStabilityPointStructuresFailureMechanism = false;
            bool containsStabilityStoneCoverFailureMechanism = false;
            bool containsWaveImpactAsphaltCoverFailureMechanism = false;
            bool containsDuneErosionFailureMechanism = false;

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var duneErosionFailureMechanism = failureMechanism as DuneErosionFailureMechanism;

                if (closingStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(closingStructuresFailureMechanism);
                    AssertClosingStructureFailuresMechanismCalculationsWithOutputs(closingStructuresFailureMechanism);
                    AssertClosingStructureFailuresMechanismCalculationsWithoutOutputs(closingStructuresFailureMechanism);
                    containsClosingStructuresFailureMechanism = true;
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    AssertHasDikeProfiles(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithOutputs(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionInwardsFailureMechanism);
                    containsGrassCoverErosionInwardsFailureMechanism = true;
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(grassCoverErosionOutwardsFailureMechanism);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithOutputs(grassCoverErosionOutwardsFailureMechanism);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionOutwardsFailureMechanism);
                    containsGrassCoverErosionOutwardsFailureMechanism = true;
                }

                if (heightStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(heightStructuresFailureMechanism);
                    AssertHeightStructureFailureMechanismCalculationConfigurationsWithOutputs(heightStructuresFailureMechanism);
                    AssertHeightStructureFailureMechanismCalculationConfigurationsWithoutOutputs(heightStructuresFailureMechanism);
                    containsHeightStructuresFailureMechanism = true;
                }

                if (pipingFailureMechanism != null)
                {
                    AssertHasStochasticSoilModels(pipingFailureMechanism);
                    AssertHasSurfaceLines(pipingFailureMechanism);
                    AssertPipingFailureMechanismCalculationConfigurationsWithOutputs(pipingFailureMechanism);
                    AssertPipingFailureMechanismCalculationConfigurationsWithoutOutputs(pipingFailureMechanism);
                    containsPipingFailureMechanism = true;
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithOutputs(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityPointStructuresFailureMechanism);
                    containsStabilityPointStructuresFailureMechanism = true;
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverWaveConditionsFailureMechanismCalculationConfigurationsWithOutputs(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityStoneCoverFailureMechanism);
                    containsStabilityStoneCoverFailureMechanism = true;
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverWaveConditionsFailureMechanismCalculationConfigurationsWithOutputs(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(waveImpactAsphaltCoverFailureMechanism);
                    containsWaveImpactAsphaltCoverFailureMechanism = true;
                }

                if (duneErosionFailureMechanism != null)
                {
                    containsDuneErosionFailureMechanism = true;
                }
            }

            Assert.IsTrue(containsClosingStructuresFailureMechanism);
            Assert.IsTrue(containsGrassCoverErosionInwardsFailureMechanism);
            Assert.IsTrue(containsGrassCoverErosionOutwardsFailureMechanism);
            Assert.IsTrue(containsHeightStructuresFailureMechanism);
            Assert.IsTrue(containsPipingFailureMechanism);
            Assert.IsTrue(containsStabilityPointStructuresFailureMechanism);
            Assert.IsTrue(containsStabilityStoneCoverFailureMechanism);
            Assert.IsTrue(containsWaveImpactAsphaltCoverFailureMechanism);
            Assert.IsTrue(containsDuneErosionFailureMechanism);
        }

        private static void AssertFailureMechanismHasAllCalculationConfigurationsWithoutCalculationOutputs(IAssessmentSection assessmentSection)
        {
            bool containsClosingStructuresFailureMechanism = false;
            bool containsGrassCoverErosionInwardsFailureMechanism = false;
            bool containsGrassCoverErosionOutwardsFailureMechanism = false;
            bool containsHeightStructuresFailureMechanism = false;
            bool containsPipingFailureMechanism = false;
            bool containsStabilityPointStructuresFailureMechanism = false;
            bool containsStabilityStoneCoverFailureMechanism = false;
            bool containsWaveImpactAsphaltCoverFailureMechanism = false;
            bool containsDuneErosionFailureMechanism = false;

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var coverErosionOutwards = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var duneErosionFailureMechanism = failureMechanism as DuneErosionFailureMechanism;

                if (closingStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(closingStructuresFailureMechanism);
                    AssertClosingStructureFailuresMechanismCalculationsWithoutOutputs(closingStructuresFailureMechanism);
                    containsClosingStructuresFailureMechanism = true;
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    AssertHasDikeProfiles(grassCoverErosionInwardsFailureMechanism);
                    AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(grassCoverErosionInwardsFailureMechanism);
                    containsGrassCoverErosionInwardsFailureMechanism = true;
                }

                if (coverErosionOutwards != null)
                {
                    AssertHasForeshoreProfiles(coverErosionOutwards);
                    AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(coverErosionOutwards);
                    containsGrassCoverErosionOutwardsFailureMechanism = true;
                }

                if (heightStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(heightStructuresFailureMechanism);
                    AssertHeightStructureFailureMechanismCalculationConfigurationsWithoutOutputs(heightStructuresFailureMechanism);
                    containsHeightStructuresFailureMechanism = true;
                }

                if (pipingFailureMechanism != null)
                {
                    AssertHasStochasticSoilModels(pipingFailureMechanism);
                    AssertHasSurfaceLines(pipingFailureMechanism);
                    AssertPipingFailureMechanismCalculationConfigurationsWithoutOutputs(pipingFailureMechanism);
                    containsPipingFailureMechanism = true;
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityPointStructuresFailureMechanism);
                    AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityPointStructuresFailureMechanism);
                    containsStabilityPointStructuresFailureMechanism = true;
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(stabilityStoneCoverFailureMechanism);
                    AssertStabilityStoneCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(stabilityStoneCoverFailureMechanism);
                    containsStabilityStoneCoverFailureMechanism = true;
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    AssertHasForeshoreProfiles(waveImpactAsphaltCoverFailureMechanism);
                    AssertWaveImpactAsphaltCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(waveImpactAsphaltCoverFailureMechanism);
                    containsWaveImpactAsphaltCoverFailureMechanism = true;
                }

                if (duneErosionFailureMechanism != null)
                {
                    containsDuneErosionFailureMechanism = true;
                }
            }

            Assert.IsTrue(containsClosingStructuresFailureMechanism);
            Assert.IsTrue(containsGrassCoverErosionOutwardsFailureMechanism);
            Assert.IsTrue(containsHeightStructuresFailureMechanism);
            Assert.IsTrue(containsPipingFailureMechanism);
            Assert.IsTrue(containsStabilityPointStructuresFailureMechanism);
            Assert.IsTrue(containsStabilityStoneCoverFailureMechanism);
            Assert.IsTrue(containsWaveImpactAsphaltCoverFailureMechanism);
            Assert.IsTrue(containsDuneErosionFailureMechanism);
            Assert.IsTrue(containsGrassCoverErosionInwardsFailureMechanism);
        }

        #region Piping

        private static void AssertPipingFailureMechanismCalculationConfigurationsWithOutputs(PipingFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertPipingCalculationGroupWithOutput(calculationRoot.OfType<PipingCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertPipingCalculationGroupWithOutput(nestedCalculations.Children.OfType<PipingCalculation>());
        }

        private static void AssertPipingFailureMechanismCalculationConfigurationsWithoutOutputs(PipingFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertPipingCalculationGroupWithoutOutput(calculationRoot.OfType<PipingCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertPipingCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<PipingCalculation>());
        }

        private static void AssertPipingCalculationGroupWithOutput(IEnumerable<PipingCalculation> children)
        {
            AssertCalculationConfig(children, true, true);
        }

        private static void AssertPipingCalculationGroupWithoutOutput(IEnumerable<PipingCalculation> children)
        {
            AssertCalculationConfig(children, false, false);
            AssertCalculationConfig(children, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<PipingCalculation> children, bool hasHydraulicBoundaryLocation, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasStochasticSoilModels(PipingFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.StochasticSoilModels);
        }

        private static void AssertHasSurfaceLines(PipingFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.SurfaceLines);
        }

        #endregion

        #region Grass Cover Erosion Inwards

        private static void AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertGrassCoverErosionInwardsCalculationGroupWithOutput(calculationRoot.OfType<GrassCoverErosionInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionInwardsCalculationGroupWithOutput(nestedCalculations.Children.OfType<GrassCoverErosionInwardsCalculation>());
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanismCalculationConfigurationsWithoutOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(calculationRoot.OfType<GrassCoverErosionInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<GrassCoverErosionInwardsCalculation>());
        }

        private static void AssertGrassCoverErosionInwardsCalculationGroupWithOutput(IEnumerable<GrassCoverErosionInwardsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertGrassCoverErosionInwardsCalculationGroupWithoutOutput(IEnumerable<GrassCoverErosionInwardsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<GrassCoverErosionInwardsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && (calc.InputParameters.DikeProfile != null) == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasDikeProfiles(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.DikeProfiles);
        }

        #endregion

        #region Stability Stone Cover 

        private static void AssertStabilityStoneCoverWaveConditionsFailureMechanismCalculationConfigurationsWithOutputs(
            StabilityStoneCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(
                calculationRoot.OfType<StabilityStoneCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(
                nestedCalculations.Children.OfType<StabilityStoneCoverWaveConditionsCalculation>());
        }

        private static void AssertStabilityStoneCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(calculationRoot.OfType<StabilityStoneCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<StabilityStoneCoverWaveConditionsCalculation>());
        }

        private static void AssertStabilityStoneCoverWaveConditionsCalculationGroupWithOutput(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertStabilityStoneCoverWaveCalculationGroupWithoutOutput(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && (calc.InputParameters.ForeshoreProfile != null) == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Wave Impact Asphalt Cover

        private static void AssertWaveImpactAsphaltCoverWaveConditionsFailureMechanismCalculationConfigurationsWithOutputs(
            WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationGroupWithOutput(
                calculationRoot.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationGroupWithOutput(
                nestedCalculations.Children.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());
        }

        private static void AssertWaveImpactAsphaltCoverWaveConditionsFailureMechanismCalculationConfigurationsWithoutOutputs(
            WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertWaveImpactAsphaltCoverWaveCalculationGroupWithoutOutput(calculationRoot.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertWaveImpactAsphaltCoverWaveCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>());
        }

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculationGroupWithOutput(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertWaveImpactAsphaltCoverWaveCalculationGroupWithoutOutput(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && (calc.InputParameters.ForeshoreProfile != null) == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Grass Cover Erosion Outwards

        private static void AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertGrassCoverErosionCalculationGroupWithOutput(calculationRoot.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionCalculationGroupWithOutput(nestedCalculations.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanismCalculationConfigurationsWithoutOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.WaveConditionsCalculationGroup.Children;
            AssertGrassCoverErosionCalculationGroupWithoutOutput(calculationRoot.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertGrassCoverErosionCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>());
        }

        private static void AssertGrassCoverErosionCalculationGroupWithOutput(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, true, false, true);
            AssertCalculationConfig(children, true, true, true);
        }

        private static void AssertGrassCoverErosionCalculationGroupWithoutOutput(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children)
        {
            AssertCalculationConfig(children, false, false, false);
            AssertCalculationConfig(children, true, false, false);
            AssertCalculationConfig(children, true, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && (calc.InputParameters.ForeshoreProfile != null) == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        private static void AssertHasForeshoreProfiles(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region Dune Erosion

        private static void AssertDuneErosionFailureMechanismCalculationConfigurationsWithOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            Assert.True(failureMechanism.DuneLocations.Any(dl => dl.Output != null));
        }

        private static void AssertDuneErosionFailureMechanismCalculationConfigurationsWithoutOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            Assert.True(failureMechanism.DuneLocations.Any(dl => dl.Output == null));
        }

        #endregion

        #region Structures

        #region ClosingStructures

        private static void AssertClosingStructureFailuresMechanismCalculationsWithOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<ClosingStructure, ClosingStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<ClosingStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<ClosingStructure, ClosingStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<ClosingStructuresInput>>());
        }

        private static void AssertClosingStructureFailuresMechanismCalculationsWithoutOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<ClosingStructure, ClosingStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<ClosingStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<ClosingStructure, ClosingStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<ClosingStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(ClosingStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region StabilityPointStructures

        private static void AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<StabilityPointStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        private static void AssertStabilityPointStructuresFailureMechanismCalculationConfigurationsWithoutOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<StabilityPointStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<StabilityPointStructure, StabilityPointStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        #region HeightStructures

        private static void AssertHeightStructureFailureMechanismCalculationConfigurationsWithOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithOutputs<HeightStructure, HeightStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<HeightStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithOutputs<HeightStructure, HeightStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<HeightStructuresInput>>());
        }

        private static void AssertHeightStructureFailureMechanismCalculationConfigurationsWithoutOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationsRoot = failureMechanism.CalculationsGroup.Children;
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<HeightStructure, HeightStructuresInput>(
                calculationsRoot.OfType<StructuresCalculation<HeightStructuresInput>>());

            CalculationGroup nestedCalculations = calculationsRoot.OfType<CalculationGroup>().First();
            AssertStructuresCalculationGroupConfigurationWithoutOutputs<HeightStructure, HeightStructuresInput>(
                nestedCalculations.Children.OfType<StructuresCalculation<HeightStructuresInput>>());
        }

        private static void AssertHasForeshoreProfiles(HeightStructuresFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.ForeshoreProfiles);
        }

        #endregion

        private static void AssertStructuresCalculationGroupConfigurationWithOutputs<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, false, true);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, true, true);
        }

        private static void AssertStructuresCalculationGroupConfigurationWithoutOutputs<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, false, false, false);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, false, false);
            AssertCalculationConfig<TStructureBase, TCalculationInput>(children, true, true, false);
        }

        private static void AssertCalculationConfig<TStructureBase, TCalculationInput>(
            IEnumerable<StructuresCalculation<TCalculationInput>> children, bool hasHydraulicBoundaryLocation, bool hasDikeProfile, bool hasOutput)
            where TStructureBase : StructureBase
            where TCalculationInput : StructuresInputBase<TStructureBase>, new()
        {
            Assert.NotNull(children.FirstOrDefault(calc => (calc.InputParameters.HydraulicBoundaryLocation != null) == hasHydraulicBoundaryLocation
                                                           && (calc.InputParameters.ForeshoreProfile != null) == hasDikeProfile
                                                           && calc.HasOutput == hasOutput));
        }

        #endregion
    }
}