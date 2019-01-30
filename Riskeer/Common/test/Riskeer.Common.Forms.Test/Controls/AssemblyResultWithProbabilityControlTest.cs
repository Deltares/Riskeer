// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using NUnit.Framework;
using Riskeer.Common.Forms.Controls;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class AssemblyResultWithProbabilityControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            using (var form = new Form())
            // Call
            using (var resultControl = new TestAssemblyResultWithProbabilityControl())
            {
                form.Controls.Add(resultControl);
                form.Show();

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

                TableLayoutPanel probabilityPanel = GetProbabilityPanel(resultControl);
                Assert.AreEqual(1, probabilityPanel.ColumnCount);
                Assert.AreEqual(1, probabilityPanel.RowCount);

                var probabilityLabel = (BorderedLabel) probabilityPanel.GetControlFromPosition(0, 0);
                Assert.IsTrue(probabilityLabel.AutoSize);
                Assert.AreEqual(DockStyle.Left, probabilityLabel.Dock);
                Assert.AreEqual(new Padding(5, 0, 5, 0), probabilityLabel.Padding);
                Assert.AreEqual(Color.White, probabilityLabel.BackColor);
            }
        }

        [Test]
        public void ClearAssemblyResult_Always_ClearsResultOnControl()
        {
            // Setup
            using (var resultControl = new TestAssemblyResultWithProbabilityControl())
            {
                Control groupLabel = GetGroupPanel(resultControl).GetControlFromPosition(0, 0);
                Control probabilityLabel = GetProbabilityPanel(resultControl).GetControlFromPosition(0, 0);
                groupLabel.Text = "abcd";
                groupLabel.BackColor = Color.Yellow;
                probabilityLabel.Text = "1/245";

                // Call
                resultControl.ClearAssemblyResult();

                // Assert
                Assert.IsEmpty(groupLabel.Text);
                Assert.AreEqual(Color.White, groupLabel.BackColor);
                Assert.AreEqual("-", probabilityLabel.Text);
            }
        }

        private static TableLayoutPanel GetProbabilityPanel(AssemblyResultWithProbabilityControl resultControl)
        {
            return (TableLayoutPanel) resultControl.Controls["probabilityPanel"];
        }

        private static TableLayoutPanel GetGroupPanel(AssemblyResultWithProbabilityControl resultControl)
        {
            return (TableLayoutPanel) resultControl.Controls["GroupPanel"];
        }

        private class TestAssemblyResultWithProbabilityControl : AssemblyResultWithProbabilityControl {}
    }
}