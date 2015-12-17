using System;

namespace Core.Common.Controls.Table.Validation
{
    public class RowValidationResult
    {
        public RowValidationResult(string errorText) : this(-1, errorText) //-1 = no specific column, row error
        {}

        public RowValidationResult(int columnIndex, string errorText)
        {
            ColumnIndex = columnIndex;
            ErrorText = errorText;
            Valid = String.IsNullOrEmpty(ErrorText);
        }

        public int ColumnIndex { get; private set; }

        public string ErrorText { get; private set; }

        public bool Valid { get; private set; }
    }
}