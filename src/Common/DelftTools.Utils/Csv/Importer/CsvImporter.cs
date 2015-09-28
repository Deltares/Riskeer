using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using LumenWorks.Framework.IO.Csv;

namespace DelftTools.Utils.Csv.Importer
{
    // TODO: Add support for ShouldCancel and ProgressChanged
    public class CsvImporter
    {
        public CsvImporter()
        {
            AllowEmptyCells = false;
        }

        public DataTable SplitToTable(StreamReader streamReader, CsvSettings csvSettings)
        {
            var dataTable = new DataTable();
            using (var csvReader = new CsvReader(streamReader, csvSettings.FirstRowIsHeader, csvSettings.Delimiter))
            {
                csvReader.SkipEmptyLines = csvSettings.SkipEmptyLines;
                try
                {
                    dataTable.Load(csvReader);
                }
                catch
                {
                    //any error result in an empty data table 
                    dataTable = new DataTable();
                }
            }
            return dataTable;
        }

        public DataTable SplitToTable(string path, CsvSettings csvSettings)
        {
            using (var streamReader = new StreamReader(path))
            {
                return SplitToTable(streamReader, csvSettings);
            }
        }

        private object Parse(object o, Type type, CsvColumnInfo info)
        {
            if (o is DBNull && AllowEmptyCells)
            {
                return o;
            }
            if (type == typeof (DateTime))
            {
                var format = info.FormatProvider as DateTimeFormatInfo;
                return DateTime.ParseExact((string)o, format.FullDateTimePattern, format);
            }
            return info != null
                       ? Convert.ChangeType(o, type, info.FormatProvider)
                       : Convert.ChangeType(o, type); // e.g. for string
        }

        public bool AllowEmptyCells { get; set; }

        public DataTable Extract(DataTable dataTable, 
            IDictionary<CsvRequiredField, CsvColumnInfo> fieldToColumnInfoMapping, 
            IEnumerable<CsvFilter> filters=null)
        {
            filters = filters ?? new CsvFilter[0];
            
            var typedDataTable = new DataTable();

            var csvRequiredFields = fieldToColumnInfoMapping.Keys.ToList();
            foreach (var field in csvRequiredFields)
            {
                typedDataTable.Columns.Add(new DataColumn(field.Name, field.ValueType));
            }

            foreach (DataRow oldRow in dataTable.Rows)
            {
                if (SkipRow(oldRow, filters))
                    continue;

                var newRow = typedDataTable.NewRow();
                for (int i = 0; i < typedDataTable.Columns.Count; i++)
                {
                    var field = csvRequiredFields[i];
                    var info = fieldToColumnInfoMapping[field];
                    try
                    {
                        newRow[i] = Parse(oldRow[info.ColumnIndex], field.ValueType, info);
                    }
                    catch (Exception ex)
                    {
                        newRow.SetColumnError(i, ex.Message);
                    }
                }
                typedDataTable.Rows.Add(newRow);
            }

            return typedDataTable;
        }

        private bool SkipRow(DataRow oldRow, IEnumerable<CsvFilter> filters)
        {
            return filters.Any(filter => filter.ShouldSkipRow(oldRow));
        }

        public DataTable ImportCsv(string filePath, CsvMappingData mappingData)
        {
            var dtTemp = SplitToTable(filePath, mappingData.Settings);
            return Extract(dtTemp, mappingData.FieldToColumnMapping, mappingData.Filters);
        }
    }
}