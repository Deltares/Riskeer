// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Components.OxyPlot.Forms;
using Core.Components.Stack.Data;
using Core.Components.Stack.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
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

            Assert.AreEqual(new Size(400, 320), chartControl.AutoScrollMinSize);
        }

        [Test]
        public void IllustrationPointsControl_Always_SetsCorrectVerticalAxisTitle()
        {
            // Setup
            using (var form = new Form())
            using (var illustrationPointsControl = new IllustrationPointsControl())
            {
                form.Controls.Add(illustrationPointsControl);

                // Call
                form.Show();

                // Assert
                var stackChartControl = (StackChartControl) new ControlTester("stackChartControl").TheObject;
                Assert.AreEqual("Invloedscoëfficiënt^2", stackChartControl.VerticalAxisTitle);
            }
        }

        [Test]
        public void GivenIllustrationPointsChartControlWithoutData_WhenDataNotNull_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl();
            IllustrationPointControlItem[] illustrationPointControlItems = GetControlItems().ToArray();

            // When
            chartControl.Data = illustrationPointControlItems;

            // Then
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();
            string[] columns = chart.Data.Columns.ToArray();
            RowChartData[] rows = chart.Data.Rows.ToArray();

            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(3, rows.Length);

            Assert.AreEqual(illustrationPointControlItems[0].WindDirectionName, columns[0]);
            Assert.AreEqual(illustrationPointControlItems[1].WindDirectionName, columns[1]);
            Assert.AreEqual(illustrationPointControlItems[2].WindDirectionName, columns[2]);

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
        public void GivenIllustrationPointsChartControlWithData_WhenClosingSituationsAreDifferent_StackChartControlDisplaysClosingSituation()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl();
            var random = new Random(21);

            const string closingSituationRegular = "Regular";
            const string closingSituationOpen = "Open";

            var controlItems = new[]
            {
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SE",
                                                 closingSituationOpen,
                                                 Enumerable.Empty<Stochast>(),
                                                 random.NextRoundedDouble()),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "NE",
                                                 closingSituationRegular,
                                                 Enumerable.Empty<Stochast>(),
                                                 random.NextRoundedDouble())
            };

            // When
            chartControl.Data = controlItems;

            // Then
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();
            string[] columns = chart.Data.Columns.ToArray();
            RowChartData[] rows = chart.Data.Rows.ToArray();

            Assert.AreEqual(2, columns.Length);
            Assert.AreEqual(0, rows.Length);

            Assert.AreEqual($"{controlItems[0].WindDirectionName} ({closingSituationOpen})", columns[0]);
            Assert.AreEqual($"{controlItems[1].WindDirectionName} ({closingSituationRegular})", columns[1]);
        }

        [Test]
        public void GivenIllustrationPointsChartControlWithData_WhenDataSetToNull_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl
            {
                Data = GetControlItems()
            };

            // When
            chartControl.Data = null;

            // Then
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();
            CollectionAssert.IsEmpty(chart.Data.Columns);
            CollectionAssert.IsEmpty(chart.Data.Rows);
        }

        [Test]
        public void GivenIllustrationPointsChartControlWithData_WhenDataSetToOther_ThenStackChartControlUpdated()
        {
            // Given
            var chartControl = new IllustrationPointsChartControl
            {
                Data = GetControlItems()
            };

            // Precondition
            IStackChartControl chart = chartControl.Controls.OfType<IStackChartControl>().Single();

            Assert.AreEqual(3, chart.Data.Columns.Count());
            Assert.AreEqual(3, chart.Data.Rows.Count());

            // When
            chartControl.Data = new[]
            {
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Regular",
                                                 new[]
                                                 {
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.9),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.43589)
                                                 }, (RoundedDouble) 1.0),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "NE",
                                                 "Closing",
                                                 new[]
                                                 {
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.43589),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.9)
                                                 }, (RoundedDouble) 3.0)
            };

            // Then
            Assert.AreEqual(2, chart.Data.Columns.Count());

            RowChartData[] rows = chart.Data.Rows.ToArray();
            Assert.AreEqual(2, rows.Length);

            Assert.AreEqual("Stochast 3", rows[0].Name);
            Assert.AreEqual("Stochast 4", rows[1].Name);
        }

        private static IEnumerable<IllustrationPointControlItem> GetControlItems()
        {
            return new[]
            {
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Regular",
                                                 new[]
                                                 {
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.9),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.43589),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.01),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.01)
                                                 }, (RoundedDouble) 1.0),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "NE",
                                                 "Regular",
                                                 new[]
                                                 {
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.43589),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.9),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.02),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.02)
                                                 }, (RoundedDouble) 2.0),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SE",
                                                 "Regular",
                                                 new[]
                                                 {
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.43589),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.9),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.03),
                                                     new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.03)
                                                 }, (RoundedDouble) 3.0)
            };
        }
    }
}