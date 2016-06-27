// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Components.Charting.Data;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class ChartDataFactoryTest
    {
        [Test]
        public void CreateEmptyLineData_Always_ReturnEmptyChartLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            ChartLineData chartData = ChartDataFactory.CreateEmptyLineData(name);

            // Assert
            Assert.AreEqual(name, chartData.Name);
            Assert.IsEmpty(chartData.Points);
        }

        [Test]
        public void CreateEmptyPointData_Always_ReturnEmptyChartPointDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            ChartPointData chartData = ChartDataFactory.CreateEmptyPointData(name);

            // Assert
            Assert.AreEqual(name, chartData.Name);
            Assert.IsEmpty(chartData.Points);
        }
    }
}