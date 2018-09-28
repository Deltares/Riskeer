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
using Core.Components.Chart.Data;
using NUnit.Framework;

namespace Core.Components.Chart.TestUtil.Test
{
    [TestFixture]
    public class TestChartDataTest
    {
        [Test]
        public void DefaultConstructor_InstanceWithExpectedProperties()
        {
            // Call
            var chartData = new TestChartData();

            // Assert
            Assert.IsInstanceOf<ChartData>(chartData);
            Assert.AreEqual("test data", chartData.Name);
            Assert.IsFalse(chartData.HasData);
        }

        [Test]
        public void Constructor_WithName_InstanceWithExpectedProperties()
        {
            // Setup
            const string name = "some name";

            // Call
            var chartData = new TestChartData(name);

            // Assert
            Assert.IsInstanceOf<ChartData>(chartData);
            Assert.AreEqual(name, chartData.Name);
            Assert.IsFalse(chartData.HasData);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithoutName_InstanceWithExpectedProperties(string name)
        {
            // Call
            TestDelegate test = () => new TestChartData(name);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }
    }
}