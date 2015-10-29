using System;
using System.Collections;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf.Editors
{
    public partial class ComboBoxTypeEditor : UserControl, ITypeEditor
    {
        public ComboBoxTypeEditor()
        {
            InitializeComponent();

            ItemsMandatory = true;
        }

        /// <summary>
        /// Determines if the <see cref="Items"/> are mandatory
        /// (if true -> values that are not in the <see cref="Items"/> list will be shown as blank)
        /// </summary>
        public bool ItemsMandatory { get; set; }

        /// <summary>
        /// List of values that the user can choose from
        /// </summary>
        public IEnumerable Items { get; set; }

        ///<summary>
        /// Sets the displayformat of the column. For example c2, D or AA{0}
        ///</summary>
        public string DisplayFormat { get; set; }

        /// <summary>
        /// Allows to override the way cell text is rendered.
        /// </summary>
        public ICustomFormatter CustomFormatter { get; set; }

        public object EditableValue { get; set; }

        public bool CanAcceptEditValue()
        {
            return true;
        }

        public bool CanPopup()
        {
            return true;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Items = null;
                EditableValue = null;

                components.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}