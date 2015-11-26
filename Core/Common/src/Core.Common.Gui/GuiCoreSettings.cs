namespace Core.Common.Gui
{
    /// <summary>
    /// Container for settings in the graphical user interface.
    /// </summary>
    public class GuiCoreSettings
    {
        /// <summary>
        /// The start page url to use in the graphical user interface.
        /// </summary>
        public string StartPageUrl { get; set; }

        /// <summary>
        /// The support email address to show in the graphical user interface.
        /// </summary>
        public string SupportEmailAddress { get; set; }

        /// <summary>
        /// The support phone number to show in the graphical user interface.
        /// </summary>
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// The copyright to show in the graphical user interface.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The license description to show in the graphical user interface.
        /// </summary>
        public string LicenseDescription { get; set; }

        /// <summary>
        /// The title to show in the main window of the graphical user interface.
        /// </summary>
        public string MainWindowTitle { get; set; }

        /// <summary>
        /// The path of the license file to use in the graphical interface.
        /// </summary>
        public string LicenseFilePath { get; set; }

        /// <summary>
        /// The path of the manual file to use in the graphical interface.
        /// </summary>
        public string ManualFilePath { get; set; }
    }
}
