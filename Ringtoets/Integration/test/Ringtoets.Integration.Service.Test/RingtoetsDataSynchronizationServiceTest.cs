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
        public void ClearAssessmentSectionData_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            HeightStructuresCalculation emptyHeightStructuresCalculation = new HeightStructuresCalculation();
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            // Call
            var affectedItems = RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                heightStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultiplePipingFailureMechanisms_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            PipingFailureMechanism failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            PipingFailureMechanism failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            GrassCoverErosionInwardsFailureMechanism failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAssessmentSectionData_WithMultipleHeightStructuresFailureMechanisms_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });
            HeightStructuresFailureMechanism failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(a => a.HydraulicBoundaryDatabase).Return(null);
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAssessmentSectionData(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithAssessmentSection_ClearsFailureMechanismCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            HeightStructuresCalculation emptyHeightStructuresCalculation = new HeightStructuresCalculation();
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                heightStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultiplePipingFailureMechanisms_ClearsOutputAndReturnsAffectedItems()
        {
            // Setup
            PipingFailureMechanism failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            PipingFailureMechanism failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            GrassCoverErosionInwardsFailureMechanism failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearFailureMechanismCalculationOutputs_WithMultipleHeightStructuresFailureMechanisms_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });
            HeightStructuresFailureMechanism failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.Output);
            Assert.IsNull(calculation2.Output);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithAssessmentSection_ClearsHydraulicBoundaryLocationAndReturnsAffectedCalculations()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);
            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            HeightStructuresCalculation emptyHeightStructuresCalculation = new HeightStructuresCalculation();
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionInwardsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(heightStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                heightStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultiplePipingFailureMechanisms_ClearsHydraulicBoundaryLocationsAndReturnsAffectedItems()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            PipingFailureMechanism failureMechanism1 = new PipingFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            PipingFailureMechanism failureMechanism2 = new PipingFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            PipingCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as PipingCalculation;
            PipingCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as PipingCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultipleGrassCoverErosionInwardsFailureMechanisms_ClearsHydraulicBoundaryLocationsAndReturnsAffectedItems()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            GrassCoverErosionInwardsFailureMechanism failureMechanism1 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            GrassCoverErosionInwardsFailureMechanism failureMechanism2 = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            GrassCoverErosionInwardsCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            GrassCoverErosionInwardsCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearHydraulicBoundaryLocationFromCalculations_WithMultipleHeightStructuresFailureMechanisms_ClearsHydraulicBoundaryLocationsAndReturnsAffectedItems()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);

            HeightStructuresFailureMechanism failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
            HeightStructuresFailureMechanism failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });

            MockRepository mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationFromCalculations(assessmentSection);

            // Assert
            HeightStructuresCalculation calculation1 = failureMechanism1.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            HeightStructuresCalculation calculation2 = failureMechanism2.CalculationsGroup.Children[0] as HeightStructuresCalculation;
            Assert.IsNull(calculation1.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation2.InputParameters.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyCalculationObservers_Always_NotifiesAllCalculationsInAssessmentSection()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            IObserver observer = mocks.StrictMock<IObserver>();
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

        [Test]
        public void NotifyCalculationObservers_Always_NotifiesGivenCalculations()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            IObserver observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(3);
            mocks.ReplayAll();

            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput());
            pipingCalculation.Attach(observer);
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            grassCoverErosionInwardsCalculation.Attach(observer);
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation();
            heightStructuresCalculation.Attach(observer);

            // Call
            RingtoetsDataSynchronizationService.NotifyCalculationObservers(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                heightStructuresCalculation
            });

            // Assert
            mocks.VerifyAll();
        }
    }
}