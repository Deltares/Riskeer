using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class CustomControlBinding : IBinding
    {
        private static readonly ErrorProvider ErrorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        private readonly ICustomControlHelper customControlHelper;
        private FieldUIDescription fieldDescription;

        public CustomControlBinding(ICustomControlHelper customControlHelper)
        {
            this.customControlHelper = customControlHelper;
        }

        public Control EditControl { get; private set; }

        FieldUIDescription IBinding.FieldDescription
        {
            get
            {
                return fieldDescription;
            }
        }

        public object Data
        {
            get
            {
                return null;
            }
            set
            {
                customControlHelper.SetData(EditControl, value, value != null
                                                                    ? fieldDescription.GetValue(value)
                                                                    : null);
            }
        }

        public void InitializeControl(FieldUIDescription fieldDescription, Control editControl, Control parentControl)
        {
            this.fieldDescription = fieldDescription;
            EditControl = editControl;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // do nothing for now.. may need this later
        }

        public void Validate(object value)
        {
            string errorMessage;
            if (!fieldDescription.Validate(Data, value, out errorMessage))
            {
                ErrorProvider.SetError(EditControl, errorMessage);
            }
            else
            {
                ErrorProvider.SetError(EditControl, string.Empty);
            }
        }

        public void RefreshEnabled()
        {
            // do nothing for now.. may need this later
        }
    }
}