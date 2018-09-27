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
using Core.Common.Controls.PresentationObjects;
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using Core.Plugins.Chart.PresentationObjects;
using NUnit.Framework;

namespace Core.Plugins.Chart.Test.PresentationObjects
{
    [TestFixture]
    public class ChartDataContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
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
        public void ParameteredConstructor_ParentChartDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ChartDataContext(new TestChartData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parentChartData", exception.ParamName);
        }
    }
}