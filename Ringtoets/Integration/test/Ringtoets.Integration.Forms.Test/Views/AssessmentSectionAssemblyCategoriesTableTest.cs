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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionAssemblyCategoriesTableTest
    {
        private const int categoryGroupColumnIndex = 0;
        private const int colorColumnIndex = 1;
        private const int lowerBoundaryColumnIndex = 2;
        private const int upperBoundaryColumnIndex = 3;

        [Test]
        public void Constructor_InitializesWithColumns()
        {
            // Call
            using (var table = new AssessmentSectionAssemblyCategoriesTable())
            {
                // Assert
                DataGridViewColumn groupColumn = table.GetColumnFromIndex(categoryGroupColumnIndex);
                Assert.AreEqual("Categorie", groupColumn.HeaderText);
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
            using (var table = new AssessmentSectionAssemblyCategoriesTable())
            {
                var categories = new[]
                {
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory()
                };

                // Call
                table.SetData(categories);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_SetNullDataAfterDataAlreadySet_ClearsData()
        {
            // Setup
            using (var table = new AssessmentSectionAssemblyCategoriesTable())
            {
                var categories = new[]
                {
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory()
                };
                table.SetData(categories);

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
            using (var table = new AssessmentSectionAssemblyCategoriesTable())
            {
                var categories = new[]
                {
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory()
                };
                table.SetData(new[]
                {
                    CreateAssessmentSectionAssemblyCategory()
                });

                // Call
                table.SetData(categories);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
            }
        }

        [Test]
        public void SetData_WithData_ExpectedValuesInTable()
        {
            // Setup
            using (var table = new AssessmentSectionAssemblyCategoriesTable())
            {
                var categories = new[]
                {
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory(),
                    CreateAssessmentSectionAssemblyCategory()
                };

                // Call
                table.SetData(categories);

                // Assert
                Assert.AreEqual(3, table.Rows.Count);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    AssessmentSectionAssemblyCategory category = categories[i];
                    DataGridViewCellCollection rowCells = table.Rows[i].Cells;

                    Assert.AreEqual(new EnumDisplayWrapper<AssessmentSectionAssemblyCategoryGroup>(category.Group).DisplayName, rowCells[categoryGroupColumnIndex].Value);
                    Assert.AreEqual(AssemblyCategoryGroupHelper.GetAssessmentSectionAssemblyCategoryGroupColor(category.Group), rowCells[colorColumnIndex].Value);
                    Assert.AreEqual(category.LowerBoundary, rowCells[lowerBoundaryColumnIndex].Value);
                    Assert.AreEqual(category.UpperBoundary, rowCells[upperBoundaryColumnIndex].Value);
                }
            }
        }

        private static AssessmentSectionAssemblyCategory CreateAssessmentSectionAssemblyCategory()
        {
            var random = new Random(39);
            return new AssessmentSectionAssemblyCategory(random.NextDouble(),
                                                         random.NextDouble(),
                                                         random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
        }
    }
}