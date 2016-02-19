// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    /// <summary>
    /// Class that receives messages from <see cref="MessageWindowLogAppender"/> to be displayed
    /// in a thread-safe way. This view supports filtering particular logging levels.
    /// </summary>
    public partial class MessageWindow : UserControl, IMessageWindow
    {
        #region Constants referring to the item-names of the ImageList

        private const string errorLevelImageName = "exclamation.png";
        private const string warningLevelImageName = "error.png";
        private const string informationLevelImageName = "information.png";
        private const string debugLevelImageName = "debug.png";

        #endregion

        private readonly IWin32Window dialogParent;
        private readonly Dictionary<string, string> levelImageName;
        private readonly ConcurrentQueue<MessageData> newMessages = new ConcurrentQueue<MessageData>();
        private bool filtering;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWindow" /> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which dialogs should be shown on top.</param>
        public MessageWindow(IWin32Window dialogParent)
        {
            this.dialogParent = dialogParent;

            Text = Resources.MessageWindow_MessageWindow_Messages;
            MessageWindowLogAppender.Instance.MessageWindow = this;
            InitializeComponent();

            levelImageName = new Dictionary<string, string>();

            // order is the same as in log4j Level (check sources of log4net)
            levelImageName[Level.Off.ToString()] = errorLevelImageName;
            levelImageName[Level.Emergency.ToString()] = errorLevelImageName;
            levelImageName[Level.Fatal.ToString()] = errorLevelImageName;
            levelImageName[Level.Alert.ToString()] = errorLevelImageName;
            levelImageName[Level.Critical.ToString()] = errorLevelImageName;
            levelImageName[Level.Severe.ToString()] = errorLevelImageName;
            levelImageName[Level.Error.ToString()] = errorLevelImageName;
            levelImageName[Level.Warn.ToString()] = warningLevelImageName;
            levelImageName[Level.Notice.ToString()] = warningLevelImageName;
            levelImageName[Level.Info.ToString()] = informationLevelImageName;
            levelImageName[Level.Debug.ToString()] = debugLevelImageName;
            levelImageName[Level.Fine.ToString()] = debugLevelImageName;
            levelImageName[Level.Trace.ToString()] = debugLevelImageName;
            levelImageName[Level.Finer.ToString()] = debugLevelImageName;
            levelImageName[Level.Verbose.ToString()] = debugLevelImageName;
            levelImageName[Level.Finest.ToString()] = debugLevelImageName;
            levelImageName[Level.All.ToString()] = debugLevelImageName;
            messagesDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            messagesDataGridView.MouseUp += MessagesDataGridViewMouseUp;

            // Apply sorting so the messages added last will be on Top.
            messagesBindingSource.Sort = "Id";
            messagesBindingSource.ApplySort(messagesBindingSource.SortProperty, ListSortDirection.Descending);
            ApplyFilter();
            messagesDataGridView.CellFormatting += MessagesDataGridViewCellFormatting;

            // fixes DPI problem
            messagesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messagesDataGridView.RowsAdded += MessagesDataGridViewRowsAdded;
        }

        #region IView Members

        public object Data
        {
            get
            {
                return messageWindowData;
            }
            set {}
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PopulateMessages();
        }

        private void PopulateMessages()
        {
            if (newMessages.IsEmpty)
            {
                return;
            }

            messageWindowData.Messages.BeginLoadData();
            try
            {
                MessageData msg;
                while (newMessages.TryDequeue(out msg))
                {
                    messageWindowData.Messages.AddMessagesRow(msg.ImageName, msg.Time, msg.Message);
                }
            }
            finally
            {
                messageWindowData.Messages.EndLoadData();
            }

            if (messagesDataGridView.Rows.Count > 0)
            {
                messagesDataGridView.CurrentCell = messagesDataGridView.Rows[0].Cells[0];
            }
        }

        private void MessagesDataGridViewRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (filtering)
            {
                return;
            }
            var row = messagesDataGridView.Rows[e.RowIndex];
            AutoSizeRow(row);
        }

        private void AutoSizeRow(DataGridViewRow row)
        {
            var prefHeight = row.GetPreferredHeight(row.Index, DataGridViewAutoSizeRowMode.AllCells, false);

            if (prefHeight > 100)
            {
                row.Height = 100;
                return;
            }

            messagesDataGridView.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCells);
        }

        private void MessagesDataGridViewMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ValidateContextMenuCommands();
            }
        }

        private void ValidateContextMenuCommands()
        {
            foreach (ToolStripItem item in contextMenu.Items)
            {
                item.Enabled = (messagesDataGridView.Rows.Count > 0);
            }
        }

        private void ButtonClearAllClick(object sender, EventArgs e)
        {
            Clear();
        }

        private void ButtonCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(messagesDataGridView.GetClipboardContent());
        }

        private void MessagesDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.Value == null)
            {
                return;
            }

            // Dataset stores image-name instead of actual image, therefore we map to 
            // actual image during formatting.
            var level = (string) e.Value;
            e.Value = levelImages.Images[levelImageName[level]];
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
            var filterlines = new List<string>();
            if (buttonShowInfo.Checked)
            {
                filterlines.Add(string.Format("Image = '{0}'", Level.Info));
            }
            if (buttonShowWarning.Checked)
            {
                filterlines.Add(string.Format("Image = '{0}'", Level.Warn));
            }
            if (buttonShowError.Checked)
            {
                filterlines.Add(string.Format("Image = '{0}'", Level.Error));
                filterlines.Add(string.Format("Image = '{0}'", Level.Fatal));
            }
            return filterlines.Count == 0 ?
                       "Image = 'NOTHING SHOWN'" :
                       string.Join(" OR ", filterlines);
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

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (messagesDataGridView.CurrentRow == null)
            {
                return;
            }

            var messageWindowDialog = new MessageWindowDialog(dialogParent, (string) messagesDataGridView.CurrentRow.Cells[messageDataGridViewTextBoxColumn.Index].Value);

            messageWindowDialog.ShowDialog();
        }

        private class MessageData
        {
            public string ImageName { get; set; }
            public DateTime Time { get; set; }
            public string Message { get; set; }
        }

        #region IMessageWindow Members

        public void AddMessage(Level level, DateTime time, string message)
        {
            newMessages.Enqueue(new MessageData
            {
                ImageName = level.ToString(), Time = time, Message = message
            });
            Invalidate();
        }

        public void Clear()
        {
            messageWindowData.Clear();
        }

        #endregion
    }
}