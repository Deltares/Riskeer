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
        public void AddColumn_RowsAdded_ThrowsInvalidOperationException()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddRow("Row 1", new[]
            {
                1.1
            });

            // Call
            TestDelegate test = () => data.AddColumn("Column 2");

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Cannot add columns when rows already present.", exception.Message);
        }

        [Test]
        public void AddRow_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("column 1");

            // Call
            TestDelegate test = () => data.AddRow(null, new[]
            {
                1.0
            }, Color.White);

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
            data.AddColumn("column 1");
            data.AddColumn("column 2");

            // Call
            TestDelegate test = () => data.AddRow("test", new[]
            {
                2.1
            });

            // Assert
            const string message = "The number of value items must be the same as the number of columns.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
            Assert.AreEqual("values", exception.ParamName);
        }

        [Test]
        public void AddRow_ValidDataWithColor_AddRowToCollection()
        {
            // Setup
            const string name = "Row 1";
            var values = new[]
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
            var values = new[]
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
        public void AddRow_NoColumnsAdded_ThrowInvalidOperationException()
        {
            // Setup
            var data = new StackChartData();
            // Call
            TestDelegate test = () => data.AddRow("Row", new[]
            {
                1.1,
                2.2
            });

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(test);
            Assert.AreEqual("Cannot add rows before columns are added.", exception.Message);
        }

        [Test]
        public void Clear_Always_ClearsAllColumnsAndRows()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddColumn("Column 2");
            data.AddRow("Row 1", new[]
            {
                1.0,
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