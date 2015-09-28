namespace DelftTools.Utils.Editing
{
    public interface IEditableObject
    {
        /// <summary>
        /// True if object is being edited (potentially in invalid state).
        /// Note: When IsEditing is set to true, <see cref="CurrentEditAction"/> must not be null.
        /// Note: When IsEditing is set to false, <see cref="CurrentEditAction"/> must not be null.
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// Is set to true if the last edit action was cancelled.
        /// </summary>
        bool EditWasCancelled { get; }

        /// <summary>
        /// Current edit action. 
        /// </summary>
        IEditAction CurrentEditAction { get; }

        /// <summary>
        /// Start editing object with the named action. 
        /// Note: Object must assign <paramref cref="action"/> to <see cref="CurrentEditAction"/> before <see cref="IsEditing"/> is changed.
        /// </summary>
        /// <param name="action"></param>
        void BeginEdit(IEditAction action);

        /// <summary>
        /// Submit changes to the datasource.
        /// Postcondition: After call for last <see cref="IEditAction"/> in stack, <see cref="CurrentEditAction"/> must be null and <see cref="IsEditing"/> must be false.
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Revert the changes.
        /// </summary>
        void CancelEdit();
    }
}