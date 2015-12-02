using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Core.Common.Gui;

using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Demo.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="DikeAssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoDikeAssessmentSectionCommand : IGuiCommand
    {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked { get; set; }

        public IGui Gui { get; set; }

        public void Execute(params object[] arguments)
        {
            var project = Gui.Project;
            project.Items.Add(CreateNewDemoAssessmentSection());
            project.NotifyObservers();
        }

        private DikeAssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new DikeAssessmentSection
            {
                Name = "Demo dijktraject"
            };
            InitializeDemoPipingData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoPipingData(DikeAssessmentSection demoAssessmentSection)
        {
            var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            using (var tempPath = new TemporaryImportFile("DR6_surfacelines.csv"))
            {
                var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter();
                surfaceLinesImporter.Import(tempPath.FilePath, pipingFailureMechanism.SurfaceLines);
            }

            using (var tempPath = new TemporaryImportFile("complete.soil"))
            {
                var surfaceLinesImporter = new PipingSoilProfilesImporter();
                surfaceLinesImporter.Import(tempPath.FilePath, pipingFailureMechanism.SoilProfiles);
            }

            var calculation = pipingFailureMechanism.Calculations.GetPipingCalculations().First();
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
                var assembly = Assembly.GetAssembly(GetType());
                string embeddedResourceName = assembly.GetManifestResourceNames().First(n => n.EndsWith(embeddedResourceFileName));
                return assembly.GetManifestResourceStream(embeddedResourceName);
            }

            private static byte[] GetBinaryDataOfStream(Stream stream)
            {
                var bytes = new byte[stream.Length];
                var reader = new BinaryReader(stream);
                reader.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
        }
    }
}