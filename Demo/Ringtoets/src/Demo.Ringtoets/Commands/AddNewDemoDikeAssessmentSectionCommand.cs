using System;
using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
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
            using (var temporaryShapeFile = new TemporaryImportFile("traject_10-1.shp",
                                                                    "traject_10-1.dbf", "traject_10-1.prj", "traject_10-1.shx"))
            {
                var importer = new ReferenceLineImporter();
                importer.Import(new ReferenceLineContext(demoAssessmentSection), temporaryShapeFile.FilePath);
            }
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DikeAssessmentSection demoAssessmentSection)
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

        private void InitializeDemoFailureMechanismSections(DikeAssessmentSection demoAssessmentSection)
        {
            using (var temporaryShapeFile = new TemporaryImportFile("traject_10-1_vakken.shp",
                                                                    "traject_10-1_vakken.dbf", "traject_10-1_vakken.prj", "traject_10-1_vakken.shx"))
            {
                var importer = new FailureMechanismSectionsImporter();
                foreach (var failureMechanism in demoAssessmentSection.GetFailureMechanisms())
                {
                    var context = new FailureMechanismSectionsContext(failureMechanism, demoAssessmentSection);
                    importer.Import(context, temporaryShapeFile.FilePath);
                }
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
            calculation.InputParameters.SetSurfaceLine(pipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001"));
            calculation.InputParameters.SoilProfile = pipingFailureMechanism.SoilProfiles.First(sl => sl.Name == "AD640M00_Segment_36005_1D2");
            calculation.InputParameters.PhreaticLevelExit.Mean = 3;
            calculation.InputParameters.ThicknessCoverageLayer.Mean = Math.Exp(-0.5);
        }
    }
}