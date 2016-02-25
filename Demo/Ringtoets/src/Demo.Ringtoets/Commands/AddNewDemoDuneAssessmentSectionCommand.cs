using Core.Common.Controls.Commands;
using Core.Common.Gui;

using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="DuneAssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoDuneAssessmentSectionCommand : ICommand
    {
        private readonly IProjectOwner projectOwner;

        public AddNewDemoDuneAssessmentSectionCommand(IProjectOwner projectOwner)
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

        private DuneAssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new DuneAssessmentSection
            {
                Name = "Demo duintraject"
            };
            InitializeDemoReferenceLine(demoAssessmentSection);
            InitializeDemoHydraulicBoundaryDatabase(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(DuneAssessmentSection demoAssessmentSection)
        {
            using (var temporaryShapeFile = new TemporaryImportFile("traject_10-1.shp",
                                                                    "traject_10-1.dbf", "traject_10-1.prj", "traject_10-1.shx"))
            {
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), temporaryShapeFile.FilePath);
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DuneAssessmentSection demoAssessmentSection)
        {
            using (var tempPath = new TemporaryImportFile("HRD_dutchcoastsouth.sqlite"))
            {
                using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    hydraulicBoundaryDatabaseImporter.ValidateAndConnectTo(tempPath.FilePath);
                    hydraulicBoundaryDatabaseImporter.Import(new HydraulicBoundaryDatabaseContext(demoAssessmentSection), tempPath.FilePath);
                }
            }
        }
    }
}