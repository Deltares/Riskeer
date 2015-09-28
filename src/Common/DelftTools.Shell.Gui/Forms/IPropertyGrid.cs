using DelftTools.Controls;

namespace DelftTools.Shell.Gui.Forms
{
    public interface IPropertyGrid : IView
    {
        /// <summary>
        /// Object which is shown in the property grid, uses ComponentModel to describe properties. 
        /// <seealso cref="System.ComponentModel.DescriptionAttribute"/>
        /// <seealso cref="System.ComponentModel.CategoryAttribute"/>
        /// </summary>
        // -- object SelectedObject { get; set; }
        
        // support for multiple objects
        // to be removed
        // -- object []SelectedObjects { get; set; }
        // to be removed
        bool Enabled { get; set; }

        // Useful for non-evented objects
        void Refresh();

        object GetObjectProperties(object sourceData);
    }
}