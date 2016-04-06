using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TextEditor;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test.TextEditor
{
    [TestFixture]
    public class RichTextBoxControlTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var control = new RichTextBoxControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
        }

        [Test]
        public void Rtf_ValueSetWhenLoaded_TextAsExpected()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var data = "<Some data>";

                // Call
                control.Rtf = GetValidRtfString(data);

                // Assert
                Assert.AreEqual(data, control.Controls[0].Text);
            }
        }

        [Test]
        public void Rtf_ValueSetWhenFormNotLoaded_TextAsExpected()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var control = new RichTextBoxControl();
                
                var data = "<Some data>";

                control.Rtf = GetValidRtfString(data);
                form.Controls.Add(control);
                
                // Call
                form.Show();

                // Assert
                Assert.AreEqual(data, control.Controls[0].Text);
            }
        }

        [Test]
        public void Rtf_ValueSetWhenLoaded_ReturnsValue()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var data = "<Some data>";
                var validRtfString = GetValidRtfString(data);

                // Call
                control.Rtf = validRtfString;

                // Assert
                Assert.AreEqual(validRtfString, control.Rtf);
            }
        }

        [Test]
        public void Rtf_ValueSetWhenFormNotLoaded_ReturnsValue()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var control = new RichTextBoxControl();

                var data = "<Some data>";
                var validRtfString = GetValidRtfString(data);

                control.Rtf = validRtfString;
                form.Controls.Add(control);

                // Call
                form.Show();

                // Assert
                Assert.AreEqual(validRtfString, control.Rtf);
            }
        }

        [Test]
        public void RichTextBoxControl_RichTextBoxTextChanged_TextBoxValueChangedEventRaised()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox) new ControlTester("richTextBox").TheObject;

                int eventCounter = 0;
                object eventSender = null;
                EventArgs sendEventArgs = null;
                control.TextBoxValueChanged += (sender, args) =>
                {
                    eventSender = sender;
                    sendEventArgs = args;
                    eventCounter++;
                };

                var eventArgs = new EventArgs();

                // Call
                EventHelper.RaiseEvent(richTextBox, "TextChanged", eventArgs);

                // Assert
                Assert.AreEqual(1, eventCounter);
                Assert.AreSame(eventArgs, sendEventArgs);
                Assert.IsInstanceOf<RichTextBoxControl>(eventSender);
            }
        }

        [Test]
        [TestCase(Keys.B, true, false ,false)]
        [TestCase(Keys.I, false, true ,false)]
        [TestCase(Keys.U, false, false ,true)]
        public void RichTextBoxControl_TextDoesNotHaveStyleOnStyleKeyDown_SetsStyleOnSelectedText(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;
                richTextBox.Text = "Test deze regel";

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                // Precondition
                Assert.False(richTextBox.SelectionFont.Bold);
                Assert.False(richTextBox.SelectionFont.Italic);
                Assert.False(richTextBox.SelectionFont.Underline);

                // Call
                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Assert
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
            }
        }

        [Test]
        [TestCase(Keys.B, true, false, false)]
        [TestCase(Keys.I, false, true, false)]
        [TestCase(Keys.U, false, false, true)]
        public void RichTextBoxControl_TextDoesHaveStyleOnStyleKeyDown_RemovesStyleFromSelectedText(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;
                richTextBox.Text = "Test deze regel";

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Precondition
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);

                // Call
                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Assert
                Assert.IsFalse(richTextBox.SelectionFont.Bold);
                Assert.IsFalse(richTextBox.SelectionFont.Italic);
                Assert.IsFalse(richTextBox.SelectionFont.Underline);
            }
        }

        [Test]
        [TestCase(new[] { Keys.U, Keys.B }, true, true, false)]
        [TestCase(new[] { Keys.U, Keys.I }, true, false, true)]
        [TestCase(new[] { Keys.B, Keys.I }, false, true, true)]
        [TestCase(new[] { Keys.B, Keys.I, Keys.U }, true, true, true)]
        public void RichTextBoxControl_SetDifferentStyles_SelectedTextHasDifferentStyles(IEnumerable<Keys> keys, bool underline, bool bold, bool italic)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;
                richTextBox.Text = "Test deze regel";

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                // Call
                foreach (var key in keys)
                {
                    EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key| Keys.Control));
                }

                // Assert
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
            }
        }

        private static string GetValidRtfString(string value)
        {
            RichTextBox richTextBox = new RichTextBox
            {
                Text = value
            };

            return richTextBox.Rtf;
        }
    }
}