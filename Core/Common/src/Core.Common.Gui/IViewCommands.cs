namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring methods related to manipulating views within the application.
    /// </summary>
    public interface IViewCommands
    {
        /// <summary>
        /// Presents the user with a dialog to choose an editor for the selected dataitem
        /// </summary>
        void OpenSelectViewDialog();

        void OpenViewForSelection();

        void OpenView(object dataObject);

        void RemoveAllViewsForItem(object dataObject);

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if there is a default view for the current selection</returns>
        bool CanOpenViewFor(object obj);

        /// <summary>
        /// </summary>
        /// <returns>true if there are more supported views for the current selection</returns>
        bool CanOpenSelectViewDialog();

        object GetDataOfActiveView(); 
    }
}