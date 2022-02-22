﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyGroupsTableTest
    {
        private const int groupColumnIndex = 0;
        private const int colorColumnIndex = 1;
        private const int lowerBoundaryColumnIndex = 2;
        private const int upperBoundaryColumnIndex = 3;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new AssemblyGroupsTable<TestAssemblyGroup>())
            {
                // Assert
                Assert.IsInstanceOf<DataGridViewControl>(table);

                DataGridViewColumn groupColumn = table.GetColumnFromIndex(groupColumnIndex);
                Assert.AreEqual("Naam", groupColumn.HeaderText);
                Assert.IsTrue(groupColumn.ReadOnly);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(groupColumn);

                DataGridViewColumn colorColumn = table.GetColumnFromIndex(colorColumnIndex);
                Assert.AreEqual("Kleur", colorColumn.HeaderText);
                Assert.IsTrue(colorColumn.ReadOnly);
                Assert.IsInstanceOf<DataGridViewColorColumn>(colorColumn);

                DataGridViewColumn lowerBoundaryColumn = table.GetColumnFromIndex(lowerBoundaryColumnIndex);
                Assert.AreEqual("Ondergrens [1/jaar]", lowerBoundaryColumn.HeaderText);
                Assert.IsTrue(lowerBoundaryColumn.ReadOnly);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(lowerBoundaryColumn);

                DataGridViewColumn upperBoundaryColumn = table.GetColumnFromIndex(upperBoundaryColumnIndex);
                Assert.AreEqual("Bovengrens [1/jaar]", upperBoundaryColumn.HeaderText);
                Assert.IsTrue(upperBoundaryColumn.ReadOnly);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(upperBoundaryColumn);

                Assert.Throws<ArgumentOutOfRangeException>(() => table.GetColumnFromIndex(upperBoundaryColumnIndex + 1));

                CollectionAssert.IsEmpty(table.Rows);
            }
        }

        [Test]
        public void SetData_NoDataAlreadySet_SetNewData()
        {
            // Setup
            using (var table = new AssemblyGroupsTable<TestAssemblyGroup>())
            {
                Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup>[] groups =
                {
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup()
                };

                // Call
                table.SetData(groups);

                // Assert
                Assert.AreEqual(groups.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new AssemblyGroupsTable<TestAssemblyGroup>())
            {
                Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup>[] groups =
                {
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup()
                };
                table.SetData(groups);

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
            using (var table = new AssemblyGroupsTable<TestAssemblyGroup>())
            {
                table.SetData(new[]
                {
                    CreateAssessmentSectionAssemblyGroup()
                });

                Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup>[] newGroups =
                {
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup()
                };

                // Call
                table.SetData(newGroups);

                // Assert
                Assert.AreEqual(newGroups.Length, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new AssemblyGroupsTable<TestAssemblyGroup>())
            {
                Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup>[] groups =
                {
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup(),
                    CreateAssessmentSectionAssemblyGroup()
                };

                // Call
                table.SetData(groups);

                // Assert
                Assert.AreEqual(groups.Length, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup> group = groups[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;

                    Assert.AreEqual(group.Item3, rowCells[groupColumnIndex].Value);
                    Assert.AreEqual(group.Item2, rowCells[colorColumnIndex].Value);
                    Assert.AreEqual(group.Item1.LowerBoundary, rowCells[lowerBoundaryColumnIndex].Value);
                    Assert.AreEqual(group.Item1.UpperBoundary, rowCells[upperBoundaryColumnIndex].Value);
                }
            }
        }

        private static Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup> CreateAssessmentSectionAssemblyGroup()
        {
            var random = new Random(39);
            return new Tuple<AssemblyGroupBoundaries, Color, TestAssemblyGroup>(new TestAssemblyGroupBoundaries(random.NextDouble(), random.NextDouble()),
                                                                                Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                                                                                random.NextEnumValue<TestAssemblyGroup>());
        }

        private class TestAssemblyGroupBoundaries : AssemblyGroupBoundaries
        {
            public TestAssemblyGroupBoundaries(double lowerBoundary, double upperBoundary)
                : base(lowerBoundary, upperBoundary) {}
        }

        private enum TestAssemblyGroup
        {
            I = 1
        }
    }
}