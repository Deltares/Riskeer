using System.Data;

namespace Core.Common.Utils.Csv.Importer
{
    public abstract class CsvFilter
    {
        public abstract bool ShouldSkipRow(DataRow dataRow);
    }
}