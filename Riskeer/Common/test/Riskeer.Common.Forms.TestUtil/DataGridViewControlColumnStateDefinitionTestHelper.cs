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

using System.Collections.Generic;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert properties of a <see cref="DataGridViewColumnStateDefinition"/> of 
    /// a cell in the <see cref="DataGridViewControl"/>.
    /// </summary>
    public static class DataGridViewControlColumnStateDefinitionTestHelper
    {
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

        /// <summary>
        /// Asserts that the column state definition is added and not null.
        /// </summary>
        /// <param name="columnStateDefinitions">The column state definitions to assert.</param>
        /// <param name="index">The index to assert for.</param>
        /// <exception cref="AssertionException">Thrown when the index is not added to the <paramref cref="columnStateDefinitions"/>
        /// or the column state definition is <c>null</c>.</exception>
        public static void AssertColumnStateDefinition(IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions, int index)
        {
            Assert.IsTrue(columnStateDefinitions.ContainsKey(index));
            Assert.IsNotNull(columnStateDefinitions[index]);
        }
    }
}