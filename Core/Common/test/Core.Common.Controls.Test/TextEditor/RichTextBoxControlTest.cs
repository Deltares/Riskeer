﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.TextEditor;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test.TextEditor
{
    [TestFixture]
    public class RichTextBoxControlTest
    {
        private static RichTextBox tempRichTextBox;
        private const string text = "Test DEZE regel";

        [SetUp]
        public void SetUp()
        {
            tempRichTextBox = new RichTextBox
            {
                Text = text
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (tempRichTextBox != null)
            {
                tempRichTextBox.Dispose();
                tempRichTextBox = null;
            }
        }

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

                // Call
                richTextBox.Text = "Test";

                // Assert
                Assert.AreEqual(1, eventCounter);
                Assert.IsInstanceOf<RichTextBoxControl>(eventSender);
            }
        }

        [Test]
        [TestCase(Keys.B, true, false ,false)]
        [TestCase(Keys.I, false, true, false)]
        [TestCase(Keys.U, false, false, true)]
        public void RichTextBoxControl_TextDoesNotHaveStyleOnStyleKeyDown_SelectionFontStyleApplied(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;
                richTextBox.AppendText(text);

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                // Precondition
                Assert.False(richTextBox.SelectionFont.Bold);
                Assert.False(richTextBox.SelectionFont.Italic);
                Assert.False(richTextBox.SelectionFont.Underline);
                Assert.AreEqual(GetValidRtfString(text), richTextBox.Rtf);

                // Call
                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Assert
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
                Assert.AreEqual(GetValidRtfString(5, 4, bold, italic, underline), richTextBox.Rtf);
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
                richTextBox.AppendText(text);

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Precondition
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
                Assert.AreEqual(GetValidRtfString(5, 4, bold, italic, underline), richTextBox.Rtf);

                // Call
                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Assert
                Assert.IsFalse(richTextBox.SelectionFont.Bold);
                Assert.IsFalse(richTextBox.SelectionFont.Italic);
                Assert.IsFalse(richTextBox.SelectionFont.Underline);
                Assert.AreEqual(GetValidRtfString(5, 4, bold, italic, underline), richTextBox.Rtf);
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
                richTextBox.AppendText(text);

                richTextBox.SelectionStart = 5;
                richTextBox.SelectionLength = 4;

                // Precondition
                Assert.AreEqual(GetValidRtfString(text), richTextBox.Rtf);

                // Call
                foreach (var key in keys)
                {
                    EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key| Keys.Control));
                }

                // Assert
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(GetValidRtfString(5, 4, bold, italic, underline), richTextBox.Rtf);
            }
        }

        [Test]
        [TestCase(Keys.B, true, false, false)]
        [TestCase(Keys.I, false, true, false)]
        [TestCase(Keys.U, false, false, true)]
        public void RichTextBoxControl_SetStyleBeforeAddingText_AddedTextHasStyle(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;

                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Call
                richTextBox.AppendText(text);

                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = 4;

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
        public void RichTextBoxControl_AddCharachterSetStyleAddCharacter_FirstCharacterNoStyleSecondCharacterStyleApplied(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox) new ControlTester("richTextBox").TheObject;

                richTextBox.AppendText("A");

                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                // Call
                richTextBox.AppendText("B");

                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = 1;

                // Assert
                Assert.IsFalse(richTextBox.SelectionFont.Bold);
                Assert.IsFalse(richTextBox.SelectionFont.Italic);
                Assert.IsFalse(richTextBox.SelectionFont.Underline);

                richTextBox.SelectionStart = 1;

                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);
            }
        }

        [Test]
        [TestCase(Keys.B, true, false, false)]
        [TestCase(Keys.I, false, true, false)]
        [TestCase(Keys.U, false, false, true)]
        public void RichTextBoxControl_AddCharachterSetStyleAddCharacterRemoveAllCharachtersAndAddAgain_FirstCharacterNoStyleSecondCharacterStyleAppliedAfterRemoveNoStyleApplied(Keys key, bool bold, bool italic, bool underline)
        {
            // Setup
            using (var form = new Form())
            {
                var control = new RichTextBoxControl();
                form.Controls.Add(control);
                form.Show();

                var richTextBox = (RichTextBox)new ControlTester("richTextBox").TheObject;

                richTextBox.AppendText("A");

                EventHelper.RaiseEvent(richTextBox, "KeyDown", new KeyEventArgs(key | Keys.Control));

                richTextBox.AppendText("B");

                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = 1;

                // Precondition
                Assert.IsFalse(richTextBox.SelectionFont.Bold);
                Assert.IsFalse(richTextBox.SelectionFont.Italic);
                Assert.IsFalse(richTextBox.SelectionFont.Underline);

                richTextBox.SelectionStart = 1;

                Assert.AreEqual(bold, richTextBox.SelectionFont.Bold);
                Assert.AreEqual(italic, richTextBox.SelectionFont.Italic);
                Assert.AreEqual(underline, richTextBox.SelectionFont.Underline);

                // Call
                richTextBox.Text = string.Empty;
                richTextBox.AppendText("C");

                richTextBox.SelectionStart = 0;

                // Assert
                Assert.IsFalse(richTextBox.SelectionFont.Bold);
                Assert.IsFalse(richTextBox.SelectionFont.Italic);
                Assert.IsFalse(richTextBox.SelectionFont.Underline);
            }
        }

        private static string GetValidRtfString(string value)
        {
            tempRichTextBox.Text = value;

            return tempRichTextBox.Rtf;
        }

        private static string GetValidRtfString(int selectionStart, int selectionLength, bool bold, bool italic, bool underline)
        {
            tempRichTextBox.SelectionStart = selectionStart;
            tempRichTextBox.SelectionLength = selectionLength;

            FontStyle newStyle = tempRichTextBox.SelectionFont.Style;

            if (bold)
            {
                newStyle = newStyle ^ FontStyle.Bold;
            }

            if (italic)
            {
                newStyle = newStyle ^ FontStyle.Italic;
            }

            if (underline)
            {
                newStyle = newStyle ^ FontStyle.Underline;
            }

            tempRichTextBox.SelectionFont = new Font(tempRichTextBox.SelectionFont, newStyle);

            return tempRichTextBox.Rtf;
        }
    }
}