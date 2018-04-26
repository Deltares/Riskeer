// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;

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
            TestDelegate call = () => ((DataGridViewControl)null).FormatCellWithColumnStateDefinition<TestRow>(0, 0);

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
            var random = new Random(21);
            var row = new TestRow(random.NextRoundedDouble());
            DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[0];
            definition.ReadOnly = isReadOnly;
            definition.ErrorText = errorText;
            definition.Style = cellStyle;

            using (var form = new Form())
            using (var dataGridViewControl = new DataGridViewControl())
            {
                form.Controls.Add(dataGridViewControl);
                form.Show();

                dataGridViewControl.AddTextBoxColumn(nameof(TestRow.TestRoundedDouble), "Test");
                dataGridViewControl.SetDataSource(new[]
                {
                    row
                });

                // Call
                dataGridViewControl.FormatCellWithColumnStateDefinition<TestRow>(0, 0);

                // Assert
                DataGridViewCell cell = dataGridViewControl.Rows[0].Cells[0];
                Assert.AreEqual(isReadOnly, cell.ReadOnly);
                Assert.AreEqual(errorText, cell.ErrorText);
                Assert.AreEqual(cellStyle.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(cellStyle.TextColor, cell.Style.ForeColor);
            }
        }

        private class TestRow : IHasColumnStateDefinitions
        {
            public TestRow(RoundedDouble testRoundedDouble)
            {
                TestRoundedDouble = testRoundedDouble;
                ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>
                {
                    {
                        0, new DataGridViewColumnStateDefinition()
                    }
                };
            }

            // Property needs to have a setter, otherwise some tests will fail
            public RoundedDouble TestRoundedDouble { get; set; }

            public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }
        }
    }
}