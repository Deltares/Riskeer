using System.ComponentModel;

namespace Core.Common.Controls.Table.Editors
{
    /// <summary>
    /// Type editors are controls / components used to edit complex objects of a specific type.
    /// For example cell editors in the TableView, PropertyGrid, etc.
    /// </summary>
    public interface ITypeEditor : IComponent
    {
        event CancelEventHandler Validating;

        object EditableValue { get; set; }

        bool Validate();

        bool CanAcceptEditValue();

        bool CanPopup();
    }
}