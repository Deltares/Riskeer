// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class WindowsFormsTestHelperTest
    {
        [Test]
        public void ShowActionIsRunForAFormUsingShow()
        {
            var form = new Form();
            var callCount = 0;

            WindowsFormsTestHelper.Show(form, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ShowActionIsRunForUserControl()
        {
            var uc = new UserControl();
            var callCount = 0;

            WindowsFormsTestHelper.Show(uc, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ShowActionIsRunForForm()
        {
            var form = new Form();
            var callCount = 0;

            WindowsFormsTestHelper.Show(form, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void Action_ThrowsException_StackTraceCorrectly()
        {
            TestDelegate testDelegate = () => WindowsFormsTestHelper.Show(new Label(), MethodWithException);

            var exception = Assert.Throws<InvalidOperationException>(testDelegate);
            Assert.AreEqual("my message", exception.Message);
            Assert.IsTrue(exception.StackTrace.Contains("MethodWithException"));

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void Thread_ThrowsUnhandledException_StackTraceCorrectly()
        {
            var thread = new Thread(MethodWithExceptionInSeparateThread);

            TestDelegate testDelegate = () => WindowsFormsTestHelper.Show(new Label(),
                                                                          delegate
                                                                          {
                                                                              thread.Start();
                                                                              thread.Join();
                                                                          });

            var exception = Assert.Throws<GuiTestHelper.UnhandledException>(testDelegate);

            Assert.IsTrue(exception.Message.Contains("Throwing this exception is intended and part of a test."));
            Assert.IsTrue(exception.StackTrace.Contains("MethodWithExceptionInSeparateThread"));

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void CallActionOnShowModal()
        {
            var callCount = 0;

            WindowsFormsTestHelper.ShowModal(new Form(), delegate { callCount++; });

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ShownCalledOnShowModal()
        {
            var callCount = 0;
            var form = new Form();

            form.Shown += delegate { callCount++; };

            WindowsFormsTestHelper.ShowModal(form);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ShowModal_ShownFails_ThrowsUnhandledException()
        {
            // Setup
            var form = new Form();
            form.Shown += delegate { Assert.Fail(); };

            // Call
            TestDelegate call = () => WindowsFormsTestHelper.ShowModal(form);

            // Assert
            Assert.Throws<GuiTestHelper.UnhandledException>(call);
        }

        private void MethodWithException(Form obj)
        {
            throw new InvalidOperationException("my message");
        }

        private void MethodWithExceptionInSeparateThread()
        {
            throw new InvalidOperationException("Throwing this exception is intended and part of a test.");
        }
    }
}