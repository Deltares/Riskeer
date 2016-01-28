namespace Core.Common.Gui
{
    /// <summary>
    /// Interface that declares application feature manipulation.
    /// </summary>
    public interface IApplicationFeatureCommands
    {
        /// <summary>
        /// Activates the propertyGrid toolbox
        /// </summary>
        /// <param name="obj"></param>
        void ShowPropertiesFor(object obj);

        /// <summary>
        /// Indicates if there is a property view object for some object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if a property view is defined, <c>false</c> otherwise.</returns>
        bool CanShowPropertiesFor(object obj);

        void OpenLogFileExternal();
    }
}