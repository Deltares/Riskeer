using System.Collections.Generic;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    public class ReadResult<T>
    {
        public ReadResult(bool errorOccurred)
        {
            CriticalErrorOccurred = errorOccurred;
            ImportedItems = new T[0];
        }

        public ICollection<T> ImportedItems { get; set; }

        public bool CriticalErrorOccurred { get; private set; }
    }
}