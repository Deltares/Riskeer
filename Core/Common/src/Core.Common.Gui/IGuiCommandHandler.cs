namespace Core.Common.Gui
{
    /// <summary>
    /// Handles all common high-level commands in the graphical user interface invoked via menu / toolbar.
    /// </summary>
    public interface IGuiCommandHandler
    {
        /// <summary>
        /// Activates the propertyGrid toolbox
        /// </summary>
        /// <param name="obj"></param>
        void ShowPropertiesFor(object obj);

        /// <summary>
        /// Indicates if there is a property view object for the current <see cref="IGui.Selection"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if a property view is defined, <c>false</c> otherwise.</returns>
        bool CanShowPropertiesFor(object obj);

        void OpenLogFileExternal();
    }
}