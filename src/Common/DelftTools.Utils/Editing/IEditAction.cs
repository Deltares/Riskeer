namespace DelftTools.Utils.Editing
{
    /// <summary>
    /// Defines custom actions for <see cref="IEditableObject">IEditableObjects</see>.
    /// </summary>
    public interface IEditAction
    {
        /// <summary>
        /// Name of the action. Can be based on the instance and/or arguments.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Object being edited.
        /// </summary>
        object Instance { get; set; }

        /// <summary>
        /// Action arguments.
        /// </summary>
        object[] Arguments { get; set; }

        /// <summary>
        /// Return value, can be set after the action is performed.
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// TODO: looks like a hack, maybe a naming problem, improve design
        /// </summary>
        bool HandlesRestore { get; }

        /// <summary>
        /// TODO: looks like a hack, maybe anaming problem, improve design
        /// </summary>
        void Restore();

        /// <summary>
        /// TODO: looks like a hack, maybe a naming problem, improve design
        /// </summary>
        bool SuppressEventBasedRestore { get; }

        /// <summary>
        /// TODO: looks like a hack, maybe anaming problem, improve design
        /// </summary>
        void BeforeChanges();
    }
}