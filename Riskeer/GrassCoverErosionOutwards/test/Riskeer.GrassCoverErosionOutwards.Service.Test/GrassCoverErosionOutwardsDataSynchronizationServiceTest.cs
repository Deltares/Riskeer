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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearWaveConditionsCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculation_WithCalculation_OutputNullAndReturnAffectedCalculation()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsNull(calculation.Output);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, affectedObjects);
        }

        [Test]
        public void ClearWaveConditionsCalculation_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = null
            };

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsOutputAndReturnsAffectedObjects()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>().ToArray();
            IObservable[] expectedAffectedCalculations = calculations.Where(c => c.HasOutput)
                                                                     .Cast<IObservable>()
                                                                     .ToArray();
            IObservable[] expectedAffectedCalculationInputs = calculations.Select(c => c.InputParameters)
                                                                          .Where(i => i.HydraulicBoundaryLocation != null)
                                                                          .Cast<IObservable>()
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));

            CollectionAssert.AreEqual(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .ToArray();

            // Call
            ClearResults results = GrassCoverErosionOutwardsDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_FailureMechanismWithOutputs_OutputClearedAndAffectedItemsReturned()
        {
            // Setup
            var hydraulicBoundaryLocations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
            failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();

            IEnumerable<HydraulicBoundaryLocationCalculation> expectedAffectedItems =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedItems = GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(failureMechanism);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(failureMechanism);
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelCalculations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelCalculations(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelCalculations(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelCalculations_WithArguments_IllustrationPointsClearedAndAffectedItemsReturned()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            ConfigureHydraulicBoundaryLocationCalculationsWithIllustrationPointResults(assessmentSection, failureMechanism);

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsWithOutput = GetAllDesignWaterLevelCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsWithIllustrationPoints = waterLevelCalculationsWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                                                                  .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsWithOutput = GetAllWaveHeightCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsWithIllustrationPoints = waveHeightCalculationsWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                                                                  .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelCalculations(failureMechanism,
                                                                                                                                                                        assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(waterLevelCalculationsWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearIllustrationPointResultsForWaveHeightCalculations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForWaveHeightCalculations(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPointResultsForWaveHeightCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForWaveHeightCalculations(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsForWaveHeightCalculations_WithArguments_IllustrationPointsClearedAndAffectedItemsReturned()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            ConfigureHydraulicBoundaryLocationCalculationsWithIllustrationPointResults(assessmentSection, failureMechanism);

            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsWithOutput = GetAllDesignWaterLevelCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();
            HydraulicBoundaryLocationCalculation[] waterLevelCalculationsWithIllustrationPoints = waterLevelCalculationsWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                                                                  .ToArray();

            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsWithOutput = GetAllWaveHeightCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();
            HydraulicBoundaryLocationCalculation[] waveHeightCalculationsWithIllustrationPoints = waveHeightCalculationsWithOutput.Where(calc => calc.Output.HasGeneralResult)
                                                                                                                                  .ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForWaveHeightCalculations(failureMechanism,
                                                                                                                                                                  assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(waveHeightCalculationsWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(waterLevelCalculationsWithIllustrationPoints.All(calc => calc.Output.HasGeneralResult));
            Assert.IsTrue(waterLevelCalculationsWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(waveHeightCalculationsWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(waveHeightCalculationsWithOutput.All(calc => calc.HasOutput));
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations_WithArguments_IllustrationPointsClearedAndAffectedItemsReturned()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            ConfigureHydraulicBoundaryLocationCalculationsWithIllustrationPointResults(assessmentSection, failureMechanism);

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllDesignWaterLevelCalculationsWithOutput(assessmentSection, failureMechanism)
                                                                            .Concat(GetAllWaveHeightCalculationsWithOutput(assessmentSection, failureMechanism))
                                                                            .ToArray();
            HydraulicBoundaryLocationCalculation[] calculationsWithIllustrationPoints = calculationsWithOutput.Where(calc => calc.Output.HasGeneralResult).ToArray();

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(failureMechanism,
                                                                                                                                                                                     assessmentSection);

            // Assert
            CollectionAssert.AreEquivalent(calculationsWithIllustrationPoints, affectedObjects);

            Assert.IsTrue(calculationsWithIllustrationPoints.All(calc => !calc.Output.HasGeneralResult));
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllDesignWaterLevelCalculationsWithOutput(IAssessmentSection assessmentSection,
                                                                                                                      GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm)
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm)
                                   .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                   .Concat(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                                   .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllWaveHeightCalculationsWithOutput(IAssessmentSection assessmentSection,
                                                                                                                GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm)
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm)
                                   .Concat(assessmentSection.WaveHeightCalculationsForLowerLimitNorm)
                                   .Concat(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm)
                                   .Where(calc => calc.HasOutput);
        }

        private static void ConfigureHydraulicBoundaryLocationCalculationsWithIllustrationPointResults(IAssessmentSection assessmentSection,
                                                                                                       GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);

            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            SetHydraulicBoundaryLocationCalculationOutputConfigurations(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
        }

        private static void SetHydraulicBoundaryLocationCalculationOutputConfigurations(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            var random = new Random(21);
            calculations.ElementAt(0).Output = null;
            calculations.ElementAt(1).Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            calculations.ElementAt(2).Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());
        }

        private static GrassCoverErosionOutwardsFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation
                }
            });

            return failureMechanism;
        }
    }
}