using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Placeholders;
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
            InitializeDemoFailureMechanismSections(demoAssessmentSection);
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
            using (var tempPath = new TemporaryImportFile("HRD_dutchcoastsouth.sqlite", "HLCD.sqlite"))
            {
                using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    hydraulicBoundaryDatabaseImporter.ValidateAndConnectTo(tempPath.FilePath);
                    hydraulicBoundaryDatabaseImporter.Import(new HydraulicBoundaryDatabaseContext(demoAssessmentSection));
                }
            }
        }

        private void InitializeDemoFailureMechanismSections(DuneAssessmentSection demoAssessmentSection)
        {
            using (var temporaryShapeFile = new TemporaryImportFile("traject_10-1_vakken.shp",
                                                                    "traject_10-1_vakken.dbf", "traject_10-1_vakken.prj", "traject_10-1_vakken.shx"))
            {
                var importer = new FailureMechanismSectionsImporter();
                foreach (IFailureMechanism failureMechanism in demoAssessmentSection.GetFailureMechanisms())
                {
                    var context = new FailureMechanismSectionsContext(failureMechanism, demoAssessmentSection);
                    importer.Import(context, temporaryShapeFile.FilePath);
                }
            }
        }
    }
}