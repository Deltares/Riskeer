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
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
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
                        new TestWindDirection(), "Regular",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new TestIllustrationPoint()),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new TestIllustrationPoint())
                });

            // Call
            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Assert
            string[] columns = stackChartData.Columns.ToArray();
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
            string[] columns = stackChartData.Columns.ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual("SSE (Regular)", columns[0]);
            Assert.AreEqual("SSE (Closed)", columns[1]);
            Assert.AreEqual("SSE (Open)", columns[2]);
        }

        [Test]
        public void CreateRows_GeneralResultNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateRows(null, new StackChartData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void CreateRows_StackChartDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateRows(new TestGeneralResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stackChartData", exception.ParamName);
        }

        [Test]
        public void CreateRows_WithAllData_RowsAddedToStackChartData()
        {
            var stackChartData = new StackChartData();
            var generalResult = new GeneralResult(
               new TestWindDirection(),
               Enumerable.Empty<Stochast>(),
               new[]
               {
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 1",
                                              new[]
                                              {
                                                  new RealizedStochast("Stochast 1", 1, -0.9, 3),
                                                  new RealizedStochast("Stochast 2", 1, -0.43589, 3),
                                                  new RealizedStochast("Stochast 3", 1, -0.01, 3),
                                                  new RealizedStochast("Stochast 4", 1, -0.01, 3),
                                                  new RealizedStochast("Stochast 5", 1, -0.099, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 2",
                                              new[]
                                              {
                                                  new RealizedStochast("Stochast 1", 1, -0.43589, 3),
                                                  new RealizedStochast("Stochast 2", 1, -0.9, 3),
                                                  new RealizedStochast("Stochast 3", 1, -0.02, 3),
                                                  new RealizedStochast("Stochast 4", 1, -0.02, 3),
                                                  new RealizedStochast("Stochast 5", 1, -0.9, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 3",
                                              new[]
                                              {
                                                  new RealizedStochast("Stochast 1", 1, -0.43589, 3),
                                                  new RealizedStochast("Stochast 2", 1, -0.9, 3),
                                                  new RealizedStochast("Stochast 3", 1, -0.03, 3),
                                                  new RealizedStochast("Stochast 4", 1, -0.03, 3),
                                                  new RealizedStochast("Stochast 5", 1, -0.099, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1))
               });

            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Call
            RingtoetsStackChartDataFactory.CreateRows(generalResult, stackChartData);

            // Assert
            RowChartData[] rows = stackChartData.Rows.ToArray();

            Assert.AreEqual(4, rows.Length);

            Assert.AreEqual("Stochast 1", rows[0].Name);
            CollectionAssert.AreEqual(new[]
            {
                0.81,
                0.19,
                0.19
            }, rows[0].Values, new DoubleWithToleranceComparer(1e-6));
            Assert.IsNull(rows[0].Color);
            Assert.AreEqual("Stochast 2", rows[1].Name);
            CollectionAssert.AreEqual(new[]
            {
                0.19,
                0.81,
                0.81
            }, rows[1].Values, new DoubleWithToleranceComparer(1e-6));
            Assert.IsNull(rows[1].Color);
            Assert.AreEqual("Stochast 5", rows[2].Name);
            CollectionAssert.AreEqual(new[]
            {
                0.009801,
                0.81,
                0.009801
            }, rows[2].Values, new DoubleWithToleranceComparer(1e-6));
            Assert.IsNull(rows[2].Color);
            Assert.AreEqual("Overig", rows[3].Name);
            CollectionAssert.AreEqual(new[]
            {
                0.0002,
                0.0008,
                0.0018
            }, rows[3].Values, new DoubleWithToleranceComparer(1e-6));
            Assert.AreEqual(Color.Gray, rows[3].Color);
        }
    }
}