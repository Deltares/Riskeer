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

using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class EnhancedDataGridViewTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var dataGridView = new EnhancedDataGridView();

            // Assert
            Assert.IsInstanceOf<DataGridView>(dataGridView);
        }

        [Test]
        public void GivenDataGridViewWithDataSource_WhenShown_ThenCurrentCellChangedFiredOnlyOnce()
        {
            // Given
            var count = 0;
            var dataGridView = new EnhancedDataGridView
            {
                DataSource = new List<TestDataRow>
                {
                    new TestDataRow
                    {
                        A = 1.0,
                        B = 1.1
                    },
                    new TestDataRow
                    {
                        A = 2.0,
                        B = 2.1
                    }
                }
            };

            dataGridView.CurrentCellChanged += (s, e) => count++;

            // When
            WindowsFormsTestHelper.Show(dataGridView);

            // Then
            Assert.AreEqual(1, count);
        }

        private class TestDataRow
        {
            public double A { get; set; }
            public double B { get; set; }
        }
    }
}