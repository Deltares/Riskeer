// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Util.IO;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Demo.Riskeer.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="AssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoAssessmentSectionCommand : ICommand
    {
        private readonly IProjectOwner projectOwner;
        private readonly IViewCommands viewCommands;

        public AddNewDemoAssessmentSectionCommand(IProjectOwner projectOwner, IViewCommands viewCommands)
        {
            this.projectOwner = projectOwner;
            this.viewCommands = viewCommands;
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

        private void InitializeDemoReferenceLine(IAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "traject_6-3.shp",
                                                                                   "traject_6-3.dbf",
                                                                                   "traject_6-3.prj",
                                                                                   "traject_6-3.shx"))
            {
                var importer = new ReferenceLineImporter(demoAssessmentSection.ReferenceLine,
                                                         new ReferenceLineUpdateHandler(demoAssessmentSection, viewCommands),
                                                         Path.Combine(embeddedResourceFileWriter.TargetFolderPath,
                                                                      "traject_6-3.shp"));
                importer.Import();
            }
        }

        private void InitializeDemoFailureMechanismSections(IAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "traject_6-3_vakken.shp",
                                                                                   "traject_6-3_vakken.dbf",
                                                                                   "traject_6-3_vakken.prj",
                                                                                   "traject_6-3_vakken.shx"))
            {
                IFailureMechanism[] failureMechanisms = demoAssessmentSection.GetFailureMechanisms().ToArray();

                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath,
                                               "traject_6-3_vakken.shp");

                for (var i = 0; i < failureMechanisms.Length; i++)
                {
                    IFailureMechanism failureMechanism = failureMechanisms[i];

                    if (i == 0)
                    {
                        var importer = new FailureMechanismSectionsImporter(failureMechanism,
                                                                            demoAssessmentSection.ReferenceLine,
                                                                            filePath,
                                                                            new FailureMechanismSectionReplaceStrategy(failureMechanism),
                                                                            new ImportMessageProvider());
                        importer.Import();
                    }
                    else
                    {
                        // Copy same FailureMechanismSection instances to other failure mechanisms
                        FailureMechanismSection[] clonedSections = failureMechanisms[0].Sections.Select(DeepCloneSection).ToArray();
                        failureMechanism.SetSections(clonedSections, filePath);
                    }
                }
            }
        }

        private static FailureMechanismSection DeepCloneSection(FailureMechanismSection section)
        {
            return new FailureMechanismSection(section.Name,
                                               section.Points.Select(p => new Point2D(p)).ToArray());
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

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.NotifyObservers();
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelOutputValues(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            const double targetProbability = 1.0 / 200000;
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm;

            calculations.ElementAt(0).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.19,
                targetProbability, 4.79014,
                1.0 / 1196727, 4.78959,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(1).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.19,
                targetProbability, 4.79014,
                1.0 / 1196727, 4.78959,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(2).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.18,
                targetProbability, 4.79014,
                1.0 / 1196727, 4.78959,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(3).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.18,
                targetProbability, 4.79014,
                1.0 / 1196787, 4.78960,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(4).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.18,
                targetProbability, 4.79014,
                1.0 / 1196787, 4.78960,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(5).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.39,
                targetProbability, 4.79014,
                1.0 / 1196489, 4.78955,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(6).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.39,
                targetProbability, 4.79014,
                1.0 / 1196489, 4.78955,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(7).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.39,
                targetProbability, 4.79014,
                1.0 / 1196489, 4.78955,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(8).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.40,
                targetProbability, 4.79014,
                1.0 / 1196489, 4.78955,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(9).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.40,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(10).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.40,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(11).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.40,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(12).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.40,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(13).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.41,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(14).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.41,
                targetProbability, 4.79014,
                1.0 / 1196429, 4.78954,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(15).Output = new HydraulicBoundaryLocationCalculationOutput(
                6.91,
                targetProbability, 4.79014,
                1.0 / 1197264, 4.78968,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(16).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.53,
                targetProbability, 4.79014,
                1.0 / 1195476, 4.78938,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(17).Output = new HydraulicBoundaryLocationCalculationOutput(
                7.80,
                targetProbability, 4.79014,
                1.0 / 1194761, 4.78926,
                CalculationConvergence.CalculatedConverged,
                null);
        }

        private static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightOutputValues(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            const double targetProbability = 1.0 / 200000;
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm;
            calculations.ElementAt(0).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.99,
                targetProbability, 4.79014,
                1.0 / 1199892, 4.79012,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(1).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.04,
                targetProbability, 4.79014,
                1.0 / 1199892, 4.79012,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(2).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.87,
                targetProbability, 4.79014,
                1.0 / 1199892, 4.79012,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(3).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.73,
                targetProbability, 4.79014,
                1.0 / 1199892, 4.79012,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(4).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.59,
                targetProbability, 4.79014,
                1.0 / 1199833, 4.79011,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(5).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.35,
                targetProbability, 4.79014,
                1.0 / 1197264, 4.78968,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(6).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.83,
                targetProbability, 4.79014,
                1.0 / 1196906, 4.78962,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(7).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.00,
                targetProbability, 4.79014,
                1.0 / 1197264, 4.78968,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(8).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.20,
                targetProbability, 4.79014,
                1.0 / 1197324, 4.78969,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(9).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.41,
                targetProbability, 4.79014,
                1.0 / 1197324, 4.78969,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(10).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.50,
                targetProbability, 4.79014,
                1.0 / 1197622, 4.78974,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(11).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.57,
                targetProbability, 4.79014,
                1.0 / 1197145, 4.78966,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(12).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.63,
                targetProbability, 4.79014,
                1.0 / 1196608, 4.78957,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(13).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.68,
                targetProbability, 4.79014,
                1.0 / 1196549, 4.78956,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(14).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.17,
                targetProbability, 4.79014,
                1.0 / 1199713, 4.79009,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(15).Output = new HydraulicBoundaryLocationCalculationOutput(
                11.13,
                targetProbability, 4.79014,
                1.0 / 201269, 4.79035,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(16).Output = new HydraulicBoundaryLocationCalculationOutput(
                9.24,
                targetProbability, 4.79014,
                1.0 / 197742, 4.78976,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(17).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.34,
                targetProbability, 4.79014,
                1.0 / 199056, 4.78998,
                CalculationConvergence.CalculatedConverged,
                null);
        }

        #endregion

        #region HeightStructuresFailureMechanism

        private static void InitializeHeightStructuresData(AssessmentSection demoAssessmentSection)
        {
            HeightStructuresFailureMechanism failureMechanism = demoAssessmentSection.HeightStructures;
            HeightStructure heightStructure = CreateDemoHeightStructure();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                heightStructure
            }, "heightStructurePath");

            var calculation = new StructuresCalculation<HeightStructuresInput>();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.Structure = heightStructure;
            calculation.InputParameters.NotifyObservers();
        }

        private static HeightStructure CreateDemoHeightStructure()
        {
            return new HeightStructure(
                new HeightStructure.ConstructionProperties
                {
                    Id = "KUNST1",
                    Name = "Kunstwerk 1",
                    Location = new Point2D(12345.56789, 9876.54321),
                    StructureNormalOrientation = (RoundedDouble) 10.0,
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
                        StandardDeviation = (RoundedDouble) 0.05
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
            ClosingStructure closingStructure = CreateDemoClosingStructure();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                closingStructure
            }, "closingStructurePath");

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.Structure = closingStructure;
            calculation.InputParameters.NotifyObservers();
        }

        private static ClosingStructure CreateDemoClosingStructure()
        {
            return new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Id = "KUNST1",
                    Name = "Kunstwerk 1",
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
                    StructureNormalOrientation = (RoundedDouble) 10.0,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 21,
                        StandardDeviation = (RoundedDouble) 0.05
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
            PipingFailureMechanism failureMechanism = demoAssessmentSection.Piping;

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "DR6_surfacelines.csv",
                                                                                   "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new SurfaceLinesCsvImporter<PipingSurfaceLine>(
                    failureMechanism.SurfaceLines,
                    Path.Combine(embeddedResourceFileWriter.TargetFolderPath,
                                 "DR6_surfacelines.csv"),
                    new ImportMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(demoAssessmentSection.Piping, demoAssessmentSection.ReferenceLine));
                surfaceLinesImporter.Import();
            }

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6.soil"))
            {
                var soilProfilesImporter =
                    new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                        failureMechanism.StochasticSoilModels,
                        Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"),
                        new ImportMessageProvider(),
                        PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(failureMechanism));
                soilProfilesImporter.Import();
            }

            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput);
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            NormalDistribution originalPhreaticLevelExit = calculation.InputParameters.PhreaticLevelExit;
            calculation.InputParameters.PhreaticLevelExit = new NormalDistribution(originalPhreaticLevelExit.Mean.NumberOfDecimalPlaces)
            {
                Mean = (RoundedDouble) 3.0,
                StandardDeviation = originalPhreaticLevelExit.StandardDeviation
            };
            calculation.InputParameters.SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");

            PipingStochasticSoilModel stochasticSoilModel = failureMechanism.StochasticSoilModels.First(sm => sm.Name == "PK001_0001_Piping");
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
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                CreateDemoStabilityPointStructure()
            }, "stabilityPointStructurePath");

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
                    Name = "Kunstwerk",
                    Id = "Kunstwerk id",
                    Location = new Point2D(131470.777221421, 548329.82912364),
                    StructureNormalOrientation = (RoundedDouble) 10,
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
                        CoefficientOfVariation = (RoundedDouble) 1
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    CategoryType = AssessmentSectionCategoryType.LowerLimitNorm
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #region WaveImpactAsphaltCoverFailureMechanism

        private static void InitializeWaveImpactAsphaltCoverData(AssessmentSection demoAssessmentSection)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = demoAssessmentSection.WaveImpactAsphaltCover;

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    CategoryType = AssessmentSectionCategoryType.LowerLimitNorm
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        #endregion

        #endregion

        #region HydraulicBoundaryDatabase

        private void InitializeDemoHydraulicBoundaryDatabase(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   false,
                                                                                   "HRD dutch coast south.sqlite",
                                                                                   "HLCD.sqlite",
                                                                                   "HRD dutch coast south.config.sqlite"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");
                DataImportHelper.ImportHydraulicBoundaryDatabase(demoAssessmentSection, filePath);
            }

            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations;

            demoAssessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            SetHydraulicBoundaryLocationDesignWaterLevelOutputValues(demoAssessmentSection);
            SetHydraulicBoundaryLocationWaveHeightOutputValues(demoAssessmentSection);

            demoAssessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationDesignWaterLevelOutputValues(demoAssessmentSection.GrassCoverErosionOutwards);
            SetGrassCoverErosionOutwardsHydraulicBoundaryLocationWaveHeightOutputValues(demoAssessmentSection.GrassCoverErosionOutwards);
        }

        private static void SetHydraulicBoundaryLocationDesignWaterLevelOutputValues(IAssessmentSection assessmentSection)
        {
            const double targetProbability = 1.0 / 30000;

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaterLevelCalculationsForLowerLimitNorm;

            calculations.ElementAt(0).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.78,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(1).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.77,
                targetProbability, 3.98787893,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(2).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.77,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(3).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.77,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(4).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.76865,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(5).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(6).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(7).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(8).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(9).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(10).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(11).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(12).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(13).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(14).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.93,
                targetProbability, 3.98788,
                1.0 / 29995, 3.98784,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(15).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.54,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(16).Output = new HydraulicBoundaryLocationCalculationOutput(
                5.86,
                targetProbability, 3.98788,
                1.0 / 29994, 3.98783,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(17).Output = new HydraulicBoundaryLocationCalculationOutput(
                6.00,
                targetProbability, 3.98788,
                1.0 / 29993, 3.98782,
                CalculationConvergence.CalculatedConverged,
                null);
        }

        private static void SetHydraulicBoundaryLocationWaveHeightOutputValues(IAssessmentSection assessmentSection)
        {
            const double targetProbability = 1.0 / 30000;

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaveHeightCalculationsForLowerLimitNorm;

            calculations.ElementAt(0).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.13,
                targetProbability, 3.98788,
                1.0 / 29972, 3.98766,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(1).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.19,
                targetProbability, 3.98788,
                1.0 / 29962, 3.98770,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(2).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.02,
                targetProbability, 3.98788,
                1.0 / 29977, 3.98758,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(3).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.87,
                targetProbability, 3.98788,
                1.0 / 29963, 3.98759,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(4).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.73,
                targetProbability, 3.98788,
                1.0 / 29957, 3.98754,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(5).Output = new HydraulicBoundaryLocationCalculationOutput(
                2.65,
                targetProbability, 3.98788,
                1.0 / 30022, 3.98805,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(6).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.04,
                targetProbability, 3.98788,
                1.0 / 30001, 3.98789,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(7).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.20,
                targetProbability, 3.98788,
                1.0 / 30000, 3.98788,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(8).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.35,
                targetProbability, 3.98788,
                1.0 / 29996, 3.98785,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(9).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.53,
                targetProbability, 3.98788,
                1.0 / 29999, 3.98787,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(10).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.62,
                targetProbability, 3.98788,
                1.0 / 29888, 3.98699,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(11).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.68,
                targetProbability, 3.98788,
                1.0 / 29890, 3.98701,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(12).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.73,
                targetProbability, 3.98788,
                1.0 / 29882, 3.98694,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(13).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.75,
                targetProbability, 3.98788,
                1.0 / 29902, 3.98710,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(14).Output = new HydraulicBoundaryLocationCalculationOutput(
                3.30,
                targetProbability, 3.98788,
                1.0 / 30037, 3.98817,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(15).Output = new HydraulicBoundaryLocationCalculationOutput(
                9.57,
                targetProbability, 3.98788,
                1.0 / 29999, 3.98787,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(16).Output = new HydraulicBoundaryLocationCalculationOutput(
                8.02,
                targetProbability, 3.98788,
                1.0 / 30108, 3.98873,
                CalculationConvergence.CalculatedConverged,
                null);
            calculations.ElementAt(17).Output = new HydraulicBoundaryLocationCalculationOutput(
                4.11,
                targetProbability, 3.98788,
                1.0 / 29929, 3.98732,
                CalculationConvergence.CalculatedConverged,
                null);
        }

        #endregion
    }
}