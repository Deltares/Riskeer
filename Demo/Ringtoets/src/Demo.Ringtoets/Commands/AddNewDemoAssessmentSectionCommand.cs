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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Utils.IO;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

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
            InitializeDemoPipingData(demoAssessmentSection);
            InitializeGrassCoverErosionInwardsData(demoAssessmentSection);
            InitializeHeightStructuresData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "traject_6-3.shp", "traject_6-3.dbf", "traject_6-3.prj", "traject_6-3.shx"))
            {
                var importer = new ReferenceLineImporter(demoAssessmentSection);
                importer.Import(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, false, "HRD dutch coast south.sqlite", "HLCD.sqlite"))
            using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
            {
                var filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");
                hydraulicBoundaryDatabaseImporter.Import(demoAssessmentSection, filePath);
            }

            SetHydraulicBoundaryLocationDesignWaterLevelValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationWaveHeightValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationWaveHeightCalculationConvergence(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
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
                        var importer = new FailureMechanismSectionsImporter(failureMechanisms[i], demoAssessmentSection.ReferenceLine);
                        importer.Import(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));
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

        private void InitializeDemoPipingData(AssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6_surfacelines.csv", "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter(pipingFailureMechanism.SurfaceLines, demoAssessmentSection.ReferenceLine);
                surfaceLinesImporter.Import(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv"));
            }

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6.soil"))
            {
                var soilProfilesImporter = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels);
                soilProfilesImporter.Import(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"));
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

        private void InitializeGrassCoverErosionInwardsData(AssessmentSection demoAssessmentSection)
        {
            GrassCoverErosionInwardsFailureMechanism failureMechanism = demoAssessmentSection.GrassCoverErosionInwards;

            var calculation = new GrassCoverErosionInwardsCalculation();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private void InitializeHeightStructuresData(AssessmentSection demoAssessmentSection)
        {
            HeightStructuresFailureMechanism failureMechanism = demoAssessmentSection.HeightStructures;

            var calculation = new HeightStructuresCalculation();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.NotifyObservers();
        }

        private void SetHydraulicBoundaryLocationDesignWaterLevelValues(ICollection<HydraulicBoundaryLocation> locations)
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

        private void SetHydraulicBoundaryLocationDesignWaterLevelCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
            locations.ElementAt(15).DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;
        }

        private void SetHydraulicBoundaryLocationWaveHeightValues(ICollection<HydraulicBoundaryLocation> locations)
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

        private void SetHydraulicBoundaryLocationWaveHeightCalculationConvergence(ICollection<HydraulicBoundaryLocation> locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged;
            }
        }
    }
}