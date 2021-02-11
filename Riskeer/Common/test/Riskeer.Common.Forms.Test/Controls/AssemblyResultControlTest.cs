﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class AssemblyResultControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            using (var form = new Form())
                // Call
            using (var resultControl = new TestAssemblyResultControl())
            {
                form.Controls.Add(resultControl);
                form.Show();

                // Assert
                Assert.AreEqual(1, resultControl.Controls.Count);
                Assert.IsInstanceOf<UserControl>(resultControl);
                Assert.IsTrue(resultControl.AutoSize);
                Assert.AreEqual(DockStyle.Left, resultControl.Dock);

                TableLayoutPanel groupPanel = GetGroupPanel(resultControl);
                Assert.AreEqual(1, groupPanel.ColumnCount);
                Assert.AreEqual(1, groupPanel.RowCount);

                var groupLabel = (BorderedLabel) groupPanel.GetControlFromPosition(0, 0);
                Assert.IsTrue(groupLabel.AutoSize);
                Assert.AreEqual(DockStyle.Fill, groupLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), groupLabel.Padding);

                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                TestHelper.AssertImagesAreEqual(Resources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, errorProvider.BlinkStyle);
                Assert.IsEmpty(errorProvider.GetError(resultControl));

                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                TestHelper.AssertImagesAreEqual(Resources.PencilWarning.ToBitmap(), manualAssemblyWarningProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, manualAssemblyWarningProvider.BlinkStyle);
                Assert.IsEmpty(manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        public void SetError_ErrorMessageNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var resultControl = new TestAssemblyResultControl())
            {
                // Call
                TestDelegate test = () => resultControl.SetError(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("errorMessage", exception.ParamName);
            }
        }

        [Test]
        [TestCase("random error 123")]
        [TestCase("")]
        public void SetError_WithErrorMessage_SetsErrorMessageOnControl(string errorMessage)
        {
            // Setup
            using (var resultControl = new TestAssemblyResultControl())
            {
                // Call
                resultControl.SetError(errorMessage);

                // Assert
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.AreEqual(errorMessage, errorProvider.GetError(resultControl));
            }
        }

        [Test]
        public void SetManualAssemblyWarning_Always_SetsWarningMessageOnControl()
        {
            // Setup
            using (var resultControl = new TestAssemblyResultControl())
            {
                // Call
                resultControl.SetManualAssemblyWarning();

                // Assert
                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.AreEqual("Toetsoordeel is (deels) gebaseerd op handmatig overschreven toetsoordelen.", manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        [TestCase(true, 24)]
        [TestCase(false, 4)]
        public void GivenControlWithOrWithoutErrorMessage_WhenManualAssemblyWarningSet_ThenWarningSetWithExpectedPadding(bool hasError, int expectedPadding)
        {
            // Given
            using (var resultControl = new TestAssemblyResultControl())
            {
                if (hasError)
                {
                    resultControl.SetError("Error");
                }

                // Precondition
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.AreEqual(hasError, !string.IsNullOrEmpty(errorProvider.GetError(resultControl)));

                // When
                resultControl.SetManualAssemblyWarning();

                // Then
                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.AreEqual(expectedPadding, manualAssemblyWarningProvider.GetIconPadding(resultControl));
            }
        }

        [Test]
        public void GivenControlWithMessages_WhenMessagesCleared_ThenMessagesCleared()
        {
            // Given
            using (var resultControl = new TestAssemblyResultControl())
            {
                resultControl.SetError("Error");
                resultControl.SetManualAssemblyWarning();

                // When
                resultControl.ClearMessages();

                // Then
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.IsEmpty(errorProvider.GetError(resultControl));

                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.IsEmpty(manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        public void ClearAssemblyResult_Always_ClearsResultOnControl()
        {
            // Setup
            using (var resultControl = new TestAssemblyResultControl())
            {
                Control groupLabel = GetGroupPanel(resultControl).GetControlFromPosition(0, 0);
                groupLabel.Text = "abcd";
                groupLabel.BackColor = Color.Yellow;

                // Call
                resultControl.ClearAssemblyResult();

                // Assert
                Assert.IsEmpty(groupLabel.Text);
                Assert.AreEqual(Color.White, groupLabel.BackColor);
            }
        }

        private static ErrorProvider GetErrorProvider(AssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
        }

        private static ErrorProvider GetManualAssemblyWarningProvider(AssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "manualAssemblyWarningProvider");
        }

        private static TableLayoutPanel GetGroupPanel(AssemblyResultControl resultControl)
        {
            return (TableLayoutPanel) resultControl.Controls["GroupPanel"];
        }

        private class TestAssemblyResultControl : AssemblyResultControl {}
    }
}