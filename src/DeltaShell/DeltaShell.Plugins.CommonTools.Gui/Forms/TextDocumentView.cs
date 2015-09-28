﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Editing;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Forms
{
    public partial class TextDocumentView : UserControl, ISearchableView
    {
        private TextDocumentBase textDocument;

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
            get { return textDocument; }
            set
            {
                if (textDocument != null)
                {
                    ((INotifyPropertyChange)textDocument).PropertyChanged -= TextDocumentView_PropertyChanged;
                }

                textDocument = value as TextDocumentBase;

                if (textDocument != null)
                {
                    textBox.Text = textDocument.Content;
                    textBox.ReadOnly = textDocument.ReadOnly;
                    ((INotifyPropertyChange)textDocument).PropertyChanged += TextDocumentView_PropertyChanged;
                }

                /*if (textDocument == null)
                {
                    //special null treatment just assigning null to datasource gives exception...
                    //http://stackoverflow.com/questions/220392/cannot-bind-to-the-property-or-column-name-on-the-datasource-parameter-name-dat
                    textDocumentBindingSource.DataSource = typeof(TextDocument);    
                }
                else
                {
                    textDocumentBindingSource.DataSource = textDocument;    
                }*/

                if (value == null)
                {
                    return;
                }

                //textBox.Lines = new[] { textDocument.Content }; // TODO: improve it!
                textBox.SelectionStart = textBox.TextLength; //set caret to end position
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (textModified && !IsDisposed)
            {
                settingContent = true;

                textDocument.BeginEdit(new DefaultEditAction(Resources.TextDocumentView_OnVisibleChanged_Edit_text__ + characters));
                textDocument.Content = textBox.Text;
                textDocument.EndEdit();

                characters = "";
                settingContent = false;

                textModified = false;
            }
        }
        
        void TextDocumentView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content" && !settingContent && !IsDisposed)
            {
                SetContent();
            }
        }

        [InvokeRequired]
        private void SetContent()
        {
            settingContent = true;
            textBox.Text = textDocument.Content;
            settingContent = false;
        }

        public Image Image
        {
            get { return TextDocumentImage; }
            set { }
        }

        public void EnsureVisible(object item)
        {
            var text = item as string;
            if (text == null) return;

            textBox.SelectionStart = textBox.Text.IndexOf(text, 0, StringComparison.InvariantCulture);
            textBox.ScrollToCaret();
            textBox.SelectionLength = text.Length;
        }

        public ViewInfo ViewInfo { get; set; }

        public IEnumerable<System.Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled, Action<int> setProgressPercentage)
        {
            var lines = textDocument.Content.Split(new[] { "\r","\n","\n\r" }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Where(l => caseSensitive ? l.Contains(text) : l.ToLower().Contains(text.ToLower())).Select(l => new System.Tuple<string,object>(l,l)); 
        }

        private bool textModified;

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (settingContent || IsDisposed) return;

            textModified = true;

            timer.Stop();
            timer.Start();
        }

        private string characters = "";
        private bool settingContent = false;
        private static readonly Bitmap TextDocumentImage = Properties.Resources.TextDocument;

        private void TimerTick(object sender, EventArgs e)
        {
            settingContent = true;

            textDocument.BeginEdit(new DefaultEditAction(Resources.TextDocumentView_OnVisibleChanged_Edit_text__ + characters));
            textDocument.Content = textBox.Text;
            textDocument.EndEdit();

            characters = "";
            timer.Stop();
            settingContent = false;

            textModified = false;
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys != Keys.Control)
            {
                if(e.KeyChar == (char)Keys.Return)
                {
                    characters += "<ENT>";
                }
                else if(e.KeyChar == '\b' || e.KeyChar == (char)Keys.Delete)
                {
                    characters += "<DEL>";
                }
                else
                {
                    characters += e.KeyChar;
                }   
            }
            base.OnKeyPress(e);
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if(e.KeyCode == Keys.V)
                {
                    characters += "<PASTE>";
                }
                if(e.KeyCode == Keys.X)
                {
                    characters += "<CUT>";
                }
            }
            else if(e.KeyCode == Keys.Delete)
            {
                characters += "<DEL>";
            }
            base.OnKeyUp(e);
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
