using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.DataEditorGenerator
{
    public static class LinkedToolTip
    {
        private static readonly InteractiveToolTip InteractiveToolTip = new InteractiveToolTip
        {
            UseFading = true,
            UseAnimation = true
        };

        public static void SetToolTip(Control targetControl, string text, string linkedString)
        {
            InteractiveToolTip.Show(CreateToolTipControl(text, linkedString), targetControl,
                                    new Point(targetControl.Width, 0),
                                    InteractiveToolTip.StemPosition.BottomLeft, 1000);
        }

        public static void HideToolTip()
        {
            InteractiveToolTip.Hide();
        }

        public static void Dispose()
        {
            InteractiveToolTip.Dispose();
        }

        private static Control CreateToolTipControl(string text, string linkedString)
        {
            var split = text.Split('\n');
            var widthChars = split.Select(s => s.Length).Max();
            var heightChars = split.Count();

            const int charWidth = 5;
            const int charHeight = 15;
            var heightFactor = linkedString == null ? heightChars : heightChars + 2;
            var control = new Panel
            {
                Width = widthChars*charWidth, Height = heightFactor*charHeight
            };
            var textLabel = new Label
            {
                Text = text,
                Width = widthChars*charWidth,
                Height = heightChars*charHeight,
                BackColor = Color.Transparent,
                Dock = DockStyle.Top
            };
            control.Controls.Add(textLabel);

            if (linkedString != null)
            {
                var linkLabel = new LinkLabel
                {
                    Text = "more...", BackColor = Color.Transparent, Dock = DockStyle.Bottom
                };
                linkLabel.LinkClicked += (s, e) => TryOpenUrl(linkedString);
                control.Controls.Add(linkLabel);
            }
            control.BackColor = Color.Transparent;
            return control;
        }

        private static bool TryOpenUrl(string url)
        {
            try
            {
                Process.Start(url);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open requested url");
                return false;
            }
        }
    }
}