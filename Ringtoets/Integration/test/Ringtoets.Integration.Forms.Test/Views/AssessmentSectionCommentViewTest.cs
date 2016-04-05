// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionCommentViewTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var view = new AssessmentSectionCommentView();

            // Assert
            Assert.IsInstanceOf<IView>(view);
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsNull(view.Data);
            Assert.AreEqual(1, view.Controls.Count);
            var control = view.Controls[0];
            Assert.IsInstanceOf<RichTextBoxControl>(control);
        }

        [Test]
        public void Data_AssessmentSection_DataSet()
        {
            // Setup
            var mocks = new MockRepository();
            var view = new AssessmentSectionCommentView();
            var data = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            view.Data = data;

            // Assert
            Assert.AreSame(data, view.Data);
        }

        [Test]
        public void Data_NoAssessmentSection_DataNull()
        {
            // Setup
            var view = new AssessmentSectionCommentView();
            var data = new object();

            // Call
            view.Data = data;

            // Assert
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Data_AssessmentSectionContainsComment_CommentSetOnRichTextEditor()
        {
            // Setup
            var expectedText = "<Some text>";
            var mocks = new MockRepository();
            var view = new AssessmentSectionCommentView();
            var data = mocks.StrictMock<IAssessmentSection>();
            data.Expect(d => d.Comments).Return(expectedText);

            mocks.ReplayAll();

            // Call
            view.Data = data;

            // Assert
            Assert.AreEqual(expectedText, view.Controls[0].Text);
        }
    }
}