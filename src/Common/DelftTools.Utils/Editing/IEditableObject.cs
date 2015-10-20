namespace DelftTools.Utils.Editing
{
    public interface IEditableObject
    {
        /// <summary>
        /// True if object is being edited (potentially in invalid state).
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// Start editing object with the named action. 
        /// </summary>
        /// <param name="action"></param>
        void BeginEdit(IEditAction action);

        /// <summary>
        /// Submit changes to the datasource.
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Revert the changes.
        /// </summary>
        void CancelEdit();
    }
}