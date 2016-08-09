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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class RingtoetsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAssessmentSectionData_WithoutAssessmentSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearAssessmentSectionData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAssessmentSectionData_WithoutHydraulicBoundaryDatabase_DoesNotThrow()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void ClearAssessmentSectionData_WithAssessmentSection_ClearsHydraulicBoundaryLocationOutput()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0)
            {
                WaveHeight = 3.0,
                DesignWaterLevel = 4.2
            });

            // Call
            RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            HydraulicBoundaryLocation location = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            Assert.IsNaN(location.WaveHeight);
            Assert.IsNaN(location.DesignWaterLevel);
        }

        [Test]
        public void ClearAssessmentSectionData_WithAssessmentSection_ClearsFailureMechanismCalculationsOutput()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation()
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new HeightStructuresCalculation()
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            // Call
            RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            PipingCalculation pipingCalculation = assessmentSection.PipingFailureMechanism.CalculationsGroup.Children[0] as PipingCalculation;
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            HeightStructuresCalculation heightStructuresCalculation = assessmentSection.HeightStructures.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultiplePipingFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            var failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            var failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultipleHeightStructuresFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });
            var failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutput()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation()
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new HeightStructuresCalculation()
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            // Call
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            PipingCalculation pipingCalculation = assessmentSection.PipingFailureMechanism.CalculationsGroup.Children[0] as PipingCalculation;
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            HeightStructuresCalculation heightStructuresCalculation = assessmentSection.HeightStructures.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultiplePipingFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            var failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            var failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultipleHeightStructuresFailureMechanisms_ClearsOutput()
        {
            // Setup
            var failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });
            var failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithAssessmentSection_ClearsHydraulicBoundaryLocation()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new HeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            // Call
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            PipingCalculation pipingCalculation = assessmentSection.PipingFailureMechanism.CalculationsGroup.Children[0] as PipingCalculation;
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            HeightStructuresCalculation heightStructuresCalculation = assessmentSection.HeightStructures.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(pipingCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionInwardsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(heightStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultiplePipingFailureMechanisms_ClearsHydraulicBoundaryLocations()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            var failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            var failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsHydraulicBoundaryLocations()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            var failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            var failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultipleHeightStructuresFailureMechanisms_ClearsHydraulicBoundaryLocations()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            var failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            var failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyCalculationObservers_Always_NotifiesAllCalculationsInAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(3);
            mocks.ReplayAll();

            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput());
            pipingCalculation.Attach(observer);
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            grassCoverErosionInwardsCalculation.Attach(observer);
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation();
            heightStructuresCalculation.Attach(observer);

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            // Call
            RingtoetsDataSynchronizationService.NotifyCalculationObservers(assessmentSection);

            // Assert
            mocks.VerifyAll();
        }
    }
}