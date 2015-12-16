using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using Core.Common.Utils;

namespace Core.Plugins.CommonTools.Gui.Forms
{
    public partial class TextDocumentView : UserControl, IView
    {
        private bool textModified;
        private bool settingContent;
        private TextDocument textDocument;

        public TextDocumentView()
        {
            InitializeComponent();

            findAndReplaceControl1.GetTextToSearch = () => textBox.Text;
            findAndReplaceControl1.GetCurrentPosition = () => textBox.SelectionStart;
            findAndReplaceControl1.SelectText = (start, length) => textBox.Select(start, length);
            findAndReplaceControl1.ScrollToPosition = i =>
            {
                if (textBox.SelectionStart != i)
                {
                    textBox.Select(i, 0);
                }
                textBox.ScrollToCaret();
            };
        }

        public object Data
        {
            get
            {
                return textDocument;
            }
            set
            {
                textDocument = value as TextDocument;

                if (textDocument != null)
                {
                    textBox.Text = textDocument.Content;
                    textBox.ReadOnly = textDocument.ReadOnly;
                    textBox.SelectionStart = textBox.TextLength; // Set caret to end position
                }
                else
                {
                    textBox.Text = "";
                }
            }
        }

        public ViewInfo ViewInfo { get; set; }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (textModified && !IsDisposed)
            {
                settingContent = true;

                textDocument.Content = textBox.Text;

                settingContent = false;

                textModified = false;
            }
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (settingContent || IsDisposed)
            {
                return;
            }

            textModified = true;

            timer.Stop();
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            settingContent = true;

            textDocument.Content = textBox.Text;

            timer.Stop();
            settingContent = false;

            textModified = false;
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys != Keys.Control)
            {
                if (e.KeyChar == (char) Keys.Return)
                {}
                else if (e.KeyChar == '\b' || e.KeyChar == (char) Keys.Delete)
                {}
                else
                {}
            }
            OnKeyPress(e);
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (e.KeyCode == Keys.V)
                {}
                if (e.KeyCode == Keys.X)
                {}
            }
            else if (e.KeyCode == Keys.Delete)
            {}
            OnKeyUp(e);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control && e.Alt)
            {
                findAndReplaceControl1.Visible = true;
                findAndReplaceControl1.Focus();
                e.Handled = true;
            }
        }

        private void TextDocumentView_Load(object sender, EventArgs e)
        {
            textBox.SelectionStart = 0;
        }
    }
}