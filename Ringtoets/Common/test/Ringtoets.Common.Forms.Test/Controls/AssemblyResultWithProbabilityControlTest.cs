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
using Core.Common.Controls;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Test.Controls
{
    [TestFixture]
    public class AssemblyResultWithProbabilityControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var resultControl = new TestAssemblyResultWithProbabilityControl();

            // Assert
            Assert.AreEqual(2, resultControl.Controls.Count);
            Assert.IsInstanceOf<AssemblyResultControl>(resultControl);
            Assert.IsTrue(resultControl.AutoSize);
            Assert.AreEqual(DockStyle.Left, resultControl.Dock);

            TableLayoutPanel groupPanel = GetGroupPanel(resultControl);
            Assert.AreEqual(1, groupPanel.ColumnCount);
            Assert.AreEqual(1, groupPanel.RowCount);

            var groupLabel = (BorderedLabel) groupPanel.GetControlFromPosition(0, 0);
            Assert.IsTrue(groupLabel.AutoSize);
            Assert.AreEqual(DockStyle.Fill, groupLabel.Dock);
            Assert.AreEqual(new Padding(5, 0, 5, 0), groupLabel.Padding);

            var errorProvider = TypeUtils.GetField<ErrorProvider>(resultControl, "ErrorProvider");
            TestHelper.AssertImagesAreEqual(Resources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());

            TableLayoutPanel probabilityPanel = GetProbabilityPanel(resultControl);
            Assert.AreEqual(1, probabilityPanel.ColumnCount);
            Assert.AreEqual(1, probabilityPanel.RowCount);

            var probabilityLabel = (BorderedLabel) probabilityPanel.GetControlFromPosition(0, 0);
            Assert.IsTrue(probabilityLabel.AutoSize);
            Assert.AreEqual(DockStyle.Left, probabilityLabel.Dock);
            Assert.AreEqual(new Padding(5, 0, 5, 0), probabilityLabel.Padding);
            Assert.AreEqual(Color.White, probabilityLabel.BackColor);
        }

        [Test]
        public void SetError_ErrorNull_ThrowsArgumentNullException()
        {
            // Setup
            var resultControl = new TestAssemblyResultWithProbabilityControl();

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
            var resultControl = new TestAssemblyResultWithProbabilityControl();

            // Call
            resultControl.SetError(error);

            // Assert
            var errorProvider = TypeUtils.GetField<ErrorProvider>(resultControl, "ErrorProvider");
            Assert.AreEqual(error, errorProvider.GetError(resultControl));

            Control groupLabel = GetGroupPanel(resultControl).GetControlFromPosition(0, 0);
            Control probabilityLabel = GetProbabilityPanel(resultControl).GetControlFromPosition(0, 0);
            Assert.IsEmpty(groupLabel.Text);
            Assert.AreEqual(Color.White, groupLabel.BackColor);
            Assert.AreEqual("-", probabilityLabel.Text);
        }

        private static TableLayoutPanel GetProbabilityPanel(AssemblyResultWithProbabilityControl resultControl)
        {
            return TypeUtils.GetField<TableLayoutPanel>(resultControl, "probabilityPanel");
        }

        private static TableLayoutPanel GetGroupPanel(AssemblyResultWithProbabilityControl resultControl)
        {
            return TypeUtils.GetField<TableLayoutPanel>(resultControl, "GroupPanel");
        }

        private class TestAssemblyResultWithProbabilityControl : AssemblyResultWithProbabilityControl {}
    }
}