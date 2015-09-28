namespace DelftTools.Shell.Gui.Forms
{
    /// <summary>
    /// Interface to specify controls that are incorporated in the Options dialog. Each implementation should derive from System.Windows.Forms.Control
    /// </summary>
    public interface IOptionsControl
    {
        /// <summary>
        /// Title of the control. Empty titels will be skipped. Controls with a title that matches a category are used as visualization of a category. Others are places as a child of a category node.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Category where this options control should be placed. Empty categories will be skipped.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Called when the user presses the OK button. Changed values should now be committed such that it has an effect on the GUI
        /// </summary>
        void AcceptChanges();

        /// <summary>
        /// Called when the user presses the Cancel button. Settings should be reverted back to what they were before.
        /// </summary>
        void DeclineChanges();

        // TODO: support Image Image { get; } / Image Icon { get; }
    }
}