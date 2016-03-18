using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="DikeAssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoDikeAssessmentSectionCommand : ICommand
    {
        private readonly IProjectOwner projectOwner;

        public AddNewDemoDikeAssessmentSectionCommand(IProjectOwner projectOwner)
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
            var project = projectOwner.Project;
            project.Items.Add(CreateNewDemoAssessmentSection());
            project.NotifyObservers();
        }

        private DikeAssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new DikeAssessmentSection
            {
                Name = "Demo dijktraject"
            };
            InitializeDemoReferenceLine(demoAssessmentSection);
            InitializeDemoHydraulicBoundaryDatabase(demoAssessmentSection);
            InitializeDemoFailureMechanismSections(demoAssessmentSection);
            InitializeDemoPipingData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(DikeAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "traject_6-3.shp", "traject_6-3.dbf", "traject_6-3.prj", "traject_6-3.shx"))
            {
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DikeAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(false, "HRD dutch coast south.sqlite", "HLCD.sqlite"))
            {
                using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    hydraulicBoundaryDatabaseImporter.ValidateAndConnectTo(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite"));
                    hydraulicBoundaryDatabaseImporter.Import(new HydraulicBoundaryDatabaseContext(demoAssessmentSection));
                }
            }
        }

        private void InitializeDemoFailureMechanismSections(DikeAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "traject_6-3_vakken.shp", "traject_6-3_vakken.dbf", "traject_6-3_vakken.prj", "traject_6-3_vakken.shx"))
            {
                var importer = new FailureMechanismSectionsImporter();
                foreach (var failureMechanism in demoAssessmentSection.GetFailureMechanisms())
                {
                    var context = new FailureMechanismSectionsContext(failureMechanism, demoAssessmentSection);
                    importer.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));
                }
            }
        }

        private void InitializeDemoPipingData(DikeAssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "DR6_surfacelines.csv", "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter();
                var context = new RingtoetsPipingSurfaceLinesContext(pipingFailureMechanism, demoAssessmentSection);
                surfaceLinesImporter.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv"));
            }

            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "DR6.soil"))
            {
                var surfaceLinesImporter = new PipingSoilProfilesImporter();
                surfaceLinesImporter.Import(pipingFailureMechanism.SoilProfiles, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"));
            }

            var calculation = pipingFailureMechanism.CalculationsGroup.GetPipingCalculations().First();
            calculation.InputParameters.SurfaceLine = pipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");
            calculation.InputParameters.SoilProfile = pipingFailureMechanism.SoilProfiles.First(sp => sp.Name == "W1-6_0_1D1");
            calculation.InputParameters.HydraulicBoundaryLocation = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            SetHydraulicBoundaryLocationValues(demoAssessmentSection.HydraulicBoundaryDatabase.Locations);
            calculation.InputParameters.PhreaticLevelExit.Mean = (RoundedDouble) 3;
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