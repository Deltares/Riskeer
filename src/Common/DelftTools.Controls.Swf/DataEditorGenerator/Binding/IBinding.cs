using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding
{
    public interface IBinding
    {
        FieldUIDescription FieldDescription { get; }

        Control EditControl { get; }

        object Data { get; set; }
        void InitializeControl(FieldUIDescription fieldDescription, Control editControl, Control parentControl);

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);

        void Validate(object value);

        /// <summary>
        /// Call to refresh the Enabled state of bound control.
        /// </summary>
        void RefreshEnabled();
    }
}