using System;
using System.IO;
using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Utils.Reflection;
using Ringtoets.HydraRing.Plugin;
using Ringtoets.Integration.Data;
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
            InitializeDemoHydraulicBoundaryDatabase(demoAssessmentSection);
            InitializeDemoPipingData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoHydraulicBoundaryDatabase(DikeAssessmentSection demoAssessmentSection)
        {
            var hydraulicBoundaryDatabase = demoAssessmentSection.HydraulicBoundaryDatabase;

            using (var tempPath = new TemporaryImportFile("HRD_dutchcoastsouth.sqlite"))
            {
                hydraulicBoundaryDatabase.FilePath = tempPath.FilePath;
                var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryLocationsImporter();
                hydraulicBoundaryDatabaseImporter.Import(hydraulicBoundaryDatabase.Locations, tempPath.FilePath);
            }
        }

        private void InitializeDemoPipingData(DikeAssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var tempPath = new TemporaryImportFile("DR6_surfacelines.csv"))
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

        /// <summary>
        /// Class for creating a temporary file in the windows Temp directory based on a
        /// file stored in Embedded Resources.
        /// </summary>
        private class TemporaryImportFile : IDisposable
        {
            private readonly string tempTargetFolderPath;
            private readonly string fullFilePath;

            /// <summary>
            /// Initializes a new instance of the <see cref="TemporaryImportFile"/> class.
            /// </summary>
            /// <param name="embeddedResourceFileName">Name of the file with build action 'Embedded Resource' within this project.</param>
            public TemporaryImportFile(string embeddedResourceFileName)
            {
                tempTargetFolderPath = Path.Combine(Path.GetTempPath(), "demo_traject");
                Directory.CreateDirectory(tempTargetFolderPath);

                fullFilePath = Path.Combine(tempTargetFolderPath, embeddedResourceFileName);

                var stream = GetStreamToFileInResource(embeddedResourceFileName);

                var bytes = GetBinaryDataOfStream(stream);

                File.WriteAllBytes(fullFilePath, bytes);
            }

            public string FilePath
            {
                get
                {
                    return fullFilePath;
                }
            }

            public void Dispose()
            {
                Directory.Delete(tempTargetFolderPath, true);
            }

            private Stream GetStreamToFileInResource(string embeddedResourceFileName)
            {
                return AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly, embeddedResourceFileName);
            }

            private static byte[] GetBinaryDataOfStream(Stream stream)
            {
                var bytes = new byte[stream.Length];
                var reader = new BinaryReader(stream);
                reader.Read(bytes, 0, (int) stream.Length);
                return bytes;
            }
        }
    }
}