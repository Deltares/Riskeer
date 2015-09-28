using System.Data;

namespace DelftTools.Utils.Csv.Importer
{
    public abstract class CsvFilter
    {
        public abstract bool ShouldSkipRow(DataRow dataRow);
    }
}