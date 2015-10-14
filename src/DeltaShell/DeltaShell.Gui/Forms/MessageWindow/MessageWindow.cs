using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Gui.Properties;
using log4net;
using log4net.Core;

namespace DeltaShell.Gui.Forms.MessageWindow
{
    public partial class MessageWindow : UserControl, IMessageWindow
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MessageWindow));

        private class MessageData
        {
            public string ImageName { get; set; }
            public DateTime Time { get; set; }
            public string Message { get; set; }
        }

        private readonly Dictionary<string, string> levelImageName;
        private readonly Timer addNewMessagesTimer;
        private readonly IList<MessageData> newMessages = new List<MessageData>();
        private bool filtering;

        public MessageWindow()
        {
            Text = Resources.MessageWindow_MessageWindow_Messages;
            MessageWindowLogAppender.MessageWindow = this;
            InitializeComponent();

            levelImageName = new Dictionary<string, string>();

            // order is the same as in log4j Level (check sources of log4net)
            levelImageName[Level.Off.ToString()] = "error.png";
            levelImageName[Level.Emergency.ToString()] = "error.png";
            levelImageName[Level.Fatal.ToString()] = "error.png";
            levelImageName[Level.Alert.ToString()] = "error.png";
            levelImageName[Level.Critical.ToString()] = "error.png";
            levelImageName[Level.Severe.ToString()] = "error.png";
            levelImageName[Level.Error.ToString()] = "error.png";
            levelImageName[Level.Warn.ToString()] = "warning.png";
            levelImageName[Level.Notice.ToString()] = "warning.png";
            levelImageName[Level.Info.ToString()] = "info.png";
            levelImageName[Level.Debug.ToString()] = "debug.png";
            levelImageName[Level.Fine.ToString()] = "debug.png";
            levelImageName[Level.Trace.ToString()] = "debug.png";
            levelImageName[Level.Finer.ToString()] = "debug.png";
            levelImageName[Level.Verbose.ToString()] = "debug.png";
            levelImageName[Level.Finest.ToString()] = "debug.png";
            levelImageName[Level.All.ToString()] = "debug.png";
            messagesDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            messagesDataGridView.MouseUp += MessagesDataGridViewMouseUp;

            /// Apply sorting so the messages added last will be on Top.
            messagesBindingSource.Sort = "Id";
            messagesBindingSource.ApplySort(messagesBindingSource.SortProperty, ListSortDirection.Descending);
            ApplyFilter();
            messagesDataGridView.CellFormatting += MessagesDataGridViewCellFormatting;

            Image = Resources.application_view_list;

            // TODO: make timer start only when property was changed and then stop
            addNewMessagesTimer = new Timer();
            addNewMessagesTimer.Tick += AddNewMessagesTimerTick;

            addNewMessagesTimer.Interval = 300;

            // fixes DPI problem
            messagesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messagesDataGridView.RowsAdded += messagesDataGridView_RowsAdded;
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

        

        #region IView Members

        public object Data
        {
            get
            {
                return messageWindowData;
            }
            set {}
        }

        public Image Image { get; set; }

        public void EnsureVisible(object item) {}
        public ViewInfo ViewInfo { get; set; }

        #endregion

        #region IMessageWindow Members

        /// <summary>
        /// Appends message to the Top of the messagewindow
        /// </summary>
        /// <param name="level"></param>
        /// <param name="time"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void AddMessage(Level level, DateTime time, string message)
        {
            newMessages.Add(new MessageData
            {
                ImageName = level.ToString(), Time = time, Message = message
            });
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                addNewMessagesTimer.Start(); // refresh
            }
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

        private void AddNewMessagesTimerTick(object sender, EventArgs e)
        {
            if (newMessages.Count == 0)
            {
                return;
            }

            messagesDataGridView.SuspendLayout();
            try
            {
                Refresh();

                while (newMessages.Count != 0)
                {
                    var newMessage = newMessages[0];
                    newMessages.RemoveAt(0);

                    messageWindowData.Messages.AddMessagesRow(newMessage.ImageName, newMessage.Time, newMessage.Message);

                    if (newMessage.ImageName == "ERROR" && OnError != null)
                    {
                        OnError(this, null);
                    }
                }
            }
            finally
            {
                messagesDataGridView.ResumeLayout();
            }   

            if (messagesDataGridView.Rows.Count > 0)
            {
                messagesDataGridView.CurrentCell = messagesDataGridView.Rows[0].Cells[0];
            }
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

            string level = (string) e.Value;
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
                var filter = filterlines[0];
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

        public event EventHandler OnError;

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (messagesDataGridView.CurrentRow == null)
            {
                return;
            }

            var form = new Form
            {
                Text = Resources.MessageWindow_showDetailsToolStripMenuItem_Click_Message_details,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.Sizable,
                ShowInTaskbar = false,
                Icon = Resources.application_import_blue1,
                Size = new Size(300, 300)
            };

            var text = (string) messagesDataGridView.CurrentRow.Cells[messageDataGridViewTextBoxColumn.Index].Value;

            var textDocumentView = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Text = text,
                ReadOnly = true
            };

            form.Controls.Add(textDocumentView);
            form.Select();
            form.ShowDialog();
        }
    }
}