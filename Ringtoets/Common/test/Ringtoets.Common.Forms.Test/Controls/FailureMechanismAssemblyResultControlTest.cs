// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Test.Controls
{
    [TestFixture]
    public class FailureMechanismAssemblyResultControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var resultControl = new FailureMechanismAssemblyResultControl();

            // Assert
            Assert.AreEqual(1, resultControl.Controls.Count);
            Assert.IsInstanceOf<UserControl>(resultControl);
            Assert.IsTrue(resultControl.AutoSize);

            var groupPanel = TypeUtils.GetField<TableLayoutPanel>(resultControl, "GroupPanel");
            Assert.AreEqual(2, groupPanel.ColumnCount);
            Assert.AreEqual(1, groupPanel.RowCount);

            var description = (Label) groupPanel.GetControlFromPosition(0, 0);
            Assert.IsTrue(description.AutoSize);
            Assert.AreEqual(DockStyle.Fill, description.Dock);
            Assert.AreEqual(ContentAlignment.MiddleLeft, description.TextAlign);
            Assert.AreEqual("Assemblageresultaat voor dit toetsspoor:", description.Text);

            var groupLabel = (BoxedLabel) groupPanel.GetControlFromPosition(1, 0);
            Assert.IsTrue(groupLabel.AutoSize);
            Assert.AreEqual(DockStyle.Fill, groupLabel.Dock);
            Assert.AreEqual(new Padding(5, 0, 5, 0), groupLabel.Padding);

            ErrorProvider errorProvider = GetErrorProvider(resultControl);
            TestHelper.AssertImagesAreEqual(Resources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
        }

        [Test]
        public void SetError_ErrorNull_ThrowsArgumentNullException()
        {
            // Setup
            var resultControl = new FailureMechanismAssemblyResultControl();

            // Call
            TestDelegate test = () => resultControl.SetError(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("error", exception.ParamName);
        }

        [Test]
        public void SetError_WithError_SetsErrorOnControl()
        {
            // Setup
            const string error = "random error 123";
            var resultControl = new FailureMechanismAssemblyResultControl();

            // Call
            resultControl.SetError(error);

            // Assert
            ErrorProvider errorProvider = GetErrorProvider(resultControl);
            Assert.AreEqual(error, errorProvider.GetError(resultControl));
        }

        [Test]
        public void ClearError_ClearsErrorOnControl()
        {
            // Setup
            var resultControl = new FailureMechanismAssemblyResultControl();

            // Call
            resultControl.ClearError();

            // Assert
            ErrorProvider errorProvider = GetErrorProvider(resultControl);
            Assert.AreEqual(string.Empty, errorProvider.GetError(resultControl));
        }

        private static ErrorProvider GetErrorProvider(FailureMechanismAssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "ErrorProvider");
        }
    }
}