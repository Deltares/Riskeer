using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Csv.FormatPickers;
using Core.Common.Utils.Csv.Importer;

namespace Core.Common.Controls.Swf.Csv
{
    public partial class CsvDataSelectionControl : UserControl
    {
        private const int LineOffset = 30;
        private readonly IList<ComboBox> allColumnComboBoxes = new List<ComboBox>();
        private CsvRequiredField[] requiredFields;

        private readonly IDictionary<Type, CsvFormatPickerProvider> formatPickerProviderLookup;

        public CsvDataSelectionControl()
        {
            InitializeComponent();

            formatPickerProviderLookup = new Dictionary<Type, CsvFormatPickerProvider>();
            formatPickerProviderLookup[typeof(double)] = new DoubleCsvFormatPickerProvider();
            formatPickerProviderLookup[typeof(DateTime)] = new DateTimeCsvFormatPickerProvider();

            // filter events
            checkBoxUseFilter.CheckedChanged += UserSelectionChanged;
            filterColumnCombobox.SelectedIndexChanged += UserSelectionChanged;
            textBoxFilter.Leave += UserSelectionChanged;
        }

        public IDictionary<CsvRequiredField, CsvColumnInfo> FieldToColumnMapping
        {
            get
            {
                var dictionary = new Dictionary<CsvRequiredField, CsvColumnInfo>();
                foreach (var field in requiredFields)
                {
                    var dataColumn = GetSelectedColumnForField(field);
                    var formatProvider = formatPickerProviderLookup.ContainsKey(field.ValueType)
                                             ? formatPickerProviderLookup[field.ValueType].GetFormatProvider()
                                             : null;
                    var columnInfo = new CsvColumnInfo(DataTable.Columns.IndexOf(dataColumn), formatProvider);
                    dictionary.Add(field, columnInfo);
                }
                return dictionary;
            }
        }

        public IEnumerable<CsvFilter> Filters
        {
            get
            {
                if (checkBoxUseFilter.Checked)
                {
                    var dataColumn = filterColumnCombobox.SelectedItem as DataColumn;
                    if (dataColumn != null)
                    {
                        yield return new CsvPassIfEqualFilter(DataTable.Columns.IndexOf(dataColumn), textBoxFilter.Text);
                    }
                }
            }
        }

        public bool HasErrors { get; private set; }

        public bool FilteringVisible
        {
            get
            {
                return groupFiltering.Visible;
            }
            set
            {
                groupFiltering.Visible = value;
            }
        }

        public bool ColumnSelectionVisible
        {
            get
            {
                return groupColumnSelection.Visible;
            }
            set
            {
                groupColumnSelection.Visible = value;
            }
        }

        public void RegisterFormatPickerControlForType(Type type, CsvFormatPickerProvider formatPickerProvider)
        {
            formatPickerProviderLookup[type] = formatPickerProvider;
        }

        public void SetData(DataTable dataTable, IEnumerable<CsvRequiredField> csvRequiredFields)
        {
            allColumnComboBoxes.Clear();
            if (dataTable.Rows.Count > 100)
            {
                throw new InvalidOperationException("Performance problem detected; please give a small (preview) datatable as argument, not the actual data table");
            }

            requiredFields = csvRequiredFields as CsvRequiredField[] ?? csvRequiredFields.ToArray();
            AddFormatPickerControls(requiredFields);

            DataTable = dataTable;
            dataGridBefore.DataSource = DataTable;
            AddColumnSelectionControls(dataTable, requiredFields);

            filterColumnCombobox.Items.Clear();
            filterColumnCombobox.Items.AddRange(dataTable.Columns.OfType<object>().ToArray());
            if (filterColumnCombobox.Items.Count > 0)
            {
                filterColumnCombobox.SelectedIndex = 0;
            }

            UserSelectionChanged(null, null);
        }

        private DataTable DataTable { get; set; }

        private DataColumn GetSelectedColumnForField(CsvRequiredField field)
        {
            var columnForFieldCombobox = allColumnComboBoxes.First(cb => cb.Tag == field);
            var dataColumn = (DataColumn) columnForFieldCombobox.SelectedItem;
            return dataColumn;
        }

        private void FieldToColumnMappingChanged(CsvRequiredField requiredField, DataColumn mappedColumn)
        {
            var valueType = requiredField.ValueType;

            if (!formatPickerProviderLookup.ContainsKey(valueType))
            {
                return;
            }

            var formatController = formatPickerProviderLookup[valueType];
            formatController.SetFormatPickerToInitialGuess(
                DataTable.Rows.Cast<DataRow>()
                         .Select(r => r[mappedColumn].ToString()).ToArray());
        }

        private void UserSelectionChanged(object sender, EventArgs e)
        {
            HasErrors = false;

            DataTable resultDataTable;
            try
            {
                resultDataTable = new CsvImporter().Extract(DataTable, FieldToColumnMapping, Filters);
                HasErrors = resultDataTable.HasErrors;
            }
            catch (Exception ex)
            {
                HasErrors = true;
                resultDataTable = new DataTable();
                resultDataTable.Columns.Add(new DataColumn("Error", typeof(string)));
                resultDataTable.Rows.Add(new[]
                {
                    ex.Message
                });
            }
            dataGridAfter.DataSource = resultDataTable;
        }

        private void AddFormatPickerControls(IEnumerable<CsvRequiredField> csvRequiredColumns)
        {
            var uniqueValueTypes = csvRequiredColumns.Select(c => c.ValueType).Distinct();

            groupCultureInfo.Controls.Clear();

            var placementOffset = 20;
            foreach (var valueType in uniqueValueTypes)
            {
                //add format picker controller
                if (!formatPickerProviderLookup.ContainsKey(valueType))
                {
                    continue;
                }
                var formatPickerController = formatPickerProviderLookup[valueType];

                // add label
                groupCultureInfo.Controls.Add(new Label
                {
                    Text = formatPickerController.Label,
                    Left = 10,
                    Width = 150,
                    Top = placementOffset
                });

                formatPickerController.UserSelectionChanged += UserSelectionChanged;

                // add format picker UI element
                var formatPicker = formatPickerController.GetFormatPicker();
                formatPicker.Top = placementOffset;
                formatPicker.Left = 200;

                groupCultureInfo.Controls.Add(formatPicker);

                placementOffset += formatPicker.Height + 5;
            }
            groupCultureInfo.Height = placementOffset + 5;
        }

        private void AddColumnSelectionControls(DataTable dataTable, ICollection<CsvRequiredField> csvRequiredFields)
        {
            allColumnComboBoxes.Clear();
            groupColumnSelection.Controls.Clear();

            var placementOffset = 20;
            int index = 0;

            foreach (var requiredField in csvRequiredFields)
            {
                groupColumnSelection.Controls.Add(new Label
                {
                    Text = requiredField.Name,
                    Left = 10,
                    Width = 150,
                    Top = placementOffset
                });
                var columnsCombobox = new ComboBox
                {
                    Left = 200,
                    Top = placementOffset,
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                columnsCombobox.Font = new Font(columnsCombobox.Font, FontStyle.Bold);

                var columns = dataTable.Columns.OfType<object>().ToArray();
                columnsCombobox.Items.AddRange(columns);
                columnsCombobox.SelectedIndex = Math.Min(index, columns.Length - 1);
                columnsCombobox.Tag = requiredField;

                var field = requiredField;
                columnsCombobox.SelectedIndexChanged +=
                    (s, e) =>
                    {
                        FieldToColumnMappingChanged(field, (DataColumn) columnsCombobox.SelectedItem);
                        UserSelectionChanged(null, null);
                    };

                allColumnComboBoxes.Add(columnsCombobox);

                groupColumnSelection.Controls.Add(columnsCombobox);

                placementOffset += LineOffset;
                index++;
            }

            foreach (var requiredField in csvRequiredFields)
            {
                FieldToColumnMappingChanged(requiredField, GetSelectedColumnForField(requiredField));
            }
            groupColumnSelection.Height = placementOffset + 5;
        }

        private void checkBoxUseFilter_CheckedChanged(object sender, EventArgs e)
        {
            filterColumnCombobox.Enabled = checkBoxUseFilter.Checked;
            textBoxFilter.Enabled = checkBoxUseFilter.Checked;
        }

        private void btApplyFilter_Click(object sender, EventArgs e)
        {
            // empty on purpose, handled by Leave event
        }
    }
}