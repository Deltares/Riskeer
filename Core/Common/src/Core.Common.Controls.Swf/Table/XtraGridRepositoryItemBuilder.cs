using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using log4net;

namespace Core.Common.Controls.Swf.Table
{
    public static class XtraGridRepositoryItemBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TableViewColumn));

        public static RepositoryItem CreateFromTypeEditor(ITypeEditor typeEditor, GridControl gridControl, GridColumn column, string caption)
        {
            if (typeEditor is MultiLineTextEdior)
            {
                // do not use wordwrap
                column.AppearanceCell.Options.UseTextOptions = true;
                column.AppearanceCell.TextOptions.WordWrap = WordWrap.NoWrap;
                column.AppearanceCell.TextOptions.Trimming = Trimming.EllipsisWord;

                return new RepositoryItemMemoEdit
                {
                    WordWrap = false
                };
            }

            var comboBoxTypeEditor = typeEditor as ComboBoxTypeEditor;
            if (comboBoxTypeEditor != null)
            {
                return CreateComboBoxRepositoryItem(comboBoxTypeEditor, gridControl, caption);
            }

            var buttonEditor = typeEditor as ButtonTypeEditor;
            if (buttonEditor != null)
            {
                var buttonTypeEditorRepositoryItem = new RepositoryItemButtonEdit
                {
                    TextEditStyle = TextEditStyles.HideTextEditor
                };
                var editorButton = buttonTypeEditorRepositoryItem.Buttons[0];

                editorButton.ToolTip = buttonEditor.Tooltip;

                if (buttonEditor.Caption != null || buttonEditor.Image != null)
                {
                    editorButton.Kind = ButtonPredefines.Glyph;
                    editorButton.Caption = buttonEditor.Caption;
                    editorButton.Image = buttonEditor.Image;
                }

                buttonTypeEditorRepositoryItem.ButtonClick += (s, e) => ButtonTypeEditorRepositoryItemButtonClick(e, typeEditor);
                return buttonTypeEditorRepositoryItem;
            }

            // the mechanisms for validating do not work as expected; perhaps some of the encapsulating layers
            // consumes events?
            var repositoryItem = new RepositoryItemPopupContainerEdit
            {
                CloseOnOuterMouseClick = false
            };

            var popupControl = new PopupContainerControl
            {
                AutoSize = true
            };
            var editorControl = (Control) typeEditor;
            popupControl.Controls.Add(editorControl);

            repositoryItem.PopupControl = popupControl;

            repositoryItem.QueryPopUp += RepositoryItemQueryPopUp;
            repositoryItem.QueryResultValue += RepositoryItemQueryResultValue;
            repositoryItem.Validating += RepositoryItemValidating;

            repositoryItem.CloseUp += (sender, e) => RepositoryItemCloseUp(e, typeEditor);
            //close and open with enter 
            repositoryItem.CloseUpKey = new KeyShortcut(Keys.Enter);

            return repositoryItem;
        }

        /// <summary>
        /// Creates a ComboBox editor as <see cref="RepositoryItem"/>. The requested width for rendering
        /// is determined by the rendersize of the largest string in the selection set or the caption.
        /// </summary>
        /// <remarks>It has been assumed the final caption is already set before calling this function.</remarks>
        private static RepositoryItem CreateComboBoxRepositoryItem(ComboBoxTypeEditor comboBoxEditor, GridControl dxGridControl, string caption)
        {
            RepositoryItem comboBoxRepositoryItem;

            // TODO: make it check if Items is evented, if yes - refresh
            var items = comboBoxEditor.Items.Cast<object>().Select(i => new TableViewComboBoxItem
            {
                Value = i
            }).ToList();
            items.ForEach(i => i.CustomFormatter = comboBoxEditor.CustomFormatter);

            if (!comboBoxEditor.ItemsMandatory)
            {
                var repositoryItemComboBox = new RepositoryItemComboBox();

                repositoryItemComboBox.Items.AddRange(items);
                repositoryItemComboBox.CloseUp += RepositoryItemComboBoxCloseUp;
                repositoryItemComboBox.TextEditStyle = TextEditStyles.DisableTextEditor; // disable editing
                repositoryItemComboBox.DrawItem += OnDrawItem;
                comboBoxRepositoryItem = repositoryItemComboBox;
            }
            else
            {
                var repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
                {
                    SearchMode = SearchMode.AutoFilter
                };

                repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("DisplayText"));
                repositoryItemLookUpEdit.DataSource = items;
                repositoryItemLookUpEdit.DisplayMember = "Value";
                repositoryItemLookUpEdit.ValueMember = "Value";
                repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.DisableTextEditor;
                repositoryItemLookUpEdit.ShowHeader = false;
                repositoryItemLookUpEdit.ShowFooter = false;
                repositoryItemLookUpEdit.AllowNullInput = DefaultBoolean.False;
                repositoryItemLookUpEdit.NullText = "";
                comboBoxRepositoryItem = repositoryItemLookUpEdit;
            }

            // Needed for formatting the value in the textbox during editing -> this is done separately from the items
            if (comboBoxEditor.CustomFormatter != null)
            {
                comboBoxRepositoryItem.DisplayFormat.FormatType = FormatType.Custom;
                comboBoxRepositoryItem.DisplayFormat.Format = new TableViewCellFormatterProvider(comboBoxEditor.CustomFormatter);
            }

            comboBoxRepositoryItem.EditValueChanged += (sender, args) => dxGridControl.FocusedView.CloseEditor();
            comboBoxRepositoryItem.BestFitWidth = GetBestFitEditorWidth(items, comboBoxRepositoryItem.Appearance.Font, caption);

            return comboBoxRepositoryItem;
        }

        private static void RepositoryItemComboBoxCloseUp(object sender, CloseUpEventArgs e)
        {
            var comboBoxItem = e.Value as TableViewComboBoxItem;
            if (comboBoxItem != null)
            {
                e.Value = comboBoxItem.Value;
            }
        }

        private static int GetBestFitEditorWidth(IEnumerable<TableViewComboBoxItem> items, Font font, string caption)
        {
            int bestFitWidth = -1; // Default, auto-size
            //Determine longest string in combobox and estimate it's rendering size
            foreach (var item in items)
            {
                Size textSize = TextRenderer.MeasureText(item.ToString(), font);
                if (textSize.Width > bestFitWidth && textSize.Width != 0)
                {
                    bestFitWidth = textSize.Width;
                }
            }

            // Use caption render width if it's the largest
            Size captionRenderSize = TextRenderer.MeasureText(caption, font);
            if (captionRenderSize.Width > bestFitWidth && captionRenderSize.Width != 0)
            {
                bestFitWidth = captionRenderSize.Width;
            }
            // If we have determined a BestFitWidth, add margin for dorpdown menu button
            if (bestFitWidth != -1)
            {
                bestFitWidth += 30;
            }
            return bestFitWidth;
        }

        // Work around for DevExpress bug : Formatting RepositoryItemComboBox items text
        // http://www.devexpress.com/Support/Center/p/DQ23557.aspx
        private static void OnDrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            var comboboxitem = e.Item as TableViewComboBoxItem;
            if (comboboxitem == null)
            {
                throw new NotSupportedException("Unknown item type");
            }

            e.Cache.FillRectangle(new SolidBrush(e.Appearance.BackColor), e.Bounds);
            e.Cache.DrawString(comboboxitem.DisplayText, e.Appearance.Font, new SolidBrush(e.Appearance.ForeColor), e.Bounds, e.Appearance.GetStringFormat());
            e.Handled = true;
        }

        /// <summary>
        /// Called when the popup is closed
        /// </summary>
        private static void RepositoryItemCloseUp(CloseUpEventArgs e, ITypeEditor editor)
        {
            if (e.CloseMode == PopupCloseMode.Cancel)
            {
                return;
            }
            if (!editor.CanAcceptEditValue())
            {
                e.AcceptValue = false;
            }
        }

        private static void RepositoryItemValidating(object sender, CancelEventArgs e)
        {
            var popupContainer = (PopupContainerEdit) sender;
            var typeEditor = (ITypeEditor) popupContainer.Properties.PopupControl.Controls[0];
            if (!typeEditor.Validate())
            {
                e.Cancel = true;
            }
        }

        private static void ButtonTypeEditorRepositoryItemButtonClick(ButtonPressedEventArgs e, ITypeEditor editor)
        {
            var buttonTypeEditor = (ButtonTypeEditor) editor;
            if (buttonTypeEditor.ButtonClickAction != null)
            {
                buttonTypeEditor.ButtonClickAction();
            }
        }

        private static void RepositoryItemQueryResultValue(object sender, QueryResultValueEventArgs e)
        {
            var popupContainer = (PopupContainerEdit) sender;
            var typeEditor = (ITypeEditor) popupContainer.Properties.PopupControl.Controls[0];
            e.Value = typeEditor.EditableValue;
        }

        private static void RepositoryItemQueryPopUp(object sender, CancelEventArgs e)
        {
            var popupContainer = (PopupContainerEdit) sender;

            var typeEditor = (ITypeEditor) popupContainer.Properties.PopupControl.Controls[0];
            if (!typeEditor.CanPopup())
            {
                Log.Warn("repositoryItem_QueryPopUp ! CanPopup");
                e.Cancel = true;
                return;
            }
            var editValue = popupContainer.EditValue;
            typeEditor.EditableValue = editValue;
        }
    }
}