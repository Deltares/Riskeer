using System.Collections.Generic;

namespace Core.Common.Utils.Csv.Importer
{
    public class CsvMappingData
    {
        public CsvSettings Settings { get; set; }
        public IDictionary<CsvRequiredField, CsvColumnInfo> FieldToColumnMapping { get; set; }
        public IEnumerable<CsvFilter> Filters { get; set; }
    }
}