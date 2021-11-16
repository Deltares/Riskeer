// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Core.Common.Util.Drawing;
using Core.Gui.Clipboard;
using Core.Gui.Properties;
using log4net.Core;

namespace Core.Gui.Forms.Log
{
    /// <summary>
    /// Class that receives messages from <see cref="MessageWindowLogAppender"/> to be displayed
    /// in a thread-safe way. This view supports filtering particular logging levels.
    /// </summary>
    public partial class MessageWindow : UserControl, IMessageWindow
    {
        private const string errorLevelUnicode = "\uE90B";
        private const string warningLevelUnicode = "\uE90A";
        private const string informationLevelUnicode = "\uE909";
        private const string debugLevelUnicode = "\uE90C";
        
        private static readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        private static readonly Font font = FontHelper.CreateFont(Resources.Symbols, privateFontCollection);
        
        private readonly IWin32Window dialogParent;
        private readonly Dictionary<string, string> levelImageName;
        private readonly ConcurrentQueue<MessageData> newMessages = new ConcurrentQueue<MessageData>();
        
        private bool filtering;

        /// <summary>
        /// Creates a new instance of <see cref="MessageWindow" />.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which dialogs should be shown on top.</param>
        public MessageWindow(IWin32Window dialogParent)
        {
            this.dialogParent = dialogParent;

            MessageWindowLogAppender.Instance.MessageWindow = this;
            InitializeComponent();

            // order is the same as in log4j Level (check sources of log4net)
            levelImageName = new Dictionary<string, string>
            {
                [Level.Off.ToString()] = errorLevelUnicode,
                [Level.Emergency.ToString()] = errorLevelUnicode,
                [Level.Fatal.ToString()] = errorLevelUnicode,
                [Level.Alert.ToString()] = errorLevelUnicode,
                [Level.Critical.ToString()] = errorLevelUnicode,
                [Level.Severe.ToString()] = errorLevelUnicode,
                [Level.Error.ToString()] = errorLevelUnicode,
                [Level.Warn.ToString()] = warningLevelUnicode,
                [Level.Notice.ToString()] = warningLevelUnicode,
                [Level.Info.ToString()] = informationLevelUnicode,
                [Level.Debug.ToString()] = debugLevelUnicode,
                [Level.Fine.ToString()] = debugLevelUnicode,
                [Level.Trace.ToString()] = debugLevelUnicode,
                [Level.Finer.ToString()] = debugLevelUnicode,
                [Level.Verbose.ToString()] = debugLevelUnicode,
                [Level.Finest.ToString()] = debugLevelUnicode,
                [Level.All.ToString()] = debugLevelUnicode
            };

            SetFonts();

            messagesDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            messagesDataGridView.MouseUp += MessagesDataGridViewMouseUp;

            ApplyFilter();
            messagesDataGridView.CellFormatting += MessagesDataGridViewCellFormatting;

            // fixes DPI problem
            messagesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messagesDataGridView.RowsAdded += MessagesDataGridViewRowsAdded;
        }

        #region IView Members

        public object Data
        {
            get => Messages;
            set {}
        }

        #endregion

        #region IMessageWindow Members

        public void AddMessage(Level level, DateTime time, string message)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            string shortMessage;
            using (var reader = new StringReader(message))
            {
                shortMessage = reader.ReadLine();
            }

            newMessages.Enqueue(new MessageData
            {
                ImageName = level.ToString(),
                Time = time,
                ShortMessage = shortMessage,
                FullMessage = message
            });
            Invalidate();
        }

        #endregion

        private void SetFonts()
        {
            buttonShowDetails.Font = font;
            buttonShowInfo.Font = font;
            buttonShowWarning.Font = font;
            buttonShowError.Font = font;
            levelColumnDataGridViewTextBoxColumn.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = font
            };
        }

        private void PopulateMessages()
        {
            if (newMessages.IsEmpty)
            {
                return;
            }

            Messages.BeginLoadData();

            try
            {
                while (newMessages.TryDequeue(out MessageData msg))
                {
                    DataRow row = Messages.NewRow();

                    row[0] = msg.ImageName;
                    row[1] = msg.Time;
                    row[2] = msg.ShortMessage;
                    row[3] = msg.FullMessage;

                    Messages.Rows.InsertAt(row, 0);
                }
            }
            finally
            {
                Messages.EndLoadData();
            }

            if (messagesDataGridView.Rows.Count > 0)
            {
                messagesDataGridView.CurrentCell = messagesDataGridView.Rows[0].Cells[0];
            }
        }

        private void AutoSizeRow(DataGridViewRow row)
        {
            int prefHeight = row.GetPreferredHeight(row.Index, DataGridViewAutoSizeRowMode.AllCells, false);

            if (prefHeight > 100)
            {
                row.Height = 100;
                return;
            }

            messagesDataGridView.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCells);
        }

        private void ValidateContextMenuCommands()
        {
            foreach (ToolStripItem item in contextMenu.Items)
            {
                item.Enabled = messagesDataGridView.Rows.Count > 0;
            }
        }

        private void ApplyFilter()
        {
            filtering = true;

            messagesBindingSource.Filter = CreateLoggingLevelDataGridViewFilter();

            filtering = false;

            foreach (DataGridViewRow row in messagesDataGridView.Rows)
            {
                AutoSizeRow(row);
            }
        }

        private string CreateLoggingLevelDataGridViewFilter()
        {
            var filterLines = new List<string>();
            string filterFormat = string.Format(CultureInfo.CurrentCulture,
                                                "{0} = '{{0}}'",
                                                levelColumn.ColumnName);
            if (buttonShowInfo.Checked)
            {
                filterLines.Add(string.Format(CultureInfo.CurrentCulture,
                                              filterFormat,
                                              Level.Info));
            }

            if (buttonShowWarning.Checked)
            {
                filterLines.Add(string.Format(CultureInfo.CurrentCulture,
                                              filterFormat,
                                              Level.Warn));
            }

            if (buttonShowError.Checked)
            {
                filterLines.Add(string.Format(CultureInfo.CurrentCulture,
                                              filterFormat,
                                              Level.Error));
                filterLines.Add(string.Format(CultureInfo.CurrentCulture,
                                              filterFormat,
                                              Level.Fatal));
            }

            return filterLines.Count == 0
                       ? string.Format(CultureInfo.CurrentCulture,
                                       filterFormat,
                                       "NOTHING SHOWN")
                       : string.Join(" OR ", filterLines);
        }

        private void ShowMessageWindowDialog()
        {
            if (messagesDataGridView.CurrentRow == null)
            {
                return;
            }

            var messageWindowDialog = new MessageWindowDialog(dialogParent, (string) messagesDataGridView.CurrentRow.Cells[fullMessageColumnDataGridViewTextBoxColumn.Index].Value);

            messageWindowDialog.ShowDialog();
        }

        /// <summary>
        /// Class that holds message information.
        /// </summary>
        private class MessageData
        {
            /// <summary>
            /// Gets or sets the image representation of the logging level.
            /// </summary>
            public string ImageName { get; set; }

            /// <summary>
            /// Gets or sets the time when the message was logged.
            /// </summary>
            public DateTime Time { get; set; }

            /// <summary>
            /// Gets or sets the short message text, i.e., the first line of  <see cref="FullMessage"/>.
            /// </summary>
            public string ShortMessage { get; set; }

            /// <summary>
            /// Gets or sets the full message text.
            /// </summary>
            public string FullMessage { get; set; }
        }

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PopulateMessages();
        }

        #region Messages data grid view

        private void MessagesDataGridViewCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.RowIndex > -1)
            {
                ShowMessageWindowDialog();
            }
        }

        private void MessagesDataGridViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowMessageWindowDialog();

                e.Handled = true;
            }
        }

        private void MessagesDataGridViewMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ValidateContextMenuCommands();
            }
        }

        private void MessagesDataGridViewRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (filtering)
            {
                return;
            }

            DataGridViewRow row = messagesDataGridView.Rows[e.RowIndex];
            AutoSizeRow(row);
        }

        private void MessagesDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != levelColumnDataGridViewTextBoxColumn.Index || e.Value == null)
            {
                return;
            }

            // Dataset stores image-name instead of actual image, therefore we map to 
            // actual image during formatting.
            var level = (string) e.Value;
            e.Value = levelImageName[level];
        }

        #endregion

        #region Button

        private void ButtonClearAllClick(object sender, EventArgs e)
        {
            Messages.Clear();
        }

        private void ButtonCopyClick(object sender, EventArgs e)
        {
            ClipboardProvider.Clipboard.SetDataObject(messagesDataGridView.GetClipboardContent());
        }

        private void ButtonShowDetailsClick(object sender, EventArgs e)
        {
            ShowMessageWindowDialog();
        }

        private void ButtonShowInfoClick(object sender, EventArgs e)
        {
            buttonShowInfo.Checked = !buttonShowInfo.Checked;
            ApplyFilter();
        }

        private void ButtonShowWarningClick(object sender, EventArgs e)
        {
            buttonShowWarning.Checked = !buttonShowWarning.Checked;
            ApplyFilter();
        }

        private void ButtonShowErrorClick(object sender, EventArgs e)
        {
            buttonShowError.Checked = !buttonShowError.Checked;
            ApplyFilter();
        }

        #endregion

        #endregion
    }
}