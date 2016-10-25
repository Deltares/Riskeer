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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Utils.IO;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="AssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoAssessmentSectionCommand : ICommand
    {
        private readonly IProjectOwner projectOwner;

        public AddNewDemoAssessmentSectionCommand(IProjectOwner projectOwner)
        {
            this.projectOwner = projectOwner;
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute()
        {
            var project = (RingtoetsProject) projectOwner.Project;
            project.AssessmentSections.Add(CreateNewDemoAssessmentSection());
            project.NotifyObservers();
        }

        private AssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "6-3",
                Name = "Demo traject"
            };
            InitializeDemoReferenceLine(demoAssessmentSection);
            InitializeDemoHydraulicBoundaryDatabase(demoAssessmentSection);
            InitializeDemoFailureMechanismSections(demoAssessmentSection);

            InitializeGrassCoverErosionInwardsData(demoAssessmentSection);
            InitializeGrassCoverErosionOutwardsData(demoAssessmentSection);
            InitializeHeightStructuresData(demoAssessmentSection);
            InitializeClosingStructuresData(demoAssessmentSection);
            InitializeDemoPipingData(demoAssessmentSection);
            InitializeStabilityPointStructuresData(demoAssessmentSection);
            InitializeStabilityStoneCoverData(demoAssessmentSection);
            InitializeWaveImpactAsphaltCoverData(demoAssessmentSection);

            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "traject_6-3.shp", "traject_6-3.dbf", "traject_6-3.prj", "traject_6-3.shx"))
            {
                var importer = new ReferenceLineImporter(demoAssessmentSection,
                                                         Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));
                importer.Import();
            }
        }

        private void InitializeDemoFailureMechanismSections(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "traject_6-3_vakken.shp", "traject_6-3_vakken.dbf", "traject_6-3_vakken.prj", "traject_6-3_vakken.shx"))
            {
                IFailureMechanism[] failureMechanisms = demoAssessmentSection.GetFailureMechanisms().ToArray();
                for (int i = 0; i < failureMechanisms.Length; i++)
                {
                    if (i == 0)
                    {
                        var importer = new FailureMechanismSectionsImporter(failureMechanisms[i], demoAssessmentSection.ReferenceLine,
                                                                            Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));
                        importer.Import();
                    }
                    else
                    {
                        // Copy same FailureMechanismSection instances to other failure mechanisms
                        foreach (FailureMechanismSection section in failureMechanisms[0].Sections)
                        {
                            FailureMechanismSection clonedSection = DeepCloneSection(section);
                            failureMechanisms[i].AddSection(clonedSection);
                        }
                    }
                }
            }
        }

        private static FailureMechanismSection DeepCloneSection(FailureMechanismSection section)
        {
            return new FailureMechanismSection(section.Name,
                                               section.Points.Select(p => new Point2D(p.X, p.Y)));
        }

        #region FailureMechanisms

        #region GrassCoverErosionInwardsFailureMechanism

        private static void InitializeGrassCoverErosionInwardsData(AssessmentSection demoAssessmentSection)
        {
            GrassCoverErosionInwardsFailureMechanism failureMechanism = demoAssessmentSection.GrassCoverErosionInwards;

            var calculation = new GrassCoverErosionInwardsCalculation();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #region GrassCoverErosionOutwardsFailureMechanism

        private static void InitializeGrassCoverErosionOutwardsData(AssessmentSection demoAssessmentSection)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = demoAssessmentSection.GrassCoverErosionOutwards;

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelValues(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            ObservableList<HydraulicBoundaryLocation> locations = failureMechanism.HydraulicBoundaryLocations;
            locations.ElementAt(0).DesignWaterLevel = (RoundedDouble) 7.19;
            locations.ElementAt(1).DesignWaterLevel = (RoundedDouble) 7.19;
            locations.ElementAt(2).DesignWaterLevel = (RoundedDouble) 7.18;
            locations.ElementAt(3).DesignWaterLevel = (RoundedDouble) 7.18;
            locations.ElementAt(4).DesignWaterLevel = (RoundedDouble) 7.18;
            locations.ElementAt(5).DesignWaterLevel = (RoundedDouble) 7.39;
            locations.ElementAt(6).DesignWaterLevel = (RoundedDouble) 7.39;
            locations.ElementAt(7).DesignWaterLevel = (RoundedDouble) 7.39;
            locations.ElementAt(8).DesignWaterLevel = (RoundedDouble) 7.40;
            locations.ElementAt(9).DesignWaterLevel = (RoundedDouble) 7.40;
            locations.ElementAt(10).DesignWaterLevel = (RoundedDouble) 7.40;
            locations.ElementAt(11).DesignWaterLevel = (RoundedDouble) 7.40;
            locations.ElementAt(12).DesignWaterLevel = (RoundedDouble) 7.41;
            locations.ElementAt(13).DesignWaterLevel = (RoundedDouble) 7.41;
            locations.ElementAt(14).DesignWaterLevel = (RoundedDouble) 7.41;
            locations.ElementAt(15).DesignWaterLevel = (RoundedDouble) 6.91;
            locations.ElementAt(16).DesignWaterLevel = (RoundedDouble) 7.53;
            locations.ElementAt(17).DesignWaterLevel = (RoundedDouble) 7.81;
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightValues(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            ObservableList<HydraulicBoundaryLocation> locations = failureMechanism.HydraulicBoundaryLocations;
            locations.ElementAt(0).WaveHeight = (RoundedDouble) 4.99;
            locations.ElementAt(1).WaveHeight = (RoundedDouble) 5.04;
            locations.ElementAt(2).WaveHeight = (RoundedDouble) 4.87;
            locations.ElementAt(3).WaveHeight = (RoundedDouble) 4.73;
            locations.ElementAt(4).WaveHeight = (RoundedDouble) 4.59;
            locations.ElementAt(5).WaveHeight = (RoundedDouble) 3.35;
            locations.ElementAt(6).WaveHeight = (RoundedDouble) 3.83;
            locations.ElementAt(7).WaveHeight = (RoundedDouble) 4.00;
            locations.ElementAt(8).WaveHeight = (RoundedDouble) 4.20;
            locations.ElementAt(9).WaveHeight = (RoundedDouble) 4.41;
            locations.ElementAt(10).WaveHeight = (RoundedDouble) 4.50;
            locations.ElementAt(11).WaveHeight = (RoundedDouble) 4.57;
            locations.ElementAt(12).WaveHeight = (RoundedDouble) 4.63;
            locations.ElementAt(13).WaveHeight = (RoundedDouble) 4.68;
            locations.ElementAt(14).WaveHeight = (RoundedDouble) 4.17;
            locations.ElementAt(15).WaveHeight = (RoundedDouble) 11.14;
            locations.ElementAt(16).WaveHeight = (RoundedDouble) 9.24;
            locations.ElementAt(17).WaveHeight = (RoundedDouble) 5.34;
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
        }

        #endregion

        #region HeightStructuresFailureMechanism

        private static void InitializeHeightStructuresData(AssessmentSection demoAssessmentSection)
        {
            HeightStructuresFailureMechanism failureMechanism = demoAssessmentSection.HeightStructures;
            failureMechanism.HeightStructures.Add(CreateDemoHeightStructure());

            var calculation = new StructuresCalculation<HeightStructuresInput>();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private static HeightStructure CreateDemoHeightStructure()
        {
            return new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Id = "KUNST1", Name = "KUNST1",
                Location = new Point2D(12345.56789, 9876.54321),
                StructureNormalOrientation = 10.0,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) 4.95,
                    StandardDeviation = (RoundedDouble) 0.05
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 25.0,
                    StandardDeviation = (RoundedDouble) 0.05
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 0.1,
                    CoefficientOfVariation = (RoundedDouble) 0.15
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 21.0,
                    CoefficientOfVariation = (RoundedDouble) 0.05
                },
                FailureProbabilityStructureWithErosion = 1.0,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 20000.0,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 0.2,
                    StandardDeviation = (RoundedDouble) 0.1
                }
            });
        }

        #endregion

        #region ClosingStructuresFailureMechanism

        private static void InitializeClosingStructuresData(AssessmentSection demoAssessmentSection)
        {
            ClosingStructuresFailureMechanism failureMechanism = demoAssessmentSection.ClosingStructures;
            failureMechanism.ClosingStructures.Add(CreateDemoClosingStructure());

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private static ClosingStructure CreateDemoClosingStructure()
        {
            return new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Name = "KUNST1", Id = "KUNST1",
                    Location = new Point2D(12345.56789, 9876.54321),
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 20000,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    StructureNormalOrientation = 10.0,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 21,
                        CoefficientOfVariation = (RoundedDouble) 0.05
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 31.5,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 1.0,
                        CoefficientOfVariation = (RoundedDouble) 0.15
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 25.0,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    ProbabilityOpenStructureBeforeFlooding = 1.0,
                    FailureProbabilityOpenStructure = 0.1,
                    IdenticalApertures = 4,
                    FailureProbabilityReparation = 1.0,
                    InflowModelType = ClosingStructureInflowModelType.VerticalWall
                });
        }

        #endregion

        #region PipingFailureMechanism

        private void InitializeDemoPipingData(AssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6_surfacelines.csv", "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter(pipingFailureMechanism.SurfaceLines, demoAssessmentSection.ReferenceLine,
                                                                             Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv"));
                surfaceLinesImporter.Import();
            }

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6.soil"))
            {
                var soilProfilesImporter = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels,
                                                                          Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"));
                soilProfilesImporter.Import();
            }

            var calculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput);
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculation);
            var originalPhreaticLevelExit = calculation.InputParameters.PhreaticLevelExit;
            calculation.InputParameters.PhreaticLevelExit = new NormalDistribution(originalPhreaticLevelExit.Mean.NumberOfDecimalPlaces)
            {
                Mean = (RoundedDouble) 3.0,
                StandardDeviation = originalPhreaticLevelExit.StandardDeviation
            };
            calculation.InputParameters.SurfaceLine = pipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");

            var stochasticSoilModel = pipingFailureMechanism.StochasticSoilModels.First(sm => sm.Name == "PK001_0001_Piping");
            calculation.InputParameters.StochasticSoilModel = stochasticSoilModel;
            calculation.InputParameters.StochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First(sp => sp.SoilProfile.Name == "W1-6_0_1D1");
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #region StabilityPointStructuresFailureMechanism

        private static void InitializeStabilityPointStructuresData(AssessmentSection demoAssessmentSection)
        {
            StabilityPointStructuresFailureMechanism failureMechanism = demoAssessmentSection.StabilityPointStructures;
            failureMechanism.StabilityPointStructures.Add(CreateDemoStabilityPointStructure());

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private static StabilityPointStructure CreateDemoStabilityPointStructure()
        {
            return new StabilityPointStructure(
                new StabilityPointStructure.ConstructionProperties
                {
                    Name = "Kunstwerk", Id = "Kunstwerk id", Location = new Point2D(131470.777221421, 548329.82912364),
                    StructureNormalOrientation = 10,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 20000,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 21.0,
                        CoefficientOfVariation = (RoundedDouble) 0.05
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 0.15
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 25,
                        StandardDeviation = (RoundedDouble) 1.25
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    BankWidth =
                    {
                        Mean = (RoundedDouble) 0,
                        StandardDeviation = (RoundedDouble) 0
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    EvaluationLevel = 0,
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    VerticalDistance = 0,
                    FailureProbabilityRepairClosure = 0.5,
                    FailureCollisionEnergy =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.3
                    },
                    ShipMass =
                    {
                        Mean = (RoundedDouble) 16000,
                        CoefficientOfVariation = (RoundedDouble) 0.2
                    },
                    ShipVelocity =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.2
                    },
                    LevellingCount = 0,
                    ProbabilityCollisionSecondaryStructure = 0,
                    FlowVelocityStructureClosable =
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 15,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 15,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 2.5,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert
                });
        }

        #endregion

        #region StabilityStoneCoverFailureMechanism

        private static void InitializeStabilityStoneCoverData(AssessmentSection demoAssessmentSection)
        {
            StabilityStoneCoverFailureMechanism failureMechanism = demoAssessmentSection.StabilityStoneCover;

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #region WaveImpactAsphaltCoverFailureMechanism

        private static void InitializeWaveImpactAsphaltCoverData(AssessmentSection demoAssessmentSection)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = demoAssessmentSection.WaveImpactAsphaltCover;

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #endregion

        #region HydraulicBoundaryDatabase

        private void InitializeDemoHydraulicBoundaryDatabase(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, false, "HRD dutch coast south.sqlite", "HLCD.sqlite"))
            {
                using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    var filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");
                    hydraulicBoundaryDatabaseImporter.Import(demoAssessmentSection, filePath);
                }
            }

            SetHydraulicBoundaryLocationDesignWaterLevelValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationWaveHeightValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationWaveHeightCalculationConvergence(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);

            demoAssessmentSection.GrassCoverErosionOutwards.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(demoAssessmentSection.HydraulicBoundaryDatabase);
            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelValues(demoAssessmentSection.GrassCoverErosionOutwards);
            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(
                demoAssessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations);

            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightValues(demoAssessmentSection.GrassCoverErosionOutwards);
            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightCalculationConvergence(
                demoAssessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations);
        }

        private static void SetHydraulicBoundaryLocationDesignWaterLevelValues(ICollection<HydraulicBoundaryLocation> locations)
        {
            locations.ElementAt(0).DesignWaterLevel = (RoundedDouble) 5.78;
            locations.ElementAt(1).DesignWaterLevel = (RoundedDouble) 5.77;
            locations.ElementAt(2).DesignWaterLevel = (RoundedDouble) 5.77;
            locations.ElementAt(3).DesignWaterLevel = (RoundedDouble) 5.77;
            locations.ElementAt(4).DesignWaterLevel = (RoundedDouble) 5.77;
            locations.ElementAt(5).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(6).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(7).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(8).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(9).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(10).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(11).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(12).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(13).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(14).DesignWaterLevel = (RoundedDouble) 5.93;
            locations.ElementAt(15).DesignWaterLevel = (RoundedDouble) 5.54;
            locations.ElementAt(16).DesignWaterLevel = (RoundedDouble) 5.86;
            locations.ElementAt(17).DesignWaterLevel = (RoundedDouble) 6.0;
        }

        private static void SetHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
        }

        private static void SetHydraulicBoundaryLocationWaveHeightValues(ICollection<HydraulicBoundaryLocation> locations)
        {
            locations.ElementAt(0).WaveHeight = (RoundedDouble) 4.13374;
            locations.ElementAt(1).WaveHeight = (RoundedDouble) 4.19044;
            locations.ElementAt(2).WaveHeight = (RoundedDouble) 4.01717;
            locations.ElementAt(3).WaveHeight = (RoundedDouble) 3.87408;
            locations.ElementAt(4).WaveHeight = (RoundedDouble) 3.73281;
            locations.ElementAt(5).WaveHeight = (RoundedDouble) 2.65268;
            locations.ElementAt(6).WaveHeight = (RoundedDouble) 3.04333;
            locations.ElementAt(7).WaveHeight = (RoundedDouble) 3.19952;
            locations.ElementAt(8).WaveHeight = (RoundedDouble) 3.3554;
            locations.ElementAt(9).WaveHeight = (RoundedDouble) 3.52929;
            locations.ElementAt(10).WaveHeight = (RoundedDouble) 3.62194;
            locations.ElementAt(11).WaveHeight = (RoundedDouble) 3.6851;
            locations.ElementAt(12).WaveHeight = (RoundedDouble) 3.72909;
            locations.ElementAt(13).WaveHeight = (RoundedDouble) 3.74794;
            locations.ElementAt(14).WaveHeight = (RoundedDouble) 3.29686;
            locations.ElementAt(15).WaveHeight = (RoundedDouble) 9.57558;
            locations.ElementAt(16).WaveHeight = (RoundedDouble) 8.01959;
            locations.ElementAt(17).WaveHeight = (RoundedDouble) 4.11447;
        }

        private static void SetHydraulicBoundaryLocationWaveHeightCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
        }

        #endregion
    }
}