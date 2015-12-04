namespace Core.Common.Gui.Forms.OptionsDialog
{
    /// <summary>
    /// Used for localizing the items in the theme selection combo box.
    /// </summary>
    public class ColorThemeItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ColorTheme"/> for this item.
        /// </summary>
        public ColorTheme Theme { get; set; }

        /// <summary>
        /// Gets or sets the name to display for the <see cref="ColorTheme"/>.
        /// </summary>
        public string DisplayName { get; set; }
    }
}