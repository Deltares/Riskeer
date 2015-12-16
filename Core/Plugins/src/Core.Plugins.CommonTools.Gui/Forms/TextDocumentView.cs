﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Forms
{
    public partial class TextDocumentView : UserControl, ISearchableView
    {
        private bool textModified;
        private bool settingContent;
        private string characters = "";
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

        public Image Image
        {
            get { return Resources.TextDocument; }
            set {}
        }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item)
        {
            var text = item as string;
            if (text == null)
            {
                return;
            }

            textBox.SelectionStart = textBox.Text.IndexOf(text, 0, StringComparison.InvariantCulture);
            textBox.ScrollToCaret();
            textBox.SelectionLength = text.Length;
        }

        public IEnumerable<Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled, Action<int> setProgressPercentage)
        {
            var lines = textDocument.Content.Split(new[]
            {
                "\r",
                "\n",
                "\n\r"
            }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Where(l => caseSensitive ? l.Contains(text) : l.ToLower().Contains(text.ToLower())).Select(l => new Tuple<string, object>(l, l));
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (textModified && !IsDisposed)
            {
                settingContent = true;

                textDocument.Content = textBox.Text;

                characters = "";
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

            characters = "";
            timer.Stop();
            settingContent = false;

            textModified = false;
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys != Keys.Control)
            {
                if (e.KeyChar == (char) Keys.Return)
                {
                    characters += "<ENT>";
                }
                else if (e.KeyChar == '\b' || e.KeyChar == (char) Keys.Delete)
                {
                    characters += "<DEL>";
                }
                else
                {
                    characters += e.KeyChar;
                }
            }
            OnKeyPress(e);
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (e.KeyCode == Keys.V)
                {
                    characters += "<PASTE>";
                }
                if (e.KeyCode == Keys.X)
                {
                    characters += "<CUT>";
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                characters += "<DEL>";
            }
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