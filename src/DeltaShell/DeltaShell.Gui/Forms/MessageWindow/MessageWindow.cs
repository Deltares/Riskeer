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

#if MONO
using System.Drawing;
#endif

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
        private bool firstMessage;
        private readonly IList<MessageData> newMessages = new List<MessageData>();
        private bool filtering;

        public MessageWindow()
        {
            Text = Resources.MessageWindow_MessageWindow_Messages;
            MessageWindowLogAppender.MessageWindow = this;
#if !MONO
            InitializeComponent();
#else
            InitializeMonoComponent();
#endif

            if (!log.IsDebugEnabled)
            {
                buttonShowDebug.Visible = false;
            }

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

#if !MONO
            /// Apply sorting so the messages added last will be on Top.
            messagesBindingSource.Sort = "Id";
            messagesBindingSource.ApplySort(messagesBindingSource.SortProperty, ListSortDirection.Descending);
            ApplyFilter();
            messagesDataGridView.CellFormatting += MessagesDataGridViewCellFormatting;

#endif
            Image = Resources.application_view_list;

            MaxMessageCount = 100;

            // TODO: make timer start only when property was changed and then stop
            addNewMessagesTimer = new Timer();
            firstMessage = true;
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

        public int MaxMessageCount { get; set; }

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
#if !MONO
            newMessages.Add(new MessageData
            {
                ImageName = level.ToString(), Time = time, Message = message
            });
#else                    
            if (messagesDataGridView != null && messagesDataGridView.Columns.Count != 0)
            {
                messagesDataGridView.Rows.Add(new object[] { level.ToString(), timeStamp.ToLongTimeString(), source, message});
                
                //Time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
                Time.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Source.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                LevelColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                LogMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                messagesDataGridView.Invalidate(true);
            }
#endif
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
            if (level == Level.Debug && !buttonShowDebug.Checked)
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

            Refresh();

            while (newMessages.Count != 0)
            {
                var newMessage = newMessages[0];
                newMessages.RemoveAt(0);
                if (newMessage != null)
                {
                    messageWindowData.Messages.AddMessagesRow(newMessage.ImageName, newMessage.Time, newMessage.Message);

                    if (newMessage.ImageName == "ERROR" && Error != null)
                    {
                        Error(this, null);
                    }
                }
            }

            messagesDataGridView.ResumeLayout();

            if (firstMessage)
            {
                firstMessage = false;
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

#if !MONO
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
#endif

#if MONO

        private DataGridViewTextBoxColumn Time;
        private DataGridViewTextBoxColumn Source;
        private DataGridViewTextBoxColumn LogMessage;
        private DataGridViewTextBoxColumn LevelColumn;
        private void InitializeMonoComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof (MessageWindow));
            messagesDataGridView = new DataGridView();
            LevelColumn = new DataGridViewTextBoxColumn();
            Time = new DataGridViewTextBoxColumn();
            Source = new DataGridViewTextBoxColumn();
            LogMessage = new DataGridViewTextBoxColumn();
            contextMenu = new ContextMenuStrip(components);
            buttonCopy = new ToolStripMenuItem();
            cToolStripMenuItem = new ToolStripSeparator();
            buttonClearAll = new ToolStripMenuItem();
            levelImages = new ImageList(components);
            text = new TextBox();
            ((ISupportInitialize) (messagesDataGridView)).BeginInit();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // messagesDataGridView
            // 
            messagesDataGridView.AllowUserToAddRows = false;
            messagesDataGridView.AllowUserToResizeRows = false;
//            this.messagesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            messagesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messagesDataGridView.Columns.AddRange(new DataGridViewColumn[]
                                               {
                                                   LevelColumn,
                                                   Time,
                                                   Source,
                                                   LogMessage
                                               });
            messagesDataGridView.ContextMenuStrip = contextMenu;
            messagesDataGridView.Dock = DockStyle.Fill;
            messagesDataGridView.Location = new Point(0, 0);
            messagesDataGridView.Margin = new Padding(2);
            messagesDataGridView.Name = "messagesDataGridView";
            messagesDataGridView.ReadOnly = true;
            messagesDataGridView.RowHeadersVisible = false;
            messagesDataGridView.RowTemplate.Height = 24;
            messagesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//            this.messagesDataGridView.Size = new System.Drawing.Size(607, 368);
//            this.messagesDataGridView.TabIndex = 3;
            // 
            // Level
            // 
            LevelColumn.HeaderText = "Level";
            LevelColumn.Name = "Level";
            LevelColumn.ReadOnly = true;
//            this.Level.Resizable = System.Windows.Forms.DataGridViewTriState.True;
//            this.Level.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Time
            // 
            Time.HeaderText = "Time";
            Time.Name = "Time";
            Time.ReadOnly = true;
            // 
            // Source
            // 
            Source.HeaderText = "Source";
            Source.Name = "Source";
            Source.ReadOnly = true;
            // 
            // LogMessage
            // 
            LogMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            LogMessage.HeaderText = "Message";
            LogMessage.Name = "LogMessage";
            LogMessage.ReadOnly = true;
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[]
                                           {
                                               buttonCopy,
                                               cToolStripMenuItem,
                                               buttonClearAll
                                           });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(153, 76);
            // 
            // buttonCopy
            // 
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new Size(152, 22);
            buttonCopy.Text = "&Copy";
            buttonCopy.Click += buttonCopy_Click;
            // 
            // cToolStripMenuItem
            // 
            cToolStripMenuItem.Name = "cToolStripMenuItem";
            cToolStripMenuItem.Size = new Size(149, 6);
            // 
            // buttonClearAll
            // 
            buttonClearAll.Name = "buttonClearAll";
            buttonClearAll.Size = new Size(152, 22);
            buttonClearAll.Text = "Clear &All";
            buttonClearAll.Click += buttonClearAll_Click;
            // 
            // levelImages
            // 
            levelImages.ImageStream = ((ImageListStreamer) (resources.GetObject("levelImages.ImageStream")));
            levelImages.TransparentColor = Color.Transparent;
            levelImages.Images.SetKeyName(0, "error.png");
            levelImages.Images.SetKeyName(1, "info.png");
            levelImages.Images.SetKeyName(2, "warning.png");
            levelImages.Images.SetKeyName(3, "debug.png");
            // 
            // text
            // 
            text.Location = new Point(111, 162);
            text.Name = "text";
            text.Size = new Size(100, 96);
            text.TabIndex = 4;
            text.Text = "";
            text.Visible = false;
            // 
            // MessageWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(text);
            Controls.Add(messagesDataGridView);
            Margin = new Padding(2);
            Name = "MessageWindow";
            Size = new Size(607, 368);
            ((ISupportInitialize) (messagesDataGridView)).EndInit();
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }
#endif

        private void ApplyFilter()
        {
            filtering = true;
            List<string> filterlines = new List<string>();
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
            if (buttonShowDebug.Checked)
            {
                filterlines.Add(string.Format("Image = '{0}'", Level.Debug));
            }

            if (filterlines.Count == 0)
            {
                messagesBindingSource.Filter = "Image = 'NOTHING SHOWN'";
            }
            else
            {
                string filter = filterlines[0];
                for (int i = 1; i < filterlines.Count; i++)
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

        private void ButtonShowDebugClick(object sender, EventArgs e)
        {
            buttonShowDebug.Checked = !buttonShowDebug.Checked;
            ApplyFilter();
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

        private void ButtonClearAllMessagesClick(object sender, EventArgs e)
        {
            Clear();
        }

        public event EventHandler Error;

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (messagesDataGridView.CurrentRow == null)
            {
                return;
            }

            var form = new Form
            {
                Text = Resources.MessageWindow_showDetailsToolStripMenuItem_Click_Message_details,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable,
                ShowInTaskbar = false,
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