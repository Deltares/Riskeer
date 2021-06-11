// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Core.Common.Controls.PresentationObjects;
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using Core.Gui.PresentationObjects.Chart;
using NUnit.Framework;

namespace Core.Gui.Test.PresentationObjects.Chart
{
    [TestFixture]
    public class ChartDataContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            ChartData data = new TestChartData();
            var collection = new ChartDataCollection("test");

            collection.Add(data);

            // Call
            var context = new ChartDataContext(data, collection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ChartData>>(context);
            Assert.AreSame(data, context.WrappedData);
            Assert.AreSame(collection, context.ParentChartData);
        }

        [Test]
        public void Constructor_ParentChartDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ChartDataContext(new TestChartData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("parentChartData", exception.ParamName);
        }
    }
}