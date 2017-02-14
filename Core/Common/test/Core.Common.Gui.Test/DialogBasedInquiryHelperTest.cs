// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class DialogBasedInquiryHelperTest : NUnitFormTest
    {
        private IWin32Window dialogParent;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            dialogParent = mocks.StrictMock<IWin32Window>();
        }

        public override void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void DefaultConstructor_CreatesNewInquiryHelper()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var helper = new DialogBasedInquiryHelper(dialogParent);

            // Assert
            Assert.IsInstanceOf<IInquiryHelper>(helper);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetSourceFileLocation_Always_ShowsOpenFileDialog()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);

            string windowName = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new OpenFileDialogTester(wnd);
                windowName = name;
                tester.ClickCancel();
            };

            // Call
            helper.GetSourceFileLocation();

            // Assert
            Assert.AreEqual("Openen", windowName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetSourceFileLocation_CancelClicked_ResultFileSelectedIsFalse()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new OpenFileDialogTester(wnd);
                tester.ClickCancel();
            };

            // Call
            var result = helper.GetSourceFileLocation();

            // Assert
            Assert.IsFalse(result.HasFilePath);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetSourceFileLocation_ExistingFileSelected_ResultFileSelectedIsTrueFileNameSet()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);
            string expectedFilePath = Path.GetFullPath(Path.GetRandomFileName());

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new OpenFileDialogTester(wnd);
                tester.OpenFile(expectedFilePath);
            };

            using (new FileDisposeHelper(expectedFilePath))
            {
                // Call
                var result = helper.GetSourceFileLocation();

                // Assert
                Assert.IsTrue(result.HasFilePath);
                Assert.AreEqual(expectedFilePath, result.FilePath);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetTargetFileLocation_Always_ShowsOpenFileDialog()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);

            string windowName = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new OpenFileDialogTester(wnd);
                windowName = name;
                tester.ClickCancel();
            };

            // Call
            helper.GetTargetFileLocation();

            // Assert
            Assert.AreEqual("Opslaan als", windowName);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetTargetFileLocation_CancelClicked_ResultFileSelectedIsFalse()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new SaveFileDialogTester(wnd);
                tester.ClickCancel();
            };

            // Call
            var result = helper.GetTargetFileLocation();

            // Assert
            Assert.IsFalse(result.HasFilePath);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetTargetFileLocation_FileSelected_ResultFileSelectedIsTrueFileNameSet()
        {
            // Setup
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);
            string expectedFilePath = Path.GetFullPath(Path.GetRandomFileName());

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new SaveFileDialogTester(wnd);
                tester.SaveFile(expectedFilePath);
            };

            // Call
            var result = helper.GetTargetFileLocation();

            // Assert
            Assert.IsTrue(result.HasFilePath);
            Assert.AreEqual(expectedFilePath, result.FilePath);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireContinuation_OkOrCancelClicked_ReturnExpectedResult(bool confirm)
        {
            // Setup
            dialogParent.Expect(d => d.Handle).Repeat.AtLeastOnce().Return(default(IntPtr));
            mocks.ReplayAll();

            var helper = new DialogBasedInquiryHelper(dialogParent);

            string expectedQuery = "Are you sure you want to do this?";
            string expectedTitle = "Bevestigen";
            string query = null;
            string title = null;

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                query = tester.Text;
                title = tester.Title;
                if (confirm)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            // Call
            var result = helper.InquireContinuation(expectedQuery);

            // Assert
            Assert.AreEqual(expectedQuery, query);
            Assert.AreEqual(expectedTitle, title);
            Assert.AreEqual(confirm, result);
        }
    }
}