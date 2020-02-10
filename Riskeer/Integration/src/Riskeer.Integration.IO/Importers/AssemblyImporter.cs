using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Riskeer.AssemblyTool.IO;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Importers
{
    public class AssemblyImporter : FileImporterBase<ExportableAssessmentSection>
    {
        public AssemblyImporter(ExportableAssessmentSection importTarget, string filePath) 
            : base(filePath, importTarget) {}

        protected override void LogImportCanceledMessage()
        {
        }

        protected override bool OnImport()
        {
            ReadResult<SerializableAssembly> readSerializableAssembly = ReadAssembly();
            if (readSerializableAssembly.CriticalErrorOccurred)
            {
                return false;
            }

            return true;
        }

        private ReadResult<SerializableAssembly> ReadAssembly()
        {
            try
            {
                using (var reader = new SerializableAssemblyReader(FilePath))
                {
                    return new ReadResult<SerializableAssembly>(false)
                    {
                        Items = new[]
                        {
                            reader.Read()
                        }
                    };
                }
            }
            catch (CriticalFileReadException e)
            {
                Log.Error(e.Message);
                return new ReadResult<SerializableAssembly>(true);
            }
        }
    }
}
