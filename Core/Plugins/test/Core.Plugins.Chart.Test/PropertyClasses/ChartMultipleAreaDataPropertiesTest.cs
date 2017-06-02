﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Components.Charting.Data;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartMultipleAreaDataPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ChartMultipleAreaDataProperties();

            // Assert
            Assert.IsInstanceOf<ChartDataProperties<ChartMultipleAreaData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Vlakken", properties.Type);
        }
    }
}