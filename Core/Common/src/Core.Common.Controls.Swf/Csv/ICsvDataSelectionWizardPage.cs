using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Core.Common.Utils.Csv.Importer;

namespace Core.Common.Controls.Swf.Csv
{
    public interface ICsvDataSelectionWizardPage : IWizardPage, IComponent
    {
        IDictionary<CsvRequiredField, CsvColumnInfo> FieldToColumnMapping { get; }
        IEnumerable<CsvFilter> Filters { get; }
        void SetData(DataTable dataTable, IEnumerable<CsvRequiredField> requiredFields);
    }
}