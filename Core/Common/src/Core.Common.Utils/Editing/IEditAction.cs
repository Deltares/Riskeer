namespace Core.Common.Utils.Editing
{
    /// <summary>
    /// Defines custom actions for <see cref="IEditableObject">IEditableObjects</see>.
    /// </summary>
    public interface IEditAction
    {
        /// <summary>
        /// Object being edited.
        /// </summary>
        object Instance { set; }

        /// <summary>
        /// Action arguments.
        /// </summary>
        object[] Arguments { set; }

        /// <summary>
        /// Return value, can be set after the action is performed.
        /// </summary>
        object ReturnValue { set; }

        /// <summary>
        /// TODO: looks like a hack, maybe a naming problem, improve design
        /// </summary>
        bool HandlesRestore { get; }

        /// <summary>
        /// TODO: looks like a hack, maybe anaming problem, improve design
        /// </summary>
        void BeforeChanges();
    }
}