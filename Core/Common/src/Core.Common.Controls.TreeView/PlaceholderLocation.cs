namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Enum to specify whether or not the mouse is above or below a tree node.
    /// </summary>
    public enum PlaceholderLocation
    {
        /// <summary>
        /// Position the placeholder above the targetnode
        /// </summary>
        Top,

        /// <summary>
        /// position the placeholder below the targetnode
        /// </summary>
        Bottom,

        /// <summary>
        /// position the placeholder next to the targetnode
        /// </summary>
        Middle,

        /// <summary>
        /// do not draw a placeholder
        /// </summary>
        None
    }
}