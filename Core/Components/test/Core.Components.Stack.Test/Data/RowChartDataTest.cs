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

using System.Drawing;
using Core.Components.Stack.Data;
using NUnit.Framework;

namespace Core.Components.Stack.Test.Data
{
    [TestFixture]
    public class RowChartDataTest
    {
        [Test]
        public void Constructor_WithColor_ExpectedValues()
        {
            // Setup
            const string name = "Row 1";
            Color color = Color.Blue;
            var values = new[]
            {
                1.2,
                2.3
            };

            // Call
            var row = new RowChartData(name, color, values);

            // Assert
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(color, row.Color);
            CollectionAssert.AreEqual(values, row.Values);
        }

        [Test]
        public void Constructor_WithoutColor_ExpectedValues()
        {
            // Setup
            const string name = "Row 1";
            var values = new[]
            {
                1.2,
                2.3
            };

            // Call
            var row = new RowChartData(name, values);

            // Assert
            Assert.AreEqual(name, row.Name);            
            CollectionAssert.AreEqual(values, row.Values);
            Assert.IsNull(row.Color);
        }
    }
}