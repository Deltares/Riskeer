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
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Revetment.Data;
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
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
            PipingCalculation calculation1 = (PipingCalculation) failureMechanism1.CalculationsGroup.Children[0];
            PipingCalculation calculation2 = (PipingCalculation) failureMechanism2.CalculationsGroup.Children[0];
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
            GrassCoverErosionInwardsCalculation calculation1 = (GrassCoverErosionInwardsCalculation) failureMechanism1.CalculationsGroup.Children[0];
            GrassCoverErosionInwardsCalculation calculation2 = (GrassCoverErosionInwardsCalculation) failureMechanism2.CalculationsGroup.Children[0];
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
            var failureMechanism1 = new HeightStructuresFailureMechanism();
            failureMechanism1.CalculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });
            var failureMechanism2 = new HeightStructuresFailureMechanism();
            failureMechanism2.CalculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>
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
            StructuresCalculation<HeightStructuresInput> calculation1 = (StructuresCalculation<HeightStructuresInput>) failureMechanism1.CalculationsGroup.Children[0];
            StructuresCalculation<HeightStructuresInput> calculation2 = (StructuresCalculation<HeightStructuresInput>) failureMechanism2.CalculationsGroup.Children[0];
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithoutAssessmentSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);
            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestPipingOutput()
            };

            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyClosingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyStabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyStabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var emptyGrassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            var emptyWaveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var waveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(emptyClosingStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(closingStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(emptyStabilityPointStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(stabilityPointStructuresCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(emptyStabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(stabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(grassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(emptyGrassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(emptyWaveImpactAshpaltCoverWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(waveImpactAshpaltCoverWaveConditionsCalculation);

            // Call

            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionInwardsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(heightStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(stabilityStoneCoverWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionOutwardsWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(waveImpactAshpaltCoverWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
            Assert.IsNull(closingStructuresCalculation.Output);
            Assert.IsNull(stabilityPointStructuresCalculation.Output);
            Assert.IsNull(stabilityStoneCoverWaveConditionsCalculation.Output);
            Assert.IsNull(grassCoverErosionOutwardsWaveConditionsCalculation.Output);
            Assert.IsNull(waveImpactAshpaltCoverWaveConditionsCalculation.Output);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                stabilityStoneCoverWaveConditionsCalculation,
                waveImpactAshpaltCoverWaveConditionsCalculation,
                grassCoverErosionOutwardsWaveConditionsCalculation,
                heightStructuresCalculation,
                closingStructuresCalculation,
                stabilityPointStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationNoOutput_ClearsHydraulicBoundaryLocationAndReturnsAffectedCalculations()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0);
            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyClosingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyStabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyStabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyGrassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var emptyWaveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var waveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
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
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(emptyClosingStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(closingStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(emptyStabilityPointStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(stabilityPointStructuresCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(emptyStabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(stabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(emptyGrassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(grassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(emptyWaveImpactAshpaltCoverWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(waveImpactAshpaltCoverWaveConditionsCalculation);

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionInwardsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(heightStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(closingStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(stabilityPointStructuresCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(stabilityStoneCoverWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(grassCoverErosionOutwardsWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(waveImpactAshpaltCoverWaveConditionsCalculation.InputParameters.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                stabilityStoneCoverWaveConditionsCalculation,
                waveImpactAshpaltCoverWaveConditionsCalculation,
                grassCoverErosionOutwardsWaveConditionsCalculation,
                heightStructuresCalculation,
                closingStructuresCalculation,
                stabilityPointStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithOutputAndNoHydraulicBoundaryLocation_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyClosingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var closingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyStabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var stabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var emptyStabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var stabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var emptyGrassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var grassCoverErosionOutwardsWaveConditionsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            var emptyWaveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var waveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(emptyClosingStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(closingStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(emptyStabilityPointStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(stabilityPointStructuresCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(emptyStabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(stabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(emptyGrassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(grassCoverErosionOutwardsWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(emptyWaveImpactAshpaltCoverWaveConditionsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(waveImpactAshpaltCoverWaveConditionsCalculation);

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
            Assert.IsNull(closingStructuresCalculation.Output);
            Assert.IsNull(stabilityPointStructuresCalculation.Output);
            Assert.IsNull(stabilityStoneCoverWaveConditionsCalculation.Output);
            Assert.IsNull(grassCoverErosionOutwardsWaveConditionsCalculation.Output);
            Assert.IsNull(waveImpactAshpaltCoverWaveConditionsCalculation.Output);
            CollectionAssert.AreEqual(new ICalculation[]
            {
                pipingCalculation,
                grassCoverErosionInwardsCalculation,
                stabilityStoneCoverWaveConditionsCalculation,
                waveImpactAshpaltCoverWaveConditionsCalculation,
                grassCoverErosionOutwardsWaveConditionsCalculation,
                heightStructuresCalculation,
                closingStructuresCalculation,
                stabilityPointStructuresCalculation
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithoutOutputAndHydraulicBoundaryLocation_ReturnNoAffectedItems()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var emptyClosingStructuresCalculation = new StructuresCalculation<ClosingStructuresInput>();
            var emptyStabilityPointStructuresCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var emptyStabilityStoneCoverWaveConditionsCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var emptyGrassCoverErosionOutwardsCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var emptyWaveImpactAshpaltCoverWaveConditionsCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(emptyClosingStructuresCalculation);
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(emptyStabilityPointStructuresCalculation);
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(emptyStabilityStoneCoverWaveConditionsCalculation);
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(emptyGrassCoverErosionOutwardsCalculation);
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(emptyWaveImpactAshpaltCoverWaveConditionsCalculation);

            // Call
            IEnumerable<ICalculation> affectedItems = RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate test = () => RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [TestCase(1.0, 3.0)]
        [TestCase(3.8, double.NaN)]
        [TestCase(double.NaN, 6.9)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithData_ClearsDataAndReturnsTrue(double waveHeight, double designWaterLevel)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0)
            {
                WaveHeight = (RoundedDouble) waveHeight,
                DesignWaterLevel = (RoundedDouble) designWaterLevel
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutWaveHeightAndDesignWaterLevel_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase, assessmentSection.GrassCoverErosionOutwards);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        [TestCase(3.5, double.NaN)]
        [TestCase(double.NaN, 8.3)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutDataAndGrassCoverErosionOutwardsLocationWithData_ClearDataAndReturnTrue(double designWaterLevel, double waveHeight)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) designWaterLevel;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight = (RoundedDouble) waveHeight;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    grassCoverErosionOutwardsHydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsTrue(affected);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutDataAndGrassCoverErosionOutwardsLocationWithoutData_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    hydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsFalse(affected);
        }

        [Test]
        [TestCase(3.5, double.NaN)]
        [TestCase(double.NaN, 8.3)]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithDataAndGrassCoverErosionOutwardsLocationWithData_ClearDataAndReturnTrue(double designWaterLevel, double waveHeight)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) designWaterLevel,
                WaveHeight = (RoundedDouble) waveHeight
            };
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    grassCoverErosionOutwardsHydraulicBoundaryLocation
                }
            };

            // Call
            bool affected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryDatabase, failureMechanism);

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            Assert.IsTrue(affected);
        }
    }
}