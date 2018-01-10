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

using System;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionsTableTest
    {
        private const int nameColumnIndex = 0;
        private const int lengthColumnIndex = 1;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new FailureMechanismSectionsTable())
            {
                // Assert
                DataGridViewColumn nameColumn = table.GetColumnFromIndex(nameColumnIndex);
                Assert.AreEqual("Naam", nameColumn.HeaderText);
                DataGridViewColumn lengthColumn = table.GetColumnFromIndex(lengthColumnIndex);
                Assert.AreEqual("Lengte [m]", lengthColumn.HeaderText);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(lengthColumnIndex + 1));

                CollectionAssert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new FailureMechanismSectionsTable())
            {
                var sections = new[]
                {
                    CreateFailureMechanismSection("a"),
                    CreateFailureMechanismSection("b"),
                    CreateFailureMechanismSection("c")
                };

                // Call
                table.SetData(sections);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new FailureMechanismSectionsTable())
            {
                var sections = new[]
                {
                    CreateFailureMechanismSection("a"),
                    CreateFailureMechanismSection("b"),
                    CreateFailureMechanismSection("c")
                };
                table.SetData(sections);

                // Call
                table.SetData(null);

                // Assert
                Assert.AreEqual(0, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNewDataAfterDataAlreadySet_ClearDataAndAddNewData()
        {
            // Setup
            using (var table = new FailureMechanismSectionsTable())
            {
                var sections = new[]
                {
                    CreateFailureMechanismSection("a"),
                    CreateFailureMechanismSection("b"),
                    CreateFailureMechanismSection("c")
                };
                table.SetData(new[]
                {
                    CreateFailureMechanismSection("d")
                });

                // Call
                table.SetData(sections);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new FailureMechanismSectionsTable())
            {
                var sections = new[]
                {
                    CreateFailureMechanismSection("a"),
                    CreateFailureMechanismSection("b"),
                    CreateFailureMechanismSection("c")
                };

                // Call
                table.SetData(sections);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    FailureMechanismSection section = sections[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;

                    Assert.AreEqual(section.Name,
                                    rowCells[nameColumnIndex].Value);
                    Assert.AreEqual(section.Length,
                                    rowCells[lengthColumnIndex].Value);
                }
            }
        }

        private static FailureMechanismSection CreateFailureMechanismSection(string name)
        {
            var random = new Random(39);
            return new FailureMechanismSection(name, new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            });
        }
    }
}