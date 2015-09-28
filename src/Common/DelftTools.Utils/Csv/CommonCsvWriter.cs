using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DelftTools.Utils.Csv
{
    public static class CommonCsvWriter
    {
        public static string WriteToString(DataTable table, bool header, bool quoteall, char delimiter = ',')
        {
            var writer = new StringWriter();
            WriteToStream(writer, table, header, quoteall, true, delimiter);
            return writer.ToString();
        }

        public static void WriteToStream(TextWriter stream, DataTable table, bool header, bool quoteall, bool cultureInvariant = true, char delimiter = ',')
        {
            //store culture (in case we replace it)
            var storedCulture = Thread.CurrentThread.CurrentCulture;
            if (cultureInvariant)
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                var headerLine = table.Columns.Cast<DataColumn>().Aggregate("", (current, column) =>
                                                                                current +
                                                                                ConvertToFileString(
                                                                                    (header
                                                                                         ? column.Caption
                                                                                         : column.ColumnName), quoteall) +
                                                                                delimiter)
                                      .TrimEnd(delimiter) + Environment.NewLine;

                stream.Write(headerLine);

                foreach (DataRow row in table.Rows)
                {
                    var line = row.ItemArray.Aggregate("", (lineData, item) => lineData + ConvertToFileString(item, quoteall) + delimiter);
                    stream.Write(line.Remove(line.Length - 1) + Environment.NewLine);
                }
            }
            finally
            {
                //reset culture (in case it was replaced)
                Thread.CurrentThread.CurrentCulture = storedCulture;
            }
        }

        private static string ConvertToFileString(object item, bool quoteall)
        {
            if (item == null) return "";

            var itemAsString = item.ToString();

            return (quoteall || itemAsString.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
                       ? "\"" + itemAsString.Replace("\"", "\"\"") + "\""
                       : itemAsString;
        }
    }
}
