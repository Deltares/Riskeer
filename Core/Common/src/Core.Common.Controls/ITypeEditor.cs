using System.ComponentModel;

namespace Core.Common.Controls
{
    /// <summary>
    /// Type editors are controls / components used to edit complex objects of a specific type.
    /// For example cell editors in the TableView, PropertyGrid, etc.
    /// </summary>

    // TODO: move to Editors, rename to IObjectEditor, or maybe merge with IView
    public interface ITypeEditor : IComponent
    {
        event CancelEventHandler Validating;
        object EditableValue { get; set; }
        bool Validate();
        bool CanAcceptEditValue();
        bool CanPopup();
    }
}