using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors.Mask;

namespace DelftTools.Controls.Swf.Editors
{
    /// <summary>
    /// Implementation of a graphical editor that will show an appropriate windows forms control based on the value type.
    /// It supports int, float, double, string, bool, DateTime and TimeSpan values; all nullable.
    /// </summary>
    public partial class ObjectEditor : UserControl
    {
        private object value;
        private Type valueType;
        private string label;
        private readonly Dictionary<Type, Control> support;
        public event EventHandler ValueChanged;

        /// <summary>
        /// Creates the graphical object editor control.
        /// </summary>
        public ObjectEditor()
        {
            InitializeComponent();

            txtValue.Properties.Mask.UseMaskAsDisplayFormat = true;

            support = new Dictionary<Type, Control>();
            support[typeof(int)] = txtValue;
            support[typeof(double)] = txtValue;
            support[typeof(float)] = txtValue;
            support[typeof(string)] = txtValue;
            support[typeof(bool)] = chkValue;
            support[typeof(DateTime)] = dtpValue;
            support[typeof(TimeSpan)] = txtValue;

            // Default to int (the setter will initialze the controls as well)
            ValueType = typeof (int);

            // Events should be bubbled
            chkValue.CheckedChanged += control_ValueChanged;
            txtValue.TextChanged += control_ValueChanged;
            dtpValue.ValueChanged += control_ValueChanged;

        }

        #region Implementation of IObjectEditor

        public object Value
        {
            get { return value; }
            set
            {
                if (value != null && value.GetType() != valueType)
                {
                    throw new ArgumentException("The set value '" + value + "' is not of the type " + valueType + ". (You might want to set ValueType first.)");
                }
                this.value = value;

                // Set the value in the active control
                if (this.value != null)
                {
                    if (GetControlInstance() is DevExpress.XtraEditors.TextEdit)
                    {
                        GetControlInstance().Text = this.value.ToString();
                    }
                    if (GetControlInstance() is CheckBox)
                    {
                        ((CheckBox) GetControlInstance()).Checked = (bool) this.value;
                    }
                    if (GetControlInstance() is DateTimePicker)
                    {
                        ((DateTimePicker)GetControlInstance()).Value = (DateTime)this.value;
                    }
                }
                else
                {
                    if (GetControlInstance() is DevExpress.XtraEditors.TextEdit)
                    {
                        GetControlInstance().Text = string.Empty;
                    }
                    if (GetControlInstance() is CheckBox)
                    {
                        ((CheckBox)GetControlInstance()).CheckState = CheckState.Indeterminate;
                    }
                    if (GetControlInstance() is DateTimePicker)
                    {
                        // no action (only used in constructor, correct value will be set later)
                    }
                }
            }
        }

        public Type ValueType
        {
            get { return valueType; }
            set
            {
                if (!support.ContainsKey(value))
                {
                    throw new ArgumentException("Type " + value + " is not supported by any available editor.");
                }
                valueType = value;

                // Reset contained value
                if (this.value != null && this.value.GetType() != value)
                {
                    // WARNING: Value lost on setting of the ValueType when a Value of a different Type was already set.
                    this.value = null;
                }

                UpdateEditorVisibility();
            }
        }

        public string Label
        {
            get { return label; }
            set
            {
                label = value;

                // Set the label on the forms control, if applicable
                if (GetControlInstance() is CheckBox)
                {
                    GetControlInstance().Text = label;
                }
            }
        }
        public Control GetControlInstance()
        {
            return support[valueType];
        }

        #endregion

        private void control_ValueChanged(object sender, EventArgs e)
        {
            // Parse input
            if (valueType == typeof(DateTime))
            {
                value = dtpValue.Value;
            }
            else if (valueType == typeof(bool))
            {
                if (chkValue.CheckState == CheckState.Indeterminate)
                {
                    value = null;
                }
                else
                {
                    value = chkValue.Checked;
                }
            }
            else
            {
                // Try to parse the text in the textbox; otherwise set null
                if (txtValue.Text == string.Empty)
                {
                    value = null;
                }
                else
                {
                    if (valueType == typeof (string))
                    {
                        value = txtValue.Text;
                    }
                    else if (valueType == typeof (int))
                    {
                        int v;
                        if (int.TryParse(txtValue.Text, out v))
                        {
                            value = v;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else if (valueType == typeof (float))
                    {
                        float v;
                        if (float.TryParse(txtValue.Text, out v))
                        {
                            value = v;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else if (valueType == typeof (double))
                    {
                        double v;
                        if (double.TryParse(txtValue.Text, out v))
                        {
                            value = v;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else if (valueType == typeof (TimeSpan))
                    {
                        TimeSpan v;
                        if (TimeSpan.TryParse(txtValue.Text, out v))
                        {
                            value = v;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                }
            }

            // Bubble value changed event, but hide the original control as sender
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        private void UpdateEditorVisibility()
        {
            // Enable only the editor that supports the current value type
            GetControlInstance().BringToFront();
            foreach (KeyValuePair<Type, Control> pair in support)
            {
                pair.Value.Visible = false;// (pair.Key == valueType);
            }
            GetControlInstance().Visible = true;
            // Adjust input control to the current ValueType
            if (valueType == typeof(DateTime))
            {
                dtpValue.Enabled = true;
            }
            else if (valueType == typeof(string))
            {
                txtValue.Properties.Mask.MaskType = MaskType.None;
            }
            else if (valueType == typeof(int))
            {
                txtValue.Properties.Mask.MaskType = MaskType.Numeric;
                txtValue.Properties.Mask.EditMask = "n0";
            }
            else if (valueType == typeof(float) || valueType == typeof(double))
            {
                txtValue.Properties.Mask.MaskType = MaskType.Numeric;
                txtValue.Properties.Mask.EditMask = "n";
            }
            else if (valueType == typeof(TimeSpan))
            {
                txtValue.Properties.Mask.MaskType = MaskType.RegEx;
                txtValue.Properties.Mask.EditMask = "\\d?\\d:\\d?\\d:\\d?\\d";
                txtValue.Properties.Mask.AutoComplete = AutoCompleteType.Optimistic;
            }
        }

    }
}