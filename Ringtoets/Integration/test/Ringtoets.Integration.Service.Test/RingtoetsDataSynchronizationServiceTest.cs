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
using Ringtoets.GrassCoverErosionOutwards.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
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
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs((IAssessmentSection) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
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
        public void ClearFailureMechanismCalculationOutputs_WithoutFailureMechanisms_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs((IEnumerable<IFailureMechanism>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithFailureMechanisms_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            IEnumerable<IFailureMechanism> failureMechanisms = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations()
                                                                                .GetFailureMechanisms()
                                                                                .ToList();
            IEnumerable<ICalculation> expectedAffectedItems = failureMechanisms
                                                              .SelectMany(f => f.Calculations)
                                                              .Where(c => c.HasOutput)
                                                              .ToList();

            // Call
            IEnumerable<IObservable> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(failureMechanisms);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanisms
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
                                                            .Cast<PipingCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.Piping.Calculations
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
            expectedAffectedItems.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                            .Cast<MacroStabilityInwardsCalculation>()
                                                            .Where(c => c.HasOutput));
            expectedAffectedItems.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                            .Cast<MacroStabilityInwardsCalculation>()
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
            Assert.IsTrue(assessmentSection.Piping.Calculations.Cast<PipingCalculation>()
                                           .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));
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
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_HydraulicBoundaryGrassCoverErosionOutwardsAndDuneLocations_ClearDataAndReturnAffectedObjects()
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

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
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

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            grassCoverErosionOutwardsFailureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation1 = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation2 = assessmentSection.WaterLevelCalculationsForSignalingNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation3 = assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation4 = assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation5 = assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation6 = assessmentSection.WaveHeightCalculationsForSignalingNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation7 = assessmentSection.WaveHeightCalculationsForLowerLimitNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation8 = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm
                                                                                                          .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));

            hydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation3.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation4.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation5.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation6.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation7.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            hydraulicBoundaryLocationCalculation8.Output = new TestHydraulicBoundaryLocationCalculationOutput();

            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation1 = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation2 = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation3 = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation4 = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation5 = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));
            HydraulicBoundaryLocationCalculation grassHydraulicBoundaryLocationCalculation6 = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm
                                                                                                                                       .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation1));

            grassHydraulicBoundaryLocationCalculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            grassHydraulicBoundaryLocationCalculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            grassHydraulicBoundaryLocationCalculation3.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            grassHydraulicBoundaryLocationCalculation4.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            grassHydraulicBoundaryLocationCalculation5.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            grassHydraulicBoundaryLocationCalculation6.Output = new TestHydraulicBoundaryLocationCalculationOutput();

            DuneLocationCalculation duneLocationCalculation1 = duneErosionFailureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));
            DuneLocationCalculation duneLocationCalculation2 = duneErosionFailureMechanism.CalculationsForMechanismSpecificSignalingNorm.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));
            DuneLocationCalculation duneLocationCalculation3 = duneErosionFailureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));
            DuneLocationCalculation duneLocationCalculation4 = duneErosionFailureMechanism.CalculationsForLowerLimitNorm.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));
            DuneLocationCalculation duneLocationCalculation5 = duneErosionFailureMechanism.CalculationsForFactorizedLowerLimitNorm.First(c => ReferenceEquals(c.DuneLocation, duneLocation1));

            duneLocationCalculation1.Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculation2.Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculation3.Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculation4.Output = new TestDuneLocationCalculationOutput();
            duneLocationCalculation5.Output = new TestDuneLocationCalculationOutput();

            var expectedAffectedItems = new IObservable[]
            {
                hydraulicBoundaryLocationCalculation1,
                hydraulicBoundaryLocationCalculation2,
                hydraulicBoundaryLocationCalculation3,
                hydraulicBoundaryLocationCalculation4,
                hydraulicBoundaryLocationCalculation5,
                hydraulicBoundaryLocationCalculation6,
                hydraulicBoundaryLocationCalculation7,
                hydraulicBoundaryLocationCalculation8,
                grassHydraulicBoundaryLocationCalculation1,
                grassHydraulicBoundaryLocationCalculation2,
                grassHydraulicBoundaryLocationCalculation3,
                grassHydraulicBoundaryLocationCalculation4,
                grassHydraulicBoundaryLocationCalculation5,
                grassHydraulicBoundaryLocationCalculation6,
                duneLocationCalculation1,
                duneLocationCalculation2,
                duneLocationCalculation3,
                duneLocationCalculation4,
                duneLocationCalculation5
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation1.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation2.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation3.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation4.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation5.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation6.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation7.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation8.HasOutput);

            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation1.HasOutput);
            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation2.HasOutput);
            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation3.HasOutput);
            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation4.HasOutput);
            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation5.HasOutput);
            Assert.IsFalse(grassHydraulicBoundaryLocationCalculation6.HasOutput);

            Assert.IsNull(duneLocationCalculation1.Output);
            Assert.IsNull(duneLocationCalculation2.Output);
            Assert.IsNull(duneLocationCalculation3.Output);
            Assert.IsNull(duneLocationCalculation4.Output);
            Assert.IsNull(duneLocationCalculation5.Output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms((IAssessmentSection) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_AssessmentSectionWithFailureMechanismsContainingNoLocations_DoNothing()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new GrassCoverErosionOutwardsFailureMechanism(),
                new DuneErosionFailureMechanism()
            });
            mockRepository.ReplayAll();

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(assessmentSection);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_AssessmentSectionWithGrassCoverErosionOutwardsFailureMechanism_ClearDataAndReturnAffectedCalculations(bool hasOutput)
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureGrassCoverErosionOutwardsFailureMechanism(grassCoverErosionOutwardsFailureMechanism, hasOutput);

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                grassCoverErosionOutwardsFailureMechanism
            });
            mockRepository.ReplayAll();

            IEnumerable<HydraulicBoundaryLocationCalculation> expectedAffectedItems =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(grassCoverErosionOutwardsFailureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(grassCoverErosionOutwardsFailureMechanism);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_AssessmentSectionWithDuneErosionFailureMechanism_ClearDataAndReturnAffectedCalculations(bool hasOutput)
        {
            // Setup
            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();
            ConfigureDuneErosionFailureMechanism(duneErosionFailureMechanism, hasOutput);

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                duneErosionFailureMechanism
            });
            mockRepository.ReplayAll();

            IEnumerable<IObservable> expectedAffectedItems = DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(duneErosionFailureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(assessmentSection);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms((IEnumerable<IFailureMechanism>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_FailureMechanismsContainingNoLocations_DoNothing()
        {
            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(new IFailureMechanism[]
            {
                new GrassCoverErosionOutwardsFailureMechanism(),
                new DuneErosionFailureMechanism()
            });

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_GrassCoverErosionOutwardsFailureMechanism_ClearDataAndReturnAffectedCalculations(bool hasOutput)
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureGrassCoverErosionOutwardsFailureMechanism(grassCoverErosionOutwardsFailureMechanism, hasOutput);

            IEnumerable<HydraulicBoundaryLocationCalculation> expectedAffectedItems =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(grassCoverErosionOutwardsFailureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(new IFailureMechanism[]
            {
                grassCoverErosionOutwardsFailureMechanism
            });

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(grassCoverErosionOutwardsFailureMechanism);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms_DuneErosionFailureMechanism_ClearDataAndReturnAffectedCalculations(bool hasOutput)
        {
            // Setup
            var duneErosionFailureMechanism = new DuneErosionFailureMechanism();
            ConfigureDuneErosionFailureMechanism(duneErosionFailureMechanism, hasOutput);

            IEnumerable<IObservable> expectedAffectedItems = DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(duneErosionFailureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(new IFailureMechanism[]
            {
                duneErosionFailureMechanism
            });

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);
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
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            // Call
            RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

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

            Assert.IsNull(assessmentSection.ReferenceLine);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSection_ClearResultsContainAllAffectedObjectsAndAllRemovedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            IEnumerable<object> expectedRemovedObjects = GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection);

            // Call
            ClearResults results = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            AssertChangedObjects(results, assessmentSection);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
            CollectionAssert.DoesNotContain(results.RemovedObjects, null);
        }

        [Test]
        public void ClearReferenceLine_FullyConfiguredAssessmentSectionWithoutReferenceLine_ClearResultsDoesNotContainReferenceLineNorNullForRemovedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ReferenceLine originalReferenceLine = assessmentSection.ReferenceLine;
            assessmentSection.ReferenceLine = null;

            IEnumerable<object> expectedRemovedObjects = GetExpectedRemovedObjectsWhenClearingReferenceLine(assessmentSection);

            // Call
            ClearResults results = RingtoetsDataSynchronizationService.ClearReferenceLine(assessmentSection);

            // Assert
            AssertChangedObjects(results, assessmentSection);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
            CollectionAssert.DoesNotContain(results.RemovedObjects, originalReferenceLine);
            CollectionAssert.DoesNotContain(results.RemovedObjects, null);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((StabilityStoneCoverFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityStoneCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((WaveImpactAsphaltCoverFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_WaveImpactAsphaltCoverFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((GrassCoverErosionOutwardsFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_GrassCoverErosionOutwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((HeightStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_HeightStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((ClosingStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_ClosingStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile((StabilityPointStructuresFailureMechanism) null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveForeshoreProfile_StabilityPointStructuresFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveForeshoreProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveAllForeshoreProfiles<ICalculationInput>(null,
                                                                                                                        new ForeshoreProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void RemoveAllForeshoreProfiles_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveAllForeshoreProfiles(Enumerable.Empty<ICalculation<ICalculationInput>>(),
                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.RemoveAllForeshoreProfiles(calculations, foreshoreProfiles);

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
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(null, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveDikeProfile_GrassCoverErosionInwardsFailureMechanismProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => RingtoetsDataSynchronizationService.RemoveDikeProfile(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
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
            Assert.AreEqual(1 + (calculations.Length * 2) + sectionResults.Length, array.Length);
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

        private static void AssertChangedObjects(ClearResults results, AssessmentSection assessmentSection)
        {
            IObservable[] changedObjects = results.ChangedObjects.ToArray();
            Assert.AreEqual(60, changedObjects.Length);

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

            CollectionAssert.Contains(changedObjects, assessmentSection);
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

            if (assessmentSection.ReferenceLine != null)
            {
                expectedRemovedObjects.Add(assessmentSection.ReferenceLine);
            }

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

        private static void ConfigureGrassCoverErosionOutwardsFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism, bool hasOutput)
        {
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            if (hasOutput)
            {
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
                failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
                failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
                failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
                failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
                failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            }
        }

        private static void ConfigureDuneErosionFailureMechanism(DuneErosionFailureMechanism failureMechanism, bool hasOutput)
        {
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });

            if (hasOutput)
            {
                failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
                failureMechanism.CalculationsForMechanismSpecificSignalingNorm.First().Output = new TestDuneLocationCalculationOutput();
                failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
                failureMechanism.CalculationsForLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
                failureMechanism.CalculationsForFactorizedLowerLimitNorm.First().Output = new TestDuneLocationCalculationOutput();
            }
        }
    }
}