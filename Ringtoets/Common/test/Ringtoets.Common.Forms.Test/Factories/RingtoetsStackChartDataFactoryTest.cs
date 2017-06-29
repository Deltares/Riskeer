// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Linq;
using Core.Components.Stack.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RingtoetsStackChartDataFactoryTest
    {
        [Test]
        public void Create_Always_ReturnStackChartData()
        {
            // Call
            StackChartData stackChartData = RingtoetsStackChartDataFactory.Create();

            // Assert
            CollectionAssert.IsEmpty(stackChartData.Columns);
            CollectionAssert.IsEmpty(stackChartData.Rows);
        }

        [Test]
        public void CreateColumns_GeneralResultNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateColumns(null, new StackChartData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void CreateColumns_StackChartDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateColumns(new TestGeneralResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stackChartData", exception.ParamName);
        }

        [Test]
        public void CreateColumns_SameClosingSituations_ColumnsAddedToStackChartData()
        {
            // Setup
            var stackChartData = new StackChartData();
            var generalResult = new GeneralResult(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "General",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "General",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "General",
                        new TestIllustrationPoint())
                });

            // Call
            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Assert
            var columns = stackChartData.Columns.ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual("SSE", columns[0]);
            Assert.AreEqual("SSE", columns[1]);
            Assert.AreEqual("SSE", columns[2]);
        }

        [Test]
        public void CreateColumns_DifferentClosingSituations_ColumnsAddedToStackChartData()
        {
            var stackChartData = new StackChartData();
            var generalResult = new GeneralResult(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Closed",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Open",
                        new TestIllustrationPoint())
                });

            // Call
            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Assert
            var columns = stackChartData.Columns.ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual("SSE (Regular)", columns[0]);
            Assert.AreEqual("SSE (Closed)", columns[1]);
            Assert.AreEqual("SSE (Open)", columns[2]);
        }
    }
}