namespace DelftTools.Utils.Csv.Importer
{
    public class CsvSettings
    {
        public char Delimiter { get; set; }
        public bool FirstRowIsHeader { get; set; }
        public bool SkipEmptyLines { get; set; }
    }
}