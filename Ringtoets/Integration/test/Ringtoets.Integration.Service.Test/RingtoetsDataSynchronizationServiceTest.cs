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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class RingtoetsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithoutAssessmentSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            IEnumerable<ICalculation> expectedAffectedItems = assessmentSection.GetFailureMechanisms()
                                                                               .SelectMany(f => f.Calculations)
                                                                               .Where(c => c.HasOutput)
                                                                               .ToList();

            // Call
            IEnumerable<IObservable> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms()
                                                      .SelectMany(f => f.Calculations)
                                                      .Where(c => c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_VariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
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
            expectedAffectedItems.AddRange(assessmentSection.PipingFailureMechanism.Calculations
                                                            .Cast<PipingCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.PipingFailureMechanism.Calculations
                                                            .Cast<PipingCalculation>()
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

            // Call
            IEnumerable<IObservable> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

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
            Assert.IsTrue(assessmentSection.PipingFailureMechanism.Calculations.Cast<PipingCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityPointStructures.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.StabilityStoneCover.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
            Assert.IsTrue(assessmentSection.WaveImpactAsphaltCover.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null,
                                                                                                               grassCoverErosionOutwardsFailureMechanism,
                                                                                                               duneErosionFailureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_GrassCoverErosionOutwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase,
                                                                                                               null,
                                                                                                               duneErosionFailureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("grassCoverErosionOutwardsFailureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_DuneErosionFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase,
                                                                                                               grassCoverErosionOutwardsFailureMechanism,
                                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneErosionFailureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_TwoArgumentsGrassCoverErosionOutwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null,
                                                                                                               duneErosionFailureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("grassCoverErosionOutwardsFailureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_TwoArgumentsDuneErosionFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(grassCoverErosionOutwardsFailureMechanism,
                                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneErosionFailureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_DoNothing()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                assessmentSection.HydraulicBoundaryDatabase,
                assessmentSection.GrassCoverErosionOutwards,
                assessmentSection.DuneErosion);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutputGrassCoverErosionOutwardsANdDunesWithoutLocations_DoNothing()
        {
            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                new GrassCoverErosionOutwardsFailureMechanism(), 
                new DuneErosionFailureMechanism());

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        [TestCaseSource(nameof(HydraulicBoundaryLocationAndGrassAndDuneLocations))]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryGrassCoverErosionOutwardsAndDuneLocations_ClearDataAndReturnAffectedLocations(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                                                                                                        HydraulicBoundaryLocation grassBoundaryLocation,
                                                                                                                                                        DuneLocation duneLocation,
                                                                                                                                                        IEnumerable<IObservable> expectedAffectedItems)
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();
            duneErosionFailureMechanism.DuneLocations.Add(duneLocation);

            grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations.Add(grassBoundaryLocation);

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                grassCoverErosionOutwardsFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.Locations[0] = hydraulicBoundaryLocation;

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSectionStub.HydraulicBoundaryDatabase,
                                                                                                                                grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                duneErosionFailureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            Assert.IsNull(duneLocation.Output);
            Assert.IsNull(grassBoundaryLocation.DesignWaterLevelOutput);
            Assert.IsNull(grassBoundaryLocation.WaveHeightOutput);
        }

        [Test]
        [TestCaseSource(nameof(GrassAndDuneLocations))]
        public void ClearHydraulicBoundaryLocationOutput_GrassCoverErosionOutwardsAndDuneLocations_ClearDataAndReturnAffectedLocations(HydraulicBoundaryLocation grassBoundaryLocation,
                                                                                                                                       DuneLocation duneLocation,
                                                                                                                                       IEnumerable<IObservable> expectedAffectedItems)
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();
            duneErosionFailureMechanism.DuneLocations.Add(duneLocation);

            grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations.Add(grassBoundaryLocation);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                grassCoverErosionOutwardsFailureMechanism,
                duneErosionFailureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            Assert.IsNull(duneLocation.Output);
            Assert.IsNull(grassBoundaryLocation.DesignWaterLevelOutput);
            Assert.IsNull(grassBoundaryLocation.WaveHeightOutput);
        }

        [Test]
        public void ClearReferenceLine_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.ClearReferenceLine(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_AllReferenceLineDependentDataCleared()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();

            // Call
            RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
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

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityInwardsFailureMechanism.SectionResults);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macrostabilityOutwardsFailureMechanism.SectionResults);

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

            Assert.IsNull(assessmentSection.ReferenceLine);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_ClearResultsContainAllAffectedObjectsAndAllRemovedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();

            IList<object> expectedRemovedObjects = GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection);

            // Call
            ClearResults results = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            IObservable[] changedObjects = results.ChangedObjects.ToArray();
            Assert.AreEqual(39, changedObjects.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.Contains(changedObjects, duneErosionFailureMechanism);

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.Contains(changedObjects, macrostabilityInwardsFailureMechanism);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.Contains(changedObjects, macrostabilityOutwardsFailureMechanism);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.Contains(changedObjects, microstabilityFailureMechanism);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.Contains(changedObjects, waterPressureAsphaltCoverFailureMechanism);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffOutwardsFailureMechanism);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffInwardsFailureMechanism);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.Contains(changedObjects, stabilityLengthwiseConstructionFailureMechanism);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.Contains(changedObjects, pipingStructureFailureMechanism);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.Contains(changedObjects, technicalInnovationFailureMechanism);

            CollectionAssert.Contains(changedObjects, assessmentSection);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
            CollectionAssert.DoesNotContain(results.RemovedObjects, null);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSectionWithoutReferenceLine_ClearResultsDoesNotContainReferenceLineNorNullForRemovedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            ReferenceLine originalReferenceLine = assessmentSection.ReferenceLine;
            assessmentSection.ReferenceLine = null;

            IList<object> expectedRemovedObjects = GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection);

            // Call
            ClearResults results = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            IObservable[] changedObjects = results.ChangedObjects.ToArray();
            Assert.AreEqual(39, changedObjects.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(changedObjects, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(changedObjects, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(changedObjects, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.Contains(changedObjects, duneErosionFailureMechanism);

            MacrostabilityInwardsFailureMechanism macrostabilityInwardsFailureMechanism = assessmentSection.MacrostabilityInwards;
            CollectionAssert.Contains(changedObjects, macrostabilityInwardsFailureMechanism);

            MacrostabilityOutwardsFailureMechanism macrostabilityOutwardsFailureMechanism = assessmentSection.MacrostabilityOutwards;
            CollectionAssert.Contains(changedObjects, macrostabilityOutwardsFailureMechanism);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.Contains(changedObjects, microstabilityFailureMechanism);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.Contains(changedObjects, waterPressureAsphaltCoverFailureMechanism);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffOutwardsFailureMechanism);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.Contains(changedObjects, grassCoverSlipOffInwardsFailureMechanism);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.Contains(changedObjects, stabilityLengthwiseConstructionFailureMechanism);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.Contains(changedObjects, pipingStructureFailureMechanism);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.Contains(changedObjects, technicalInnovationFailureMechanism);

            CollectionAssert.Contains(changedObjects, assessmentSection);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
            CollectionAssert.DoesNotContain(results.RemovedObjects, originalReferenceLine);
            CollectionAssert.DoesNotContain(results.RemovedObjects, null);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = new StabilityStoneCoverFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityStoneCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredStabilityStoneCoverFailureMechanism();
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
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredWaveImpactAsphaltCoverFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredWaveImpactAsphaltCoverFailureMechanism();
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
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredGrassCoverErosionOutwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredGrassCoverErosionOutwardsFailureMechanism();
            ForeshoreProfile profile = failureMechanism.ForeshoreProfiles[0];
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithForeshoreProfile =
                failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                .Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile))
                                .ToArray();

            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculationsWithOutput = calculationsWithForeshoreProfile.Where(c => c.HasOutput).ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithForeshoreProfile);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
            HeightStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = new HeightStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredHeightStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredHeightStructuresFailureMechanism();
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
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
            ClosingStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = new ClosingStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredClosingStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredClosingStructuresFailureMechanism();
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
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
            StabilityPointStructuresFailureMechanism failureMechanism = null;
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = new StabilityPointStructuresFailureMechanism();
            ForeshoreProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_FullyConfiguredStabilityPointStructuresFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredStabilityPointStructuresFailureMechanism();
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
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ForeshoreProfiles, profile);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithForeshoreProfile)
            {
                Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            }

            IObservable[] array = observables.ToArray();
            var expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithForeshoreProfile.Length;
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
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = null;
            DikeProfile profile = new TestDikeProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfile profile = null;

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("profile", paramName);
        }

        [Test]
        public void RemoveDikeProfile_FullyConfiguredGrassCoverErosionInwardsFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = TestDataGenerator.GetFullyConfiguredGrassCoverErosionInwardsFailureMechanism();
            DikeProfile profile = failureMechanism.DikeProfiles[0];
            GrassCoverErosionInwardsCalculation[] calculations = failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                                                                 .Where(c => ReferenceEquals(c.InputParameters.DikeProfile, profile))
                                                                                 .ToArray();
            int originalNumberOfSectionResultAssignments = failureMechanism.SectionResults.Count(sr => sr.Calculation != null);
            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults
                                                                                                     .Where(sr => calculations.Contains(sr.Calculation))
                                                                                                     .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);
            CollectionAssert.IsNotEmpty(sectionResults);

            // Call
            IEnumerable<IObservable> observables = RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profile);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, profile);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.DikeProfile);
            }
            foreach (GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult in sectionResults)
            {
                Assert.IsNull(sectionResult.Calculation);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + (calculations.Length*2) + sectionResults.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation);
                CollectionAssert.Contains(array, calculation.InputParameters);
                Assert.IsFalse(calculation.HasOutput);
            }
            foreach (GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult in sectionResults)
            {
                CollectionAssert.Contains(array, sectionResult);
            }
            Assert.AreEqual(originalNumberOfSectionResultAssignments - sectionResults.Length, failureMechanism.SectionResults.Count(sr => sr.Calculation != null),
                            "Other section results with a different calculation/dikeprofile should still have their association.");
        }

        private IList<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(AssessmentSection assessmentSection)
        {
            var expectedRemovedObjects = new List<object>();
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.PipingFailureMechanism));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.GrassCoverErosionInwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.MacrostabilityInwards));
            expectedRemovedObjects.AddRange(GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection.MacrostabilityOutwards));
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
            if (assessmentSection.ReferenceLine != null)
            {
                expectedRemovedObjects.Add(assessmentSection.ReferenceLine);
            }
            return expectedRemovedObjects;
        }

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(PipingFailureMechanism failureMechanism)
        {
            foreach (object failureMechanismObject in GetExpectedRemovedObjectsWhenClearingReferenceLine<PipingFailureMechanism>(failureMechanism))
            {
                yield return failureMechanismObject;
            }
            foreach (ICalculationBase calculationBase in failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
            {
                yield return calculationBase;
            }
            foreach (StochasticSoilModel stochasticSoilModel in failureMechanism.StochasticSoilModels)
            {
                yield return stochasticSoilModel;
            }
            foreach (RingtoetsPipingSurfaceLine surfaceLine in failureMechanism.SurfaceLines)
            {
                yield return surfaceLine;
            }
        }

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(GrassCoverErosionInwardsFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(StabilityStoneCoverFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(HeightStructuresFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(ClosingStructuresFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine(StabilityPointStructuresFailureMechanism failureMechanism)
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

        private IEnumerable<object> GetExpectedRemovedObjectsWhenClearingReferenceLine<T>(T failureMechanism) where T : IFailureMechanism, IHasSectionResults<FailureMechanismSectionResult>
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

        #region TestData

        private static IEnumerable<TestCaseData> GrassAndDuneLocations
        {
            get
            {
                TestDuneLocation duneLocationWithOutput;
                TestHydraulicBoundaryLocation grassBoundaryLocationWithOutput;

                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    new TestDuneLocation(),
                    new IObservable[0]
                    ).SetName("GrassAndDuneLocationNoOutput");
                yield return new TestCaseData(
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    new TestDuneLocation(),
                    new IObservable[]
                    {
                        grassBoundaryLocationWithOutput
                    }
                    ).SetName("GrassLocationWithOutput");
                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        duneLocationWithOutput
                    }
                    ).SetName("DuneLocationWithOutput");
                yield return new TestCaseData(
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        grassBoundaryLocationWithOutput,
                        duneLocationWithOutput
                    }
                    ).SetName("GrassAndDuneLocationWithOutput");
            }
        }

        private static IEnumerable<TestCaseData> HydraulicBoundaryLocationAndGrassAndDuneLocations
        {
            get
            {
                TestHydraulicBoundaryLocation boundaryLocationWithOutput;
                TestHydraulicBoundaryLocation grassBoundaryLocationWithOutput;
                TestDuneLocation duneLocationWithOutput;

                yield return new TestCaseData(
                    boundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    new TestHydraulicBoundaryLocation(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        boundaryLocationWithOutput,
                        duneLocationWithOutput
                    }).SetName("HydraulicBoundaryAndDuneLocationWithOutput");
                yield return new TestCaseData(
                    boundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    new TestHydraulicBoundaryLocation(),
                    new TestDuneLocation(),
                    new IObservable[]
                    {
                        boundaryLocationWithOutput
                    }).SetName("HydraulicBoundaryLocationWithOutput");
                yield return new TestCaseData(
                    boundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        boundaryLocationWithOutput,
                        grassBoundaryLocationWithOutput,
                        duneLocationWithOutput
                    }).SetName("AllTypesWithOutput");
                yield return new TestCaseData(
                    boundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    new TestDuneLocation(),
                    new IObservable[]
                    {
                        boundaryLocationWithOutput,
                        grassBoundaryLocationWithOutput
                    }).SetName("HydraulicBoundaryAndGrassLocationWithOutput");

                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    new TestHydraulicBoundaryLocation(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        duneLocationWithOutput
                    }).SetName("DuneLocationWithOutput");
                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    new TestHydraulicBoundaryLocation(),
                    new TestDuneLocation(),
                    new IObservable[0]
                    ).SetName("AllTypesNoOutput");
                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    duneLocationWithOutput = new TestDuneLocation
                    {
                        Output = new TestDuneLocationOutput()
                    },
                    new IObservable[]
                    {
                        grassBoundaryLocationWithOutput,
                        duneLocationWithOutput
                    }).SetName("GrassAndDuneLocationWithOutput");
                yield return new TestCaseData(
                    new TestHydraulicBoundaryLocation(),
                    grassBoundaryLocationWithOutput = TestHydraulicBoundaryLocation.CreateFullyCalculated(),
                    new TestDuneLocation(),
                    new IObservable[]
                    {
                        grassBoundaryLocationWithOutput
                    }).SetName("GrassLocationWithOutput");
            }
        }

        #endregion
    }
}