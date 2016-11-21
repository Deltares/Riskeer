// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="System.Windows.Forms.DataGridViewCell"/>.
    /// </summary>
    public static class DataGridViewCellTestHelper
    {
        /// <summary>
        /// Asserts whether a <see cref="DataGridViewCell"/> is in a disabled state.
        /// </summary>
        /// <param name="dataGridViewCell">The cell that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The readonly property of the cell is <c>false</c>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.ForeColor"/> is not <see cref="KnownColor.GrayText"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.BackColor"/> is not <see cref="KnownColor.DarkGray"/>.</item>
        /// </list></exception>
        public static void AssertCellIsDisabled(DataGridViewCell dataGridViewCell)
        {
            Assert.AreEqual(true, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
        }

        /// <summary>
        /// Asserts whether a <see cref="DataGridViewCell"/> is in an enabled state.
        /// </summary>
        /// <param name="dataGridViewCell">The cell that needs to be asserted.</param>
        /// <param name="isReadOnly">Flag indicating whether the cell is readonly</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The readonly property of the cell does not match with <paramref name="isReadOnly"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.ForeColor"/> is not <see cref="KnownColor.ControlText"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.BackColor"/> is not <see cref="KnownColor.White"/>.</item>
        /// </list></exception>
        public static void AssertCellIsEnabled(DataGridViewCell dataGridViewCell, bool isReadOnly = false)
        {
            Assert.AreEqual(isReadOnly, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
        }
    }
}