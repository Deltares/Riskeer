// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Common.Controls.TextEditor;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class CommentViewTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var view = new CommentView())
            {
                // Assert
                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
                Control control = view.Controls[0];
                Assert.IsInstanceOf<RichTextBoxControl>(control);
            }
        }

        [Test]
        public void Data_Comment_DataSet()
        {
            // Setup
            var data = new Comment();

            using (var view = new CommentView())
            {
                // Call
                view.Data = data;

                // Assert
                Assert.AreSame(data, view.Data);
            }
        }

        [Test]
        public void Data_NoComment_DataNull()
        {
            // Setup
            using (var view = new CommentView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_ContainsComment_CommentSetOnRichTextEditor()
        {
            // Setup
            const string expectedText = "<Some_text>";
            string validRtfString = GetValidRtfString(expectedText);

            var data = new Comment
            {
                Body = validRtfString
            };

            using (var view = new CommentView())
            {
                // Call
                view.Data = data;

                // Assert
                var textBoxControl = view.Controls[0] as RichTextBoxControl;
                Assert.IsNotNull(textBoxControl);
                Assert.AreEqual(validRtfString, textBoxControl.Rtf);
            }
        }

        [Test]
        public void RichTextEditorOnTextChanged_Always_SetsComment()
        {
            // Setup
            var data = new Comment();

            using (var form = new Form())
            using (var view = new CommentView())
            {
                form.Controls.Add(view);
                form.Show();

                // Set data after showing control:
                view.Data = data;

                // Precondition
                Assert.AreEqual(GetValidRtfString(""), data.Body);

                const string expectedText = "<Some_text>";
                string validRtfString = GetValidRtfString(expectedText);

                var richTextBoxControl = (RichTextBoxControl) new ControlTester("richTextBoxControl").TheObject;

                // Call
                richTextBoxControl.Rtf = validRtfString;

                // Assert
                Assert.AreEqual(validRtfString, data.Body);
            }
        }

        [Test]
        public void RichTextEditorOnTextChanged_WithoutData_DoesNotChangeComment()
        {
            // Setup
            using (var form = new Form())
            using (var view = new CommentView())
            {
                form.Controls.Add(view);
                form.Show();

                view.Data = null;

                const string expectedText = "<Some_text>";
                string validRtfString = GetValidRtfString(expectedText);

                var richTextBoxControl = (RichTextBoxControl) new ControlTester("richTextBoxControl").TheObject;

                // Call
                TestDelegate test = () => richTextBoxControl.Rtf = validRtfString;

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        private static string GetValidRtfString(string value)
        {
            using (var richTextBox = new RichTextBox
            {
                Text = value
            })
            {
                return richTextBox.Rtf;
            }
        }
    }
}