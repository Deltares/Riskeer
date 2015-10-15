using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding
{
    internal abstract class Binding<TControl> : IBinding where TControl : Control
    {
        private static readonly ErrorProvider errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        protected FieldUIDescription FieldDescription;
        protected TControl Control;
        protected Control ParentControl;
        private object data;

        Control IBinding.EditControl
        {
            get
            {
                return Control;
            }
        }

        FieldUIDescription IBinding.FieldDescription
        {
            get
            {
                return FieldDescription;
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null)
                {
                    Deinitialize();
                }

                data = value;

                if (data != null)
                {
                    Initialize();
                    ParentControl.Visible = FieldDescription.IsVisible(data);
                    ParentControl.Enabled = FieldDescription.IsEnabled(data);
                }
            }
        }

        public void InitializeControl(FieldUIDescription fieldDescription, Control editControl, Control parentControl)
        {
            FieldDescription = fieldDescription;
            Control = (TControl) editControl;
            ParentControl = parentControl;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control.InvokeRequired)
            {
                return; //not important enough to Invoke for
            }

            //maybe lazy only? refresh validation status if we currently have an error
            // if (!string.IsNullOrEmpty(errorProvider.GetError(Control)))

            Validate(GetControlValue());

            //general filtering
            if (FieldDescription.AlwaysRefresh || FieldDescription.HasDependencyFunctions || FieldDescription.Name == e.PropertyName)
            {
                RefreshEnabled();
                DataSourceChanged();
            }
        }

        public void RefreshEnabled()
        {
            if (Control.InvokeRequired)
            {
                return; //not important enough to Invoke for
            }

            ParentControl.Visible = FieldDescription.IsVisible(data);
            ParentControl.Enabled = FieldDescription.IsEnabled(Data);
        }

        public void Validate(object value)
        {
            errorProvider.SetIconAlignment(Control, ErrorIconAlignment.MiddleLeft);
            string errorMessage;
            errorProvider.SetError(Control, !FieldDescription.Validate(Data, value, out errorMessage) ? errorMessage : string.Empty);
        }

        protected object DataValue
        {
            get
            {
                return FieldDescription.GetValue(Data);
            }
            set
            {
                CommitValue(value);
            }
        }

        protected void CommitValue(object value)
        {
            Validate(value);
            // error or not, continue and still set it in the datasource..

            //set in datasource if changed
            var currentValue = FieldDescription.GetValue(Data);
            if (!Equals(currentValue, value))
            {
                FieldDescription.SetValue(Data, value);
            }
        }

        protected abstract object GetControlValue();

        protected abstract void Initialize();

        protected abstract void Deinitialize();

        protected abstract void DataSourceChanged();
    }
}