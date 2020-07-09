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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class ColumnStateHelperTest
    {
        [Test]
        public void SetColumnState_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ColumnStateHelper.SetColumnState(null, new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void SetColumnState_ShouldDisableFalse_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            ColumnStateHelper.SetColumnState(
                columnStateDefinition,
                false);

            // Assert
            Assert.IsFalse(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Enabled, columnStateDefinition.Style);
        }

        [Test]
        public void SetColumnState_ShouldDisableTrue_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            ColumnStateHelper.SetColumnState(
                columnStateDefinition,
                true);

            // Assert
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Disabled, columnStateDefinition.Style);
        }

        [Test]
        public void EnableColumn_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ColumnStateHelper.EnableColumn(null, new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void EnableColumn_WithValidData_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();
            bool readOnly = new Random(39).NextBoolean();

            // Call
            ColumnStateHelper.EnableColumn(
                columnStateDefinition,
                readOnly);

            // Assert
            Assert.AreEqual(readOnly, columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Enabled, columnStateDefinition.Style);
        }

        [Test]
        public void DisableColumn_DataGridViewColumnStateDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ColumnStateHelper.DisableColumn(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("columnStateDefinition", exception.ParamName);
        }

        [Test]
        public void DisableColumn_WithValidData_UpdatesColumnState()
        {
            // Setup
            var columnStateDefinition = new DataGridViewColumnStateDefinition();

            // Call
            ColumnStateHelper.DisableColumn(columnStateDefinition);

            // Assert
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(CellStyle.Disabled, columnStateDefinition.Style);
        }
    }
}
