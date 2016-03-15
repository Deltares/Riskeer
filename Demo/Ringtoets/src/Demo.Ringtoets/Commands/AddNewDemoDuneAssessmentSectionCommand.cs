using System.IO;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
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
            InitializeDemoFailureMechanismSections(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoReferenceLine(DuneAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "traject_6-3.shp", "traject_6-3.dbf", "traject_6-3.prj", "traject_6-3.shx"))
            {
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DuneAssessmentSection demoAssessmentSection)
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

        private void InitializeDemoFailureMechanismSections(DuneAssessmentSection demoAssessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(true, "traject_6-3_vakken.shp", "traject_6-3_vakken.dbf", "traject_6-3_vakken.prj", "traject_6-3_vakken.shx"))
            {
                var importer = new FailureMechanismSectionsImporter();
                foreach (IFailureMechanism failureMechanism in demoAssessmentSection.GetFailureMechanisms())
                {
                    var context = new FailureMechanismSectionsContext(failureMechanism, demoAssessmentSection);
                    importer.Import(context, Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));
                }
            }
        }
    }
}