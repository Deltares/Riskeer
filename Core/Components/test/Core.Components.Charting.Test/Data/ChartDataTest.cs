﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void ParameteredConstructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new TestChartData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string name = "Some name";

            // Call
            var data = new TestChartData(name);

            // Assert
            Assert.IsInstanceOf<Observable>(data);
            Assert.AreEqual(name, data.Name);
            Assert.IsTrue(data.IsVisible);
        }

        [Test]
        public void Name_SetName_ReturnsNewName()
        {
            // Setup
            const string name = "Some name";
            const string newName = "Something";
            var data = new TestChartData(name);

            // Precondition
            Assert.AreEqual(name, data.Name);

            // Call
            data.Name = newName;

            // Assert
            Assert.AreNotEqual(name, data.Name);
            Assert.AreEqual(newName, data.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void Name_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            const string name = "Some name";
            var data = new TestChartData(name);

            // Call
            TestDelegate call = () => data.Name = invalidName;

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }
    }

    public class TestChartData : ChartData
    {
        public TestChartData(string name) : base(name) {}
    }
}