using System.Data;

namespace DelftTools.Utils.Csv.Importer
{
    public class CsvPassIfEqualFilter : CsvFilter
    {
        public CsvPassIfEqualFilter(int columnIndex, string passValue)
        {
            ColumnIndex = columnIndex;
            PassValue = passValue;
        }

        public int ColumnIndex { get; set; }
        public string PassValue { get; set; }

        public override bool ShouldSkipRow(DataRow dataRow)
        {
            return !Equals(dataRow[ColumnIndex], PassValue);
        }
    }
}