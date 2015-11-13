using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Core.Common.Gui;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Demo.Commands
{
    /// <summary>
    /// Command that adds a new Ringtoets project with demo data to the project tree.
    /// </summary>
    public class AddNewDemoProjectCommand : IGuiCommand
    {
        public bool Enabled { get { return true; } }
        public bool Checked { get; set; }

        public void Execute(params object[] arguments)
        {
            var project = Gui.Application.Project;
            project.Items.Add(CreateNewDemoProject());
            project.NotifyObservers();
        }

        private AssessmentSection CreateNewDemoProject()
        {
            var demoAssessmentSection = new AssessmentSection
            {
                Name = "Demo traject"
            };
            InitializeDemoPipingData(demoAssessmentSection);
            return demoAssessmentSection;
        }

        private void InitializeDemoPipingData(AssessmentSection demoAssessmentSection)
        {
            if (demoAssessmentSection.CanAddPipingFailureMechanism())
            {
                demoAssessmentSection.InitializePipingFailureMechanism();
            }

            //var pipingFailureMechanism = demoAssessmentSection.PipingFailureMechanism;

            //using (var tempPath = new TemporaryImportFile("DR6_surfacelines.csv"))
            //{

            //    var surfaceLinesImporter = new PipingSurfaceLinesCsvImporter();
            //    surfaceLinesImporter.ImportItem(tempPath.FilePath, pipingFailureMechanism.SurfaceLines);
            //}
        }

        public IGui Gui { get; set; }

        private class TemporaryImportFile : IDisposable
        {
            private readonly string tempTargetFolderPath;
            private readonly string fullFilePath;

            public TemporaryImportFile(string embeddedResourceFileName)
            {
                tempTargetFolderPath = Path.Combine(Path.GetTempPath(), "demo_traject");
                fullFilePath = Path.Combine(tempTargetFolderPath, embeddedResourceFileName);
                var assembly = Assembly.GetAssembly(GetType());
                var stream = assembly.GetManifestResourceStream(GetType(), "Ringtoets.Demo.Resources."+embeddedResourceFileName);
                var reader = new BinaryReader(stream);
                List<byte> bytes = new List<byte>();
                while (reader.PeekChar() != -1)
                {
                    bytes.Add(reader.ReadByte());
                }
                File.WriteAllBytes(fullFilePath, bytes.ToArray()); 
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
        }
    }
}