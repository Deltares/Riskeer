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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Components.Stack.Data;
using Core.Components.Stack.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointsChartControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var chartControl = new IllustrationPointsChartControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(chartControl);
            Assert.IsNull(chartControl.Data);
            Assert.AreEqual(1, chartControl.Controls.Count);
            Assert.IsInstanceOf<IStackChartControl>(chartControl.Controls[0]);
        }

        [Test]
        public void GivenStackChartControlWithoutData_WhenDataNotNull_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl();

            // When
            chartControl.Data = GetGerenalResult();

            // Then
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();
            string[] columns = chart.Data.Columns.ToArray();
            RowChartData[] rows = chart.Data.Rows.ToArray();

            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(3, rows.Length);

            Assert.AreEqual("SSE", columns[0]);
            Assert.AreEqual("SSE", columns[1]);
            Assert.AreEqual("SSE", columns[2]);

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
            Assert.AreEqual("Overig", rows[2].Name);
            CollectionAssert.AreEqual(new[]
            {
                0.0002,
                0.0008,
                0.0018
            }, rows[2].Values, new DoubleWithToleranceComparer(1e-6));
            Assert.AreEqual(Color.Gray, rows[2].Color);
        }

        [Test]
        public void GivenStackChartControlWithData_WhenDataSetToNull_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl
            {
                Data = GetGerenalResult()
            };

            // When
            chartControl.Data = null;

            // Then
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();
            CollectionAssert.IsEmpty(chart.Data.Columns);
            CollectionAssert.IsEmpty(chart.Data.Rows);
        }

        [Test]
        public void GivenStackChartControlWithData_WhenDataSetToOther_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl
            {
                Data = GetGerenalResult()
            };

            // Precondition
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();

            Assert.AreEqual(3, chart.Data.Columns.Count());
            Assert.AreEqual(3, chart.Data.Rows.Count());

            // When
            chartControl.Data = new GeneralResult(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 1",
                                              new[]
                                              {
                                                  new SubmechanismIllustrationPointStochast("Stochast 3", 1, -0.9, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 4", 1, -0.43589, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 2",
                                              new[]
                                              {
                                                  new SubmechanismIllustrationPointStochast("Stochast 3", 1, -0.43589, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 4", 1, -0.9, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1))
                });

            // Then
            Assert.AreEqual(2, chart.Data.Columns.Count());

            RowChartData[] rows = chart.Data.Rows.ToArray();
            Assert.AreEqual(2, rows.Length);            

            Assert.AreEqual("Stochast 3", rows[0].Name);
            Assert.AreEqual("Stochast 4", rows[1].Name);
        }

        private static GeneralResult GetGerenalResult()
        {
            return new GeneralResult(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 1",
                                              new[]
                                              {
                                                  new SubmechanismIllustrationPointStochast("Stochast 1", 1, -0.9, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 2", 1, -0.43589, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 3", 1, -0.01, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 4", 1, -0.01, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 2",
                                              new[]
                                              {
                                                  new SubmechanismIllustrationPointStochast("Stochast 1", 1, -0.43589, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 2", 1, -0.9, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 3", 1, -0.02, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 4", 1, -0.02, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new WindDirectionClosingSituationIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new IllustrationPoint("Punt 3",
                                              new[]
                                              {
                                                  new SubmechanismIllustrationPointStochast("Stochast 1", 1, -0.43589, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 2", 1, -0.9, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 3", 1, -0.03, 3),
                                                  new SubmechanismIllustrationPointStochast("Stochast 4", 1, -0.03, 3)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1))
                });
        }
    }
}