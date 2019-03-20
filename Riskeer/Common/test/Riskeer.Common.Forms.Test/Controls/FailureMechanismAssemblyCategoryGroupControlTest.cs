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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Controls
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoryGroupControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var resultControl = new FailureMechanismAssemblyCategoryGroupControl())
            {
                // Assert
                Assert.AreEqual(1, resultControl.Controls.Count);
                Assert.IsInstanceOf<AssemblyResultControl>(resultControl);
            }
        }

        [Test]
        public void SetAssemblyResult_WithResult_SetsValues()
        {
            // Setup
            var random = new Random(39);
            var result = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
            using (var resultControl = new FailureMechanismAssemblyCategoryGroupControl())
            {
                // Call
                resultControl.SetAssemblyResult(result);

                // Assert
                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                AssertGroupLabel(result, groupLabel);
            }
        }

        [Test]
        public void ClearAssemblyResult_Always_ClearsResultOnControl()
        {
            // Setup
            var random = new Random(39);

            using (var resultControl = new FailureMechanismAssemblyCategoryGroupControl())
            {
                var result = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
                resultControl.SetAssemblyResult(result);

                // Precondition
                BorderedLabel groupLabel = GetGroupLabel(resultControl);
                AssertGroupLabel(result, groupLabel);

                // Call
                resultControl.ClearAssemblyResult();

                // Assert
                Assert.IsEmpty(groupLabel.Text);
                Assert.AreEqual(Color.White, groupLabel.BackColor);
            }
        }

        private static void AssertGroupLabel(FailureMechanismAssemblyCategoryGroup result, BorderedLabel groupLabel)
        {
            Assert.AreEqual(new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(result).DisplayName,
                            groupLabel.Text);
            Assert.AreEqual(AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(result),
                            groupLabel.BackColor);
        }

        private static BorderedLabel GetGroupLabel(FailureMechanismAssemblyCategoryGroupControl resultControl)
        {
            return (BorderedLabel) ((TableLayoutPanel) resultControl.Controls["GroupPanel"]).GetControlFromPosition(0, 0);
        }
    }
}