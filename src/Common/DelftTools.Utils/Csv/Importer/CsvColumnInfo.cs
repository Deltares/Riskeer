using System;

namespace DelftTools.Utils.Csv.Importer
{
    public class CsvColumnInfo
    {
        public CsvColumnInfo(int columnIndex, IFormatProvider formatProvider)
        {
            ColumnIndex = columnIndex;
            FormatProvider = formatProvider;
        }

        public int ColumnIndex { get; private set; }
        public IFormatProvider FormatProvider { get; private set; }
    }
}