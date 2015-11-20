namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// All type categories supported by <see cref="CategoryTreeFolder"/>.
    /// </summary>
    public enum TreeFolderCategory
    {
        /// <summary>
        /// Folder contents to not have a particular meaning.
        /// </summary>
        General,
        /// <summary>
        /// Marks the folder contents as inputs.
        /// </summary>
        Input,
        /// <summary>
        /// Marks the folder contents as outputs.
        /// </summary>
        Output
    }
}