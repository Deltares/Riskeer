using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Gui;

using Ringtoets.HydraRing.Plugin;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
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
            InitializeDemoPipingData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(DikeAssessmentSection demoAssessmentSection)
        {
            using (var temporaryShapeFile = new TemporaryImportFile("traject_10-1.shp",
                                                                    "traject_10-1.dbf", "traject_10-1.prj", "traject_10-1.shx"))
            {
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), temporaryShapeFile.FilePath);
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DikeAssessmentSection demoAssessmentSection)
        {
            var hydraulicBoundaryDatabase = demoAssessmentSection.HydraulicBoundaryDatabase;

            using (var tempPath = new TemporaryImportFile("HRD_dutchcoastsouth.sqlite"))
            {
                hydraulicBoundaryDatabase.FilePath = tempPath.FilePath;
                var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryLocationsImporter();
                hydraulicBoundaryDatabaseImporter.ValidateFile(tempPath.FilePath);
                hydraulicBoundaryDatabase.Version = hydraulicBoundaryDatabaseImporter.Version;
                hydraulicBoundaryDatabaseImporter.Import(hydraulicBoundaryDatabase.Locations, tempPath.FilePath);
            }
        }

        private void InitializeDemoPipingData(DikeAssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var tempPath = new TemporaryImportFile("DR6_surfacelines.csv", "DR6_surfacelines.krp.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter();
                surfaceLinesImporter.Import(pipingFailureMechanism.SurfaceLines, tempPath.FilePath);
            }

            using (var tempPath = new TemporaryImportFile("complete.soil"))
            {
                var surfaceLinesImporter = new PipingSoilProfilesImporter();
                surfaceLinesImporter.Import(pipingFailureMechanism.SoilProfiles, tempPath.FilePath);
            }

            var calculation = pipingFailureMechanism.CalculationsGroup.GetPipingCalculations().First();
            calculation.InputParameters.SurfaceLine = pipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");
            calculation.InputParameters.SoilProfile = pipingFailureMechanism.SoilProfiles.First(sl => sl.Name == "AD640M00_Segment_36005_1D2");
        }
    }
}