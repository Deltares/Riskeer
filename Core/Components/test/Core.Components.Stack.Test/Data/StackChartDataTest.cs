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
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Components.Stack.Data;
using NUnit.Framework;

namespace Core.Components.Stack.Test.Data
{
    [TestFixture]
    public class StackChartDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var data = new StackChartData();

            // Assert
            Assert.IsInstanceOf<Observable>(data);
            CollectionAssert.IsEmpty(data.Columns);
            CollectionAssert.IsEmpty(data.Rows);
        }

        [Test]
        public void AddColumn_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new StackChartData();

            // Call
            TestDelegate test = () => data.AddColumn(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void AddColumn_WithName_AddColumnToCollection()
        {
            // Setup
            const string columnName = "Column 1";
            var data = new StackChartData();

            // Call            
            data.AddColumn(columnName);

            // Assert
            Assert.AreEqual(1, data.Columns.Count());
            string column = data.Columns.First();
            Assert.AreEqual(columnName, column);
        }

        [Test]
        public void AddColumnWithValues_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new List<double>
            {
                4.5
            });

            // Call
            TestDelegate test = () => data.AddColumnWithValues(null, new List<double>
            {
                1
            });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void AddColumnWithValues_ValuesNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new List<double>
            {
                4.5
            });

            // Call
            TestDelegate test = () => data.AddColumnWithValues("test", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("values", exception.ParamName);
        }

        [Test]
        public void AddColumnWithValues_NumberOfRowsDifferentThanValueItems_ThrowsArgumentException()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new List<double>
            {
                4.5
            });

            // Call
            TestDelegate test = () => data.AddColumnWithValues("test", new List<double>
            {
                2.1,
                3.4
            });

            // Assert
            const string message = "The number of value items must be the same as the number of rows.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void AddColumnWithValues_NoRowsAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var data = new StackChartData();

            // Call
            TestDelegate test = () => data.AddColumnWithValues("test", new List<double>());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Rows should be added before this method is called.", exception.Message);
        }

        [Test]
        public void AddColumnWithValues_ValidData_AddColumnAndValuesExistingRows()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new List<double>
            {
                1.0
            });

            // Call
            data.AddColumnWithValues("Column 2", new List<double>
            {
                2.0
            });

            // Assert
            Assert.AreEqual(2, data.Columns.Count());
            Assert.AreEqual("Column 2", data.Columns.Last());
            Assert.AreEqual(1, data.Rows.Count());
            CollectionAssert.AreEqual(new[]
            {
                1.0,
                2.0
            }, data.Rows.First().Values);
        }

        [Test]
        public void AddRow_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new StackChartData();

            // Call
            TestDelegate test = () => data.AddRow(null, new List<double>(), Color.White);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void AddRow_ValuesNull_ThrowsArgumentNullException()
        { 
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");

            // Call
            TestDelegate test = () => data.AddRow("test", null, Color.White);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("values", exception.ParamName);
        }

        [Test]
        public void AddRow_NumberOfColumnsDifferentThanValueItems_ThrowsArgumentException()
        {
            // Setup
            var data = new StackChartData();

            // Call
            TestDelegate test = () => data.AddRow("test", new List<double>
            {
                2.1
            });

            // Assert
            const string message = "The number of value items must be the same as the number of columns.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void AddRow_ValidDataWithColor_AddRowToCollection()
        {
            // Setup
            const string name = "Row 1";
            var values = new List<double>
            {
                1.2
            };
            Color color = Color.Yellow;

            var data = new StackChartData();
            data.AddColumn("Column 1");

            // Call
            data.AddRow(name, values, color);

            // Assert
            Assert.AreEqual(1, data.Rows.Count());
            RowChartData row = data.Rows.First();
            Assert.AreEqual(name, row.Name);
            CollectionAssert.AreEqual(values, row.Values);
            Assert.AreEqual(color, row.Color);
        }

        [Test]
        public void AddRow_ValidDataWithoutColor_AddRowToCollection()
        {
            // Setup
            const string name = "Row 1";
            var values = new List<double>
            {
                1.2
            };

            var data = new StackChartData();
            data.AddColumn("Column 1");

            // Call
            data.AddRow(name, values);

            // Assert
            Assert.AreEqual(1, data.Rows.Count());
            RowChartData row = data.Rows.First();
            Assert.AreEqual(name, row.Name);
            CollectionAssert.AreEqual(values, row.Values);
            Assert.IsNull(row.Color);
        }

        [Test]
        public void Clear_Always_ClearsAllColumnsAndRows()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new List<double>
            {
                1.0
            });
            
            data.AddColumnWithValues("Column 2", new List<double>
            {
                2.0
            });

            // Precondition
            Assert.AreEqual(2, data.Columns.Count());
            Assert.AreEqual(1, data.Rows.Count());

            // Call
            data.Clear();

            // Assert
            CollectionAssert.IsEmpty(data.Columns);
            CollectionAssert.IsEmpty(data.Rows);
        }
    }
}