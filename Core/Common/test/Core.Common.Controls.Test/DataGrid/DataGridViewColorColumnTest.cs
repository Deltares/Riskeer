// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewColorColumnTest
    {
        [Test]
        public void DefaultConstructor_CreatesColumnInstance()
        {
            // Call
            using (var column = new DataGridViewColorColumn())
            {
                // Assert
                Assert.IsInstanceOf<DataGridViewColumn>(column);
            }
        }

        [Test]
        public void CellTemplate_WithColorCell_CellTemplateSet()
        {
            // Setup
            using (var column = new DataGridViewColorColumn())
            {
                var dataGridViewCell = new DataGridViewColorCell();

                // Call
                column.CellTemplate = dataGridViewCell;

                // Assert
                Assert.AreSame(dataGridViewCell, column.CellTemplate);
            }
        }

        [Test]
        public void CellTemplate_WithOtherCell_ThrowsArgumentException()
        {
            // Setup
            using (var column = new DataGridViewColorColumn())
            using (var dataGridViewCell = new DataGridViewTextBoxCell())
            {
                // Call
                TestDelegate test = () => column.CellTemplate = dataGridViewCell;

                // Assert
                var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                    test,
                    $"Given template must be of type {typeof(DataGridViewColorCell)}");
                Assert.AreEqual("value", exception.ParamName);
            }
        }
    }
}