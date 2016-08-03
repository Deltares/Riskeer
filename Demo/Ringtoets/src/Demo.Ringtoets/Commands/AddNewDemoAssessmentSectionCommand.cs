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
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
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

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute(params object[] arguments)
        {
            var project = (RingtoetsProject) projectOwner.Project;
            project.AssessmentSections.Add(CreateNewDemoAssessmentSection());
            project.NotifyObservers();
        }

        private AssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
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
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));
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

            SetHydraulicBoundaryLocationValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
        }

        private void InitializeDemoFailureMechanismSections(AssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "traject_6-3_vakken.shp", "traject_6-3_vakken.dbf", "traject_6-3_vakken.prj", "traject_6-3_vakken.shx"))
            {
                var importer = new FailureMechanismSectionsImporter();

                IFailureMechanism[] failureMechanisms = demoAssessmentSection.GetFailureMechanisms().ToArray();
                for (int i = 0; i < failureMechanisms.Length; i++)
                {
                    if (i == 0)
                    {
                        var context = new FailureMechanismSectionsContext(failureMechanisms[i], demoAssessmentSection);
                        importer.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));
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
                section.Points.Select(p=> new Point2D(p.X, p.Y)));
        }

        private void InitializeDemoPipingData(AssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6_surfacelines.csv", "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter();
                var context = new RingtoetsPipingSurfaceLinesContext(pipingFailureMechanism, demoAssessmentSection);
                surfaceLinesImporter.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv"));
            }

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, true, "DR6.soil"))
            {
                var soilProfilesImporter = new PipingSoilProfilesImporter();
                var context = new StochasticSoilModelContext(pipingFailureMechanism.StochasticSoilModels, pipingFailureMechanism, demoAssessmentSection);
                soilProfilesImporter.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"));
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

        private void SetHydraulicBoundaryLocationValues(ICollection<HydraulicBoundaryLocation> locations)
        {
            locations.ElementAt(0).DesignWaterLevel = 5.78;
            locations.ElementAt(1).DesignWaterLevel = 5.77;
            locations.ElementAt(2).DesignWaterLevel = 5.77;
            locations.ElementAt(3).DesignWaterLevel = 5.77;
            locations.ElementAt(4).DesignWaterLevel = 5.77;
            locations.ElementAt(5).DesignWaterLevel = 5.93;
            locations.ElementAt(6).DesignWaterLevel = 5.93;
            locations.ElementAt(7).DesignWaterLevel = 5.93;
            locations.ElementAt(8).DesignWaterLevel = 5.93;
            locations.ElementAt(9).DesignWaterLevel = 5.93;
            locations.ElementAt(10).DesignWaterLevel = 5.93;
            locations.ElementAt(11).DesignWaterLevel = 5.93;
            locations.ElementAt(12).DesignWaterLevel = 5.93;
            locations.ElementAt(13).DesignWaterLevel = 5.93;
            locations.ElementAt(14).DesignWaterLevel = 5.93;
            locations.ElementAt(15).DesignWaterLevel = 5.54;
            locations.ElementAt(16).DesignWaterLevel = 5.86;
            locations.ElementAt(17).DesignWaterLevel = 6.0;
        }
    }
}