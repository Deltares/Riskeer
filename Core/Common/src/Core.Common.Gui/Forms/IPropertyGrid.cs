using Core.Common.Controls;

namespace Core.Common.Gui.Forms
{
    public interface IPropertyGrid : IView
    {
        object GetObjectProperties(object sourceData);
    }
}