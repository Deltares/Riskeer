using DelftTools.Controls;

namespace DelftTools.Shell.Gui.Forms
{
    public interface IPropertyGrid : IView
    {
        object GetObjectProperties(object sourceData);
    }
}