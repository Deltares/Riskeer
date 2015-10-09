using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator.Binding;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;
using DelftTools.Controls.Swf.Properties;

namespace DelftTools.Controls.Swf.DataEditorGenerator
{
    public static class DataEditorGeneratorSwf
    {
        public const int LabelWidth = 150;
        public const int EditorWidth = FieldEditControlWidth;
        public const int DefaultHeight = 26;

        private const int FieldEditControlWidth = 125;
        private const int FieldEditControlWideWidth = 150;

        private static readonly Bitmap InformationIcon = Resources.information;

        /// <summary>
        /// Generates a user control (DataEditor) based on an abstract object (UI-meta) description. 
        /// To use the generated control, add it to a view and set its 'Data' property.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static DataEditor GenerateView(ObjectUIDescription typeInfo)
        {
            var categories = typeInfo.FieldDescriptions.GroupBy(p => p.Category).ToList();

            var dataEditor = new DataEditor
            {
                AutoSize = true
            };

            if (categories.Count == 1)
            {
                dataEditor.Controls.Add(GenerateCategoryView(categories.First(), dataEditor.Bindings));
            }
            else
            {
                var tabControl = new TabControl
                {
                    Dock = DockStyle.Fill
                };
                dataEditor.Controls.Add(tabControl);
                foreach (var category in categories)
                {
                    var page = new TabPage
                    {
                        Text = category.Key ?? "Misc.",
                        Padding = new Padding(10),
                    };
                    var c = GenerateCategoryView(category, dataEditor.Bindings);
                    c.Dock = DockStyle.Fill;
                    page.Controls.Add(c);
                    tabControl.TabPages.Add(page);
                }
            }

            return dataEditor;
        }

        private static Control GenerateCategoryView(IEnumerable<FieldUIDescription> fieldsInCategory, ICollection<IBinding> bindings)
        {
            var panel = CreateFlowLayoutPanel();

            var subCategories = fieldsInCategory.GroupBy(p => p.SubCategory).ToList();
            foreach (var subCategoryFields in subCategories)
            {
                var inGroupBox = !string.IsNullOrEmpty(subCategoryFields.Key);

                if (inGroupBox)
                {
                    var groupBox = CreateGroupBox(subCategoryFields.Key);
                    var groupBoxPanel = CreateFlowLayoutPanel(true);
                    groupBoxPanel.AutoSize = true;
                    groupBox.SetChildContainer(groupBoxPanel);
                    groupBox.Controls.Add(groupBoxPanel);
                    panel.Controls.Add(groupBox);

                    GenerateFieldControls(subCategoryFields, bindings, groupBoxPanel);

                    foreach (Control child in groupBoxPanel.Controls)
                    {
                        var collapsingPanel = child as SelfCollapsingPanel;
                        if (collapsingPanel != null)
                        {
                            groupBox.SubscribeChild(collapsingPanel);
                        }
                    }
                }
                else
                {
                    GenerateFieldControls(subCategoryFields, bindings, panel);
                }
            }

            return panel;
        }

        private static SelfCollapsingGroupbox CreateGroupBox(string title)
        {
            return new SelfCollapsingGroupbox
            {
                Text = title,
                AutoSize = true,
                Padding = new Padding(8),
            };
        }

        private static FlowLayoutPanel CreateFlowLayoutPanel(bool inGroupBox = false)
        {
            return new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = true,
                AutoScroll = !inGroupBox,
                Dock = DockStyle.Fill,
            };
        }

        private static void GenerateFieldControls(IEnumerable<FieldUIDescription> fieldsInCategory, ICollection<IBinding> bindings, Control container)
        {
            var errorProvider = new ErrorProvider
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            foreach (var fieldDesc in fieldsInCategory)
            {
                var propertyControl = GenerateFieldCompositeControl(fieldDesc, bindings, errorProvider);
                container.Controls.Add(propertyControl);
            }
        }

        /// <summary>
        /// Generates a panel with the edit control (eg, a textbox), but als the caption label and 
        /// optionally a unit label for example.
        /// Also takes care of databinding.
        /// </summary>
        /// <param name="fieldDesc"></param>
        /// <param name="bindings"></param>
        /// <param name="errorProvider"></param>
        /// <returns></returns>
        private static Control GenerateFieldCompositeControl(FieldUIDescription fieldDesc, ICollection<IBinding> bindings, ErrorProvider errorProvider)
        {
            if (fieldDesc.CustomControlHelper != null && fieldDesc.CustomControlHelper.HideCaptionAndUnitLabel())
            {
                var customControl = fieldDesc.CustomControlHelper.CreateControl();
                bindings.Add(DataBinder.Bind(fieldDesc, customControl, customControl, errorProvider));
                return customControl;
            }

            var editControl = GenerateFieldEditControl(fieldDesc);

            var captionLabel = new Label
            {
                Text = fieldDesc.Label,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = LabelWidth,
                Dock = DockStyle.Left
            };

            var panel = new SelfCollapsingPanel
            {
                Height = DefaultHeight
            };
            panel.Controls.Add(captionLabel);

            bindings.Add(DataBinder.Bind(fieldDesc, editControl, panel, errorProvider));

            var separator1 = new Panel
            {
                Width = 2, Dock = DockStyle.Left
            };
            panel.Controls.Add(separator1);

            editControl.Dock = DockStyle.Left;
            editControl.Enabled = !fieldDesc.IsReadOnly;
            if (editControl is DataGridView)
            {
                editControl.Width = FieldEditControlWidth;
                editControl.Height = editControl.Height;
                editControl.SizeChanged += (s, e) => panel.Height = editControl.Height;
            }

            panel.Controls.Add(editControl);

            var separator2 = new Panel
            {
                Width = 4, Dock = DockStyle.Left
            };
            panel.Controls.Add(separator2);

            var tooltipImage = new PictureBox
            {
                Width = 16,
                Height = 16,
                Dock = DockStyle.Left,
                Image = InformationIcon,
                Visible = false
            };
            panel.Controls.Add(tooltipImage);

            var unitLabel = new Label
            {
                Text = fieldDesc.UnitSymbol,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 25,
                Dock = DockStyle.Left
            };
            panel.Controls.Add(unitLabel);

            // add tooltip appearing / disappearing:
            if (!string.IsNullOrEmpty(fieldDesc.ToolTip))
            {
                Action<object> showToolTip = s => LinkedToolTip.SetToolTip((Control) s, fieldDesc.ToolTip, fieldDesc.DocUrl);
                captionLabel.MouseClick += (s, e) => showToolTip(s);
                captionLabel.MouseHover += (s, e) => showToolTip(s);
                tooltipImage.MouseClick += (s, e) => showToolTip(s);
                tooltipImage.MouseHover += (s, e) => showToolTip(s);
                editControl.GotFocus += (s, e) =>
                {
                    unitLabel.Visible = false;
                    tooltipImage.Visible = true;
                };
                editControl.LostFocus += (s, e) =>
                {
                    tooltipImage.Visible = false;
                    unitLabel.Visible = true;
                };
            }

            // invert order of controls
            foreach (Control c in panel.Controls)
            {
                c.BringToFront();
            }

            // calculate panel width
            panel.Width = panel.Controls.OfType<Control>().Sum(c => c.Width) - tooltipImage.Width;

            panel.VisibleChanged += (s, e) =>
            {
                if (!panel.Visible)
                {
                    LinkedToolTip.HideToolTip();
                }
            };

            return panel;
        }

        /// <summary>
        /// Generates the edit control (eg, textbox, combobox etc), excluding any labels etc
        /// </summary>
        /// <param name="fieldDesc"></param>
        /// <returns></returns>
        private static Control GenerateFieldEditControl(FieldUIDescription fieldDesc)
        {
            if (fieldDesc.CustomControlHelper != null)
            {
                return fieldDesc.CustomControlHelper.CreateControl();
            }
            if (fieldDesc.ValueType.IsEnum)
            {
                return new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList, Width = FieldEditControlWidth
                };
            }
            if (fieldDesc.ValueType == typeof(DateTime))
            {
                var dtfInfo = CultureInfo.CurrentCulture.DateTimeFormat;
                return new DateTimePicker
                {
                    Width = FieldEditControlWideWidth,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat =
                        dtfInfo.ShortDatePattern + " " +
                        dtfInfo.ShortTimePattern
                };
            }
            if (fieldDesc.ValueType == typeof(TimeSpan))
            {
                return new TimeSpanEditor
                {
                    Width = FieldEditControlWidth
                };
            }
            if (fieldDesc.ValueType == typeof(bool))
            {
                return new CheckBox
                {
                    Width = FieldEditControlWidth
                };
            }
            if (fieldDesc.ValueType == typeof(string) ||
                fieldDesc.ValueType.IsValueType)
            {
                return new TextBox
                {
                    Width = FieldEditControlWidth
                };
            }
            if (typeof(IList).IsAssignableFrom(fieldDesc.ValueType) ||
                fieldDesc.ValueType.GetInterfaces().Concat(new[]
                {
                    fieldDesc.ValueType
                })
                         .Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                var dataGridView = new DataGridView
                {
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    Enabled = true,
                    AllowUserToResizeRows = false,
                    AllowUserToResizeColumns = false,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders,
                    ScrollBars = ScrollBars.None
                };
                dataGridView.DataBindingComplete += (s, e) =>
                {
                    dataGridView.Height = (dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0) +
                                          dataGridView.Rows.OfType<DataGridViewRow>().Sum(r => r.Height) + 3;
                };
                dataGridView.EnabledChanged += (s, e) =>
                {
                    dataGridView.ForeColor = dataGridView.Enabled ? SystemColors.ControlText : SystemColors.GrayText;
                    dataGridView.DefaultCellStyle.SelectionBackColor = dataGridView.Enabled ? SystemColors.Highlight : SystemColors.ControlLightLight;
                    dataGridView.DefaultCellStyle.SelectionForeColor = dataGridView.Enabled ? SystemColors.HighlightText : SystemColors.GrayText;
                };
                return dataGridView;
            }
            throw new NotImplementedException(string.Format("No control for type {0}", fieldDesc.ValueType));
        }
    }
}