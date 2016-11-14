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
using System.Windows.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.ProgressDialog
{
    [TestFixture]
    public class ProgressReporterTest
    {
        [Test]
        [STAThread]
        public void Constructor_InvalidSynchronizationContext_ThrowsInvalidOperationException()
        {
            // Call
            TestDelegate call = () => { new ProgressReporter(); };

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void ReportProgress_ActionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var form = new TestForm())
            {
                ProgressReporter progressReporter = form.ProgressReporter;

                // Call
                TestDelegate call = () => progressReporter.ReportProgress(null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("action", paramName);
            }
        }

        [Test]
        public void RegisterContinuation_TaskIsNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var form = new TestForm())
            {
                ProgressReporter progressReporter = form.ProgressReporter;

                // Call
                TestDelegate call = () => progressReporter.RegisterContinuation(null, () => { });

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("task", paramName);
            }
        }

        private class TestForm : Form
        {
            public TestForm()
            {
                ProgressReporter = new ProgressReporter();
            }

            public ProgressReporter ProgressReporter { get; private set; }
        }
    }
}