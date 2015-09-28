using System;

namespace DelftTools.Utils.Csv.Importer
{
    public class CsvRequiredField
    {
        public CsvRequiredField(string name, Type valueType)
        {
            Name = name;
            ValueType = valueType;
        }

        public string Name { get; set; }
        public Type ValueType { get; set; }
    }
}