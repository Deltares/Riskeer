using System.Configuration;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring the members of the object that holds settings.
    /// </summary>
    public interface ISettingsOwner
    {
        /// <summary>
        /// Gets the fixed settings of the user interface.
        /// </summary>
        GuiCoreSettings FixedSettings { get; }

        /// <summary>
        /// Gets the user specific settings of the user interface.
        /// </summary>
        ApplicationSettingsBase UserSettings { get; }
    }
}