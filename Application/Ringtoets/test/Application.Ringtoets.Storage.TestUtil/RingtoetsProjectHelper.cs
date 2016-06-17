﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// This class can be used to create <see cref="Project"/> instances which have their properties set and can be used in tests.
    /// </summary>
    public static class RingtoetsProjectHelper
    {
        /// <summary>
        /// Returns a new complete instance of <see cref="Project"/>.
        /// </summary>
        /// <returns>A new complete instance of <see cref="Project"/>.</returns>
        public static Project GetFullTestProject()
        {
            ReferenceLine referenceLine = GetReferenceLine();
            Point2D[] referenceLineGeometryPoints = referenceLine.Points.ToArray();

            PipingSoilProfile pipingSoilProfile = new TestPipingSoilProfile();
            PipingSoilLayer pipingSoilLayer = pipingSoilProfile.Layers.First();
            pipingSoilLayer.AbovePhreaticLevel = 1.1;
            pipingSoilLayer.BelowPhreaticLevel = 2.2;
            pipingSoilLayer.DryUnitWeight = 3.3;
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSection",
                HydraulicBoundaryDatabase = GetHydraulicBoundaryDatabase(),
                ReferenceLine = referenceLine,
                PipingFailureMechanism =
                {
                    StochasticSoilModels =
                    {
                        new StochasticSoilModel(-1, "modelName", "modelSegmentName")
                        {
                            Geometry =
                            {
                                referenceLineGeometryPoints[1],
                                referenceLineGeometryPoints[2],
                                referenceLineGeometryPoints[3]
                            },
                            StochasticSoilProfiles =
                            {
                                new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, -1)
                                {
                                    SoilProfile = pipingSoilProfile
                                },
                                new StochasticSoilProfile(0.8, SoilProfileType.SoilProfile1D, -1)
                                {
                                    SoilProfile = new TestPipingSoilProfile()
                                }
                            }
                        }
                    },
                    SurfaceLines =
                    {
                        GetSurfaceLine()
                    }
                }
            };
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
            CalculationGroup pipingCalculationGroup = pipingFailureMechanism.CalculationsGroup;
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "A",
                Children =
                {
                    new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
                    {
                        Name = "AA",
                        IsRelevant = true,
                        Contribution = (RoundedDouble)1.0,
                        Comments = "Nice comment about this calculation!",
                        InputParameters =
                        {
                            SurfaceLine = pipingFailureMechanism.SurfaceLines.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            StochasticSoilModel = pipingFailureMechanism.StochasticSoilModels.First(),
                            StochasticSoilProfile = pipingFailureMechanism.StochasticSoilModels.First()
                                                                          .StochasticSoilProfiles.First(),
                            EntryPointL = (RoundedDouble)1.0,
                            ExitPointL = (RoundedDouble)2.0,
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble)1.1,
                                StandardDeviation = (RoundedDouble)2.2
                            },
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble)3.3,
                                StandardDeviation = (RoundedDouble)4.4
                            },
                            SaturatedVolumicWeightOfCoverageLayer =
                            {
                                Mean = (RoundedDouble)5.5,
                                StandardDeviation = (RoundedDouble)6.6,
                                Shift = (RoundedDouble)7.7
                            },
                            Diameter70 =
                            {
                                Mean = (RoundedDouble)8.8,
                                StandardDeviation = (RoundedDouble)9.9
                            },
                            DarcyPermeability =
                            {
                                Mean = (RoundedDouble)10.10,
                                StandardDeviation = (RoundedDouble)11.11
                            }
                        },
                        Output = new PipingOutput(1.1, 2.2, 3.3, 4.4, 5.5, 6.6),
                        SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(7.7, 8.8, 9.9,
                                                                                    10.10, 11.11, 12.12,
                                                                                    13.13, 14.14, 15.15,
                                                                                    16.16, 17.17,
                                                                                    18.18, 19.19, 20.20)
                    }
                }
            });
            pipingCalculationGroup.Children.Add(new CalculationGroup
            {
                Name = "B"
            });
            pipingCalculationGroup.Children.Add(new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "C",
                IsRelevant = false,
                Contribution = (RoundedDouble)0.5,
                Comments = "Another great comment",
                InputParameters =
                {
                    SurfaceLine = pipingFailureMechanism.SurfaceLines.First(),
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                    StochasticSoilModel = pipingFailureMechanism.StochasticSoilModels.First(),
                    StochasticSoilProfile = pipingFailureMechanism.StochasticSoilModels.First()
                                                                  .StochasticSoilProfiles.Skip(1).First(),
                    EntryPointL = (RoundedDouble)0.3,
                    ExitPointL = (RoundedDouble)2.3,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble)12.12,
                        StandardDeviation = (RoundedDouble)13.13
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)14.14,
                        StandardDeviation = (RoundedDouble)15.15
                    },
                    SaturatedVolumicWeightOfCoverageLayer =
                    {
                        Mean = (RoundedDouble)16.16,
                        StandardDeviation = (RoundedDouble)17.17,
                        Shift = (RoundedDouble)18.18
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)19.19,
                        StandardDeviation = (RoundedDouble)20.20
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)21.21,
                        StandardDeviation = (RoundedDouble)22.22
                    }
                },
                Output = null,
                SemiProbabilisticOutput = null
            });

            var fullTestProject = new Project
            {
                Name = "tempProjectFile",
                Description = "description",
                Items =
                {
                    assessmentSection
                }
            };

            AddSections(pipingFailureMechanism);
            SetSectionResults(pipingFailureMechanism.SectionResults);
            AddSections(assessmentSection.GrassCoverErosionInwards);
            SetSectionResults(assessmentSection.GrassCoverErosionInwards.SectionResults);
            AddSections(assessmentSection.MacrostabilityInwards);
            SetSectionResults(assessmentSection.MacrostabilityInwards.SectionResults);
            AddSections(assessmentSection.MacrostabilityOutwards);
            SetSectionResults(assessmentSection.MacrostabilityOutwards.SectionResults);
            AddSections(assessmentSection.Microstability);
            AddSections(assessmentSection.StabilityStoneCover);
            AddSections(assessmentSection.WaveImpactAsphaltCover);
            SetSectionResults(assessmentSection.WaveImpactAsphaltCover.SectionResults);
            AddSections(assessmentSection.WaterPressureAsphaltCover);
            SetSectionResults(assessmentSection.WaterPressureAsphaltCover.SectionResults);
            AddSections(assessmentSection.GrassCoverErosionOutwards);
            AddSections(assessmentSection.GrassCoverSlipOffOutwards);
            AddSections(assessmentSection.GrassCoverSlipOffInwards);
            AddSections(assessmentSection.HeightStructures);
            SetSectionResults(assessmentSection.HeightStructures.SectionResults);
            AddSections(assessmentSection.ClosingStructure);
            SetSectionResults(assessmentSection.ClosingStructure.SectionResults);
            AddSections(assessmentSection.StrengthStabilityPointConstruction);
            AddSections(assessmentSection.StrengthStabilityLengthwiseConstruction);
            SetSectionResults(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults);
            AddSections(assessmentSection.PipingStructure);
            AddSections(assessmentSection.DuneErosion);
            AddSections(assessmentSection.TechnicalInnovation);
            SetSectionResults(assessmentSection.TechnicalInnovation.SectionResults);

            return fullTestProject;
        }

        private static void SetSectionResults(IEnumerable<PipingFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble) random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<TechnicalInnovationFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<ClosingStructureFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerTwoA = (RoundedDouble)random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerTwoA = (RoundedDouble)random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerTwoA = (RoundedDouble)random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void SetSectionResults(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> sectionResults)
        {
            var random = new Random(21);
            foreach (var sectionResult in sectionResults)
            {
                sectionResult.AssessmentLayerOne = Convert.ToBoolean(random.Next(0, 2));
                sectionResult.AssessmentLayerTwoA = (RoundedDouble)random.NextDouble();
                sectionResult.AssessmentLayerThree = (RoundedDouble)random.NextDouble();
            }
        }

        private static void AddSections(IFailureMechanism failureMechanism)
        {
            failureMechanism.AddSection(new FailureMechanismSection("section 1", new[]
            {
                new Point2D(0, 2),
                new Point2D(2, 3)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("section 2", new[]
            {
                new Point2D(2, 3),
                new Point2D(4, 5)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("section 3", new[]
            {
                new Point2D(4, 5),
                new Point2D(2, 3)
            }));
        }

        private static RingtoetsPipingSurfaceLine GetSurfaceLine()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surfaceline",
                ReferenceLineIntersectionWorldPoint = new Point2D(4.0, 6.0)
            };

            var geometryPoints = new[]
            {
                new Point3D(6.0, 6.0, -2.3),
                new Point3D(5.8, 6.0, -2.3), // Dike toe at river
                new Point3D(5.6, 6.0, 3.4),
                new Point3D(4.2, 6.0, 3.5),
                new Point3D(4.0, 6.0, 0.5), // Dike toe at polder
                new Point3D(3.8, 6.0, 0.5), // Ditch dike side
                new Point3D(3.6, 6.0, 0.2), // Bottom ditch dike side
                new Point3D(3.4, 6.0, 0.25), // Bottom ditch polder side
                new Point3D(3.2, 6.0, 0.5), // Ditch polder side
                new Point3D(3.0, 6.0, 0.5),
            };
            surfaceLine.SetGeometry(geometryPoints);

            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[1]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[4]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[5]);
            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[6]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[7]);
            surfaceLine.SetDitchPolderSideAt(geometryPoints[8]);

            return surfaceLine;
        }

        private static ReferenceLine GetReferenceLine()
        {
            IEnumerable<Point2D> points = new[]
            {
                new Point2D(2, 3),
                new Point2D(5, 4),
                new Point2D(5, 8),
                new Point2D(-3, 2)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);
            return referenceLine;
        }

        private static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "/temp/test",
                Version = "1.0"
            };
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(13001, "test", 152.3, 2938.5)
            {
                DesignWaterLevel = 12.4
            });

            return hydraulicBoundaryDatabase;
        }
    }
}