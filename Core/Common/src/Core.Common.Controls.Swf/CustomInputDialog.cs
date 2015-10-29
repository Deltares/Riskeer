using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.DataEditorGenerator;
using Core.Common.Controls.Swf.DataEditorGenerator.Metadata;
using Core.Common.Utils.Reflection;

namespace Core.Common.Controls.Swf
{
    public partial class CustomInputDialog : Form
    {
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();
        private readonly Dictionary<string, FieldUIDescription> datafields = new Dictionary<string, FieldUIDescription>();

        public CustomInputDialog()
        {
            InitializeComponent();
        }

        public object this[string dataName]
        {
            get
            {
                return GetValueCore(dataName);
            }
        }

        public FieldUIDescription AddInput<T>(string dataName)
        {
            var initialValue = default(T);
            return AddInput(dataName, initialValue);
        }

        public FieldUIDescription AddInput<T>(string dataName, T initialValue)
        {
            if (typeof(T) == typeof(string) && initialValue == null)
            {
                data.Add(dataName, "");
            }
            else
            {
                data.Add(dataName, initialValue);
            }

            var fieldDescription = new FieldUIDescription(o => GetValueCore(dataName), (o, v) => SetValue(dataName, v))
            {
                Name = dataName,
                ValueType = typeof(T),
                Label = dataName,
            };

            datafields.Add(dataName, fieldDescription);
            return fieldDescription;
        }

        public FieldUIDescription AddChoice<T>(string dataName, List<T> choices)
        {
            data.Add(dataName, choices.First());

            var choicesAsStrings = choices.Select(c => c.ToString()).ToArray();
            var dynamicEnum = DynamicTypeUtils.CreateDynamicEnum(dataName,
                                                                 Enumerable.Range(0, choices.Count).ToArray(),
                                                                 choicesAsStrings, choicesAsStrings);

            var fieldDescription = new FieldUIDescription(
                o => Enum.GetValues(dynamicEnum).GetValue(choices.IndexOf((T) GetValueCore(dataName))), // get
                (o, v) => SetValue(dataName, choices[(int) v])) // set
            {
                Name = dataName,
                ValueType = dynamicEnum,
                Label = dataName,
            };

            datafields.Add(dataName, fieldDescription);
            return fieldDescription;
        }

        public T GetValue<T>(string dataName)
        {
            return (T) GetValueCore(dataName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // generate custom input fields:
            var objectDescription = new ObjectUIDescription
            {
                FieldDescriptions = datafields.Values
            };
            var dataEditor = DataEditorGeneratorSwf.GenerateView(objectDescription);
            dataEditor.Data = new object();

            var itemsHeight = 0;

            foreach (var control in dataEditor.Controls.OfType<FlowLayoutPanel>())
            {
                control.WrapContents = false;
                control.AutoScroll = false;
                itemsHeight = Math.Max(itemsHeight, control.PreferredSize.Height);
            }

            dataEditor.Location = new Point(0, 0);
            dataEditor.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            dataEditor.Height = itemsHeight;
            dataEditor.Width = Width;
            Controls.Add(dataEditor);

            ClientSize = new Size(ClientSize.Width, itemsHeight + btnPanel.Height);
        }

        private void SetValue(string dataName, object value)
        {
            data[dataName] = value;
        }

        private object GetValueCore(string dataName)
        {
            return data[dataName];
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}