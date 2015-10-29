namespace Core.Common.Controls
{
    public interface IRowValidationResult
    {
        /// <summary>
        /// Absolute index of the column
        /// </summary>
        int ColumnIndex { get; }

        /// <summary>
        /// Error text to display
        /// </summary>
        string ErrorText { get; }

        /// <summary>
        /// State of the row (valid / invalid) 
        /// </summary>
        bool Valid { get; }
    }
}