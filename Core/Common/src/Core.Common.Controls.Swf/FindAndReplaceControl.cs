using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using log4net;

namespace Core.Common.Controls.Swf
{
    public partial class FindAndReplaceControl : UserControl
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FindAndReplaceControl));
        private MatchCollection matches;
        private bool isDirty;
        private string textToSearch;
        private Action<int, int, string> replaceText;

        public FindAndReplaceControl()
        {
            InitializeComponent();
            ShowReplaceControls(false);
        }

        public TextBox FindTextBox { get; private set; }

        public TextBox ReplaceTextBox { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<string> GetTextToSearch { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<int> GetCurrentPosition { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<int> ScrollToPosition { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<int, int> SelectText { get; set; }

        public Action<int, int, string> ReplaceText
        {
            get
            {
                return replaceText;
            }
            set
            {
                replaceText = value;
                ShowReplaceControls(replaceText != null);
            }
        }

        public Action<string> HighLightText { get; set; }

        private void ShowReplaceControls(bool show)
        {
            labelReplaceWith.Visible = show;
            ReplaceTextBox.Visible = show;
            buttonReplace.Visible = show;
            buttonReplaceAll.Visible = show;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void Replace()
        {
            var currentPosition = GetCurrentPosition();

            SyncMatches();

            var currentMatch = matches.OfType<Match>().FirstOrDefault(m => m.Index == currentPosition);
            if (currentMatch == null)
            {
                FindNext();
                return;
            }

            if (ReplaceText != null)
            {
                ReplaceText(currentMatch.Index, currentMatch.Length, ReplaceTextBox.Text);
            }

            FindNext();
        }

        private void FindNext()
        {
            SyncMatches();

            textToSearch = GetTextToSearch();

            if (string.IsNullOrEmpty(textToSearch))
            {
                Log.Warn("There is no text to search.");
                return;
            }

            var nextMatch = matches.OfType<Match>().FirstOrDefault(m => m.Index > GetCurrentPosition());
            if (nextMatch == null)
            {
                Log.Info("End of document has been reached.");
                nextMatch = matches.OfType<Match>().FirstOrDefault();
            }

            if (nextMatch != null && SelectText != null)
            {
                SelectText(nextMatch.Index, FindTextBox.Text.Length);
                if (ScrollToPosition != null)
                {
                    ScrollToPosition(nextMatch.Index);
                }
            }
        }

        private void SyncMatches()
        {
            var currentText = GetTextToSearch();

            if (currentText != textToSearch)
            {
                isDirty = true;
                textToSearch = currentText;
            }

            var expr = new Regex(Regex.Escape(FindTextBox.Text));

            if (matches == null || isDirty)
            {
                matches = expr.Matches(textToSearch);
                isDirty = false;
            }
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Visible = false;
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                FindNext();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            isDirty = true;
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            Replace();
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            if (ReplaceText == null)
            {
                return;
            }

            SyncMatches();

            foreach (var match in matches.OfType<Match>().Reverse())
            {
                ReplaceText(match.Index, match.Length, ReplaceTextBox.Text);
            }
            Log.InfoFormat("Replaced {0} occurrences of {1} with {2}", matches.Count, FindTextBox.Text, ReplaceTextBox.Text);
        }

        private void button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Visible = false;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                ((Button) sender).PerformClick();
            }
        }

        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            if (sender == FindTextBox && HighLightText != null)
            {
                HighLightText(FindTextBox.Text);
            }

            isDirty = true;
        }

        private void FindAndReplaceControl_VisibleChanged(object sender, EventArgs e)
        {
            if (HighLightText == null || Visible)
            {
                return;
            }

            HighLightText("");
        }
    }
}