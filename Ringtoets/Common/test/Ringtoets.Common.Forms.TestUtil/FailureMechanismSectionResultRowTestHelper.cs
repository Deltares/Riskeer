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

using System.Collections.Generic;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert properties of a <see cref="FailureMechanismSectionResultRow{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test the colors belonging to various
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> CategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Iv, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIv, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIIv, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IVv, Color.FromArgb(204, 192, 218));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Vv, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIv, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIIv, Color.FromArgb(255, 255, 255));
            }
        }

        /// <summary>
        /// Asserts the state of the <paramref name="columnStateDefinition"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The <see cref="DataGridViewColumnStateDefinition"/>
        /// to assert.</param>
        /// <param name="isEnabled">Indicator whether the state should be enabled or not.</param>
        /// <param name="isReadOnly">Indicator whether the state should be read-only or not.</param>
        /// <exception cref="AssertionException">Thrown when the state of <paramref name="columnStateDefinition"/>
        /// is not equal to the given parameters.</exception>
        public static void AssertColumnState(DataGridViewColumnStateDefinition columnStateDefinition,
                                             bool isEnabled,
                                             bool isReadOnly = false)
        {
            if (isEnabled)
            {
                AssertColumnStateIsEnabled(columnStateDefinition, isReadOnly);
            }
            else
            {
                AssertColumnStateIsDisabled(columnStateDefinition);
            }
        }

        /// <summary>
        /// Asserts the state of the <paramref name="columnStateDefinition"/> with an expected
        /// background color.
        /// </summary>
        /// <param name="columnStateDefinition">The <see cref="DataGridViewColumnStateDefinition"/>
        /// to assert.</param>
        /// <param name="expectedBackgroundColor">The expected background color of the column style.</param>
        /// <exception cref="AssertionException">Thrown when the state of <paramref name="columnStateDefinition"/>
        /// is not the same as the expected state.</exception>
        public static void AssertColumnWithColorState(DataGridViewColumnStateDefinition columnStateDefinition,
                                                      Color expectedBackgroundColor)
        {
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), columnStateDefinition.Style.TextColor);
            Assert.AreEqual(expectedBackgroundColor, columnStateDefinition.Style.BackgroundColor);
        }

        /// <summary>
        /// Asserts that the state of the <paramref name="columnStateDefinition"/> is disabled.
        /// </summary>
        /// <param name="columnStateDefinition">The <see cref="DataGridViewColumnStateDefinition"/>
        /// to assert.</param>
        /// <exception cref="AssertionException">Thrown when the state of <paramref name="columnStateDefinition"/>
        /// is not the same as the expected disabled state.</exception>
        public static void AssertColumnStateIsDisabled(DataGridViewColumnStateDefinition columnStateDefinition)
        {
            Assert.AreEqual(CellStyle.Disabled, columnStateDefinition.Style);
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(string.Empty, columnStateDefinition.ErrorText);
        }

        /// <summary>
        /// Asserts that the state of the <paramref name="columnStateDefinition"/> is enabled.
        /// </summary>
        /// <param name="columnStateDefinition">The <see cref="DataGridViewColumnStateDefinition"/>
        /// to assert.</param>
        /// <param name="readOnly">Indicator whether the column state should be read-only or not.</param>
        /// <exception cref="AssertionException">Thrown when the state of <paramref name="columnStateDefinition"/>
        /// is not the same as the expected enabled state.</exception>
        public static void AssertColumnStateIsEnabled(DataGridViewColumnStateDefinition columnStateDefinition, bool readOnly = false)
        {
            Assert.AreEqual(CellStyle.Enabled, columnStateDefinition.Style);
            Assert.AreEqual(readOnly, columnStateDefinition.ReadOnly);
            Assert.AreEqual(string.Empty, columnStateDefinition.ErrorText);
        }
    }
}