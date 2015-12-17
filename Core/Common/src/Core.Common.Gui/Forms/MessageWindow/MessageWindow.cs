using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Properties;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    public partial class MessageWindow : UserControl, IMessageWindow
    {
        public event EventHandler OnError;
        private readonly IWin32Window owner;
        private readonly Dictionary<string, string> levelImageName;
        private readonly ConcurrentQueue<MessageData> newMessages = new ConcurrentQueue<MessageData>();
        private bool filtering;

        public MessageWindow(IWin32Window owner)
        {
            this.owner = owner;

            Text = Resources.MessageWindow_MessageWindow_Messages;
            MessageWindowLogAppender.MessageWindow = this;
            InitializeComponent();

            levelImageName = new Dictionary<string, string>();

            // order is the same as in log4j Level (check sources of log4net)
            levelImageName[Level.Off.ToString()] = "exclamation.png";
            levelImageName[Level.Emergency.ToString()] = "exclamation.png";
            levelImageName[Level.Fatal.ToString()] = "exclamation.png";
            levelImageName[Level.Alert.ToString()] = "exclamation.png";
            levelImageName[Level.Critical.ToString()] = "exclamation.png";
            levelImageName[Level.Severe.ToString()] = "exclamation.png";
            levelImageName[Level.Error.ToString()] = "exclamation.png";
            levelImageName[Level.Warn.ToString()] = "error.png";
            levelImageName[Level.Notice.ToString()] = "error.png";
            levelImageName[Level.Info.ToString()] = "information.png";
            levelImageName[Level.Debug.ToString()] = "debug.png";
            levelImageName[Level.Fine.ToString()] = "debug.png";
            levelImageName[Level.Trace.ToString()] = "debug.png";
            levelImageName[Level.Finer.ToString()] = "debug.png";
            levelImageName[Level.Verbose.ToString()] = "debug.png";
            levelImageName[Level.Finest.ToString()] = "debug.png";
            levelImageName[Level.All.ToString()] = "debug.png";
            messagesDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            messagesDataGridView.MouseUp += MessagesDataGridViewMouseUp;

            // Apply sorting so the messages added last will be on Top.
            messagesBindingSource.Sort = "Id";
            messagesBindingSource.ApplySort(messagesBindingSource.SortProperty, ListSortDirection.Descending);
            ApplyFilter();
            messagesDataGridView.CellFormatting += MessagesDataGridViewCellFormatting;

            // fixes DPI problem
            messagesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messagesDataGridView.RowsAdded += messagesDataGridView_RowsAdded;
        }

        private void PopulateMessages()
        {
            if (newMessages.IsEmpty)
            {
                return;
            }

            var hasError = false;
            messageWindowData.Messages.BeginLoadData();
            try
            {
                MessageData msg;
                while (newMessages.TryDequeue(out msg))
                {
                    messageWindowData.Messages.AddMessagesRow(msg.ImageName, msg.Time, msg.Message);
                    hasError = hasError || (msg.ImageName == "ERROR" && OnError != null);
                }
            }
            finally
            {
                messageWindowData.Messages.EndLoadData();
            }

            if (hasError)
            {
                OnError(this, null);
            }

            if (messagesDataGridView.Rows.Count > 0)
            {
                messagesDataGridView.CurrentCell = messagesDataGridView.Rows[0].Cells[0];
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PopulateMessages();
        }

        private void messagesDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
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

        /// <summary>
        /// Copies selected range to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(messagesDataGridView.GetClipboardContent());
        }

        /// <summary>
        /// since the dataset stores the image name and not the actual image, we have to put
        /// the corresponding image in the datagridviewcell of the first column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessagesDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.Value == null)
            {
                return;
            }

            var level = (string) e.Value;
            e.Value = levelImages.Images[levelImageName[level]];
        }

        private void ApplyFilter()
        {
            filtering = true;
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

            if (filterlines.Count == 0)
            {
                messagesBindingSource.Filter = "Image = 'NOTHING SHOWN'";
            }
            else
            {
                string filter = filterlines[0];
                for (var i = 1; i < filterlines.Count; i++)
                {
                    filter += " OR " + filterlines[i];
                }
                messagesBindingSource.Filter = filter;
            }

            filtering = false;

            foreach (DataGridViewRow row in messagesDataGridView.Rows)
            {
                AutoSizeRow(row);
            }
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

            var messageWindowDialog = new MessageWindowDialog(owner, (string) messagesDataGridView.CurrentRow.Cells[messageDataGridViewTextBoxColumn.Index].Value);

            messageWindowDialog.ShowDialog();
        }

        private class MessageData
        {
            public string ImageName { get; set; }
            public DateTime Time { get; set; }
            public string Message { get; set; }
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

        #region IMessageWindow Members

        /// <summary>
        /// Appends message to the Top of the messagewindow
        /// </summary>
        /// <param name="level"></param>
        /// <param name="time"></param>
        /// <param name="message"></param>
        public void AddMessage(Level level, DateTime time, string message)
        {
            newMessages.Enqueue(new MessageData
            {
                ImageName = level.ToString(), Time = time, Message = message
            });
            Invalidate();
        }

        /// <summary>
        /// Clears all messages from the view
        /// </summary>
        public void Clear()
        {
            messageWindowData.Clear();
        }

        public bool IsMessageLevelEnabled(Level level)
        {
            if (level == Level.Warn && !buttonShowWarning.Checked)
            {
                return false;
            }
            if (level == Level.Info && !buttonShowInfo.Checked)
            {
                return false;
            }
            if (level == Level.Error && !buttonShowError.Checked)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}