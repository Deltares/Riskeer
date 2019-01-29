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
using Riskeer.Integration.Forms.Controls;

namespace Riskeer.Integration.Forms.Test.Controls
{
    public class AssessmentSectionAssemblyCategoryGroupControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var resultControl = new AssessmentSectionAssemblyCategoryGroupControl())
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
            var result = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();
            using (var resultControl = new AssessmentSectionAssemblyCategoryGroupControl())
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

            using (var resultControl = new AssessmentSectionAssemblyCategoryGroupControl())
            {
                var result = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();
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

        private static void AssertGroupLabel(AssessmentSectionAssemblyCategoryGroup result, BorderedLabel groupLabel)
        {
            Assert.AreEqual(new EnumDisplayWrapper<AssessmentSectionAssemblyCategoryGroup>(result).DisplayName,
                            groupLabel.Text);
            Assert.AreEqual(AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(result),
                            groupLabel.BackColor);
        }

        private static BorderedLabel GetGroupLabel(AssessmentSectionAssemblyCategoryGroupControl resultControl)
        {
            return (BorderedLabel) ((TableLayoutPanel) resultControl.Controls["GroupPanel"]).GetControlFromPosition(0, 0);
        }
    }
}