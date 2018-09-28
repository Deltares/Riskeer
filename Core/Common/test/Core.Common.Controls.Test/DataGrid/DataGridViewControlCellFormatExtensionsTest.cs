// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewControlCellFormatExtensionsTest
    {
        private static IEnumerable<TestCaseData> CellFormattingStates
        {
            get
            {
                yield return new TestCaseData(true, "", CellStyle.Disabled);
                yield return new TestCaseData(false, "Error", CellStyle.Enabled);
            }
        }

        [Test]
        public void FormatCellWithColumnStateDefinition_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((DataGridViewControl) null).FormatCellWithColumnStateDefinition(0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(CellFormattingStates))]
        public void FormatCellWithColumnStateDefinition_WithArguments_FormatsCell(
            bool isReadOnly, string errorText, CellStyle cellStyle)
        {
            // Setup
            var definition = new DataGridViewColumnStateDefinition
            {
                ReadOnly = isReadOnly,
                ErrorText = errorText,
                Style = cellStyle
            };

            var mocks = new MockRepository();
            var row = mocks.Stub<IHasColumnStateDefinitions>();
            row.Stub(r => r.ColumnStateDefinitions).Return(new Dictionary<int, DataGridViewColumnStateDefinition>
            {
                {
                    0, definition
                }
            });
            mocks.ReplayAll();

            using (var form = new Form())
            using (var dataGridViewControl = new DataGridViewControl())
            {
                form.Controls.Add(dataGridViewControl);
                form.Show();

                dataGridViewControl.AddTextBoxColumn(null, "Test");
                dataGridViewControl.SetDataSource(new[]
                {
                    row
                });

                // Call
                dataGridViewControl.FormatCellWithColumnStateDefinition(0, 0);

                // Assert
                DataGridViewCell cell = dataGridViewControl.Rows[0].Cells[0];
                Assert.AreEqual(isReadOnly, cell.ReadOnly);
                Assert.AreEqual(errorText, cell.ErrorText);
                Assert.AreEqual(cellStyle.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(cellStyle.TextColor, cell.Style.ForeColor);
            }
        }
    }
}