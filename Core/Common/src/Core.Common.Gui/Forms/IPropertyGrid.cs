using Core.Common.Controls;
using Core.Common.Forms.Views;

namespace Core.Common.Gui.Forms
{
    public interface IPropertyGrid : IView
    {
        object GetObjectProperties(object sourceData);
    }
}