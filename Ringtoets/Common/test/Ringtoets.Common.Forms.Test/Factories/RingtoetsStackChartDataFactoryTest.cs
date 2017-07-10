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
using Ringtoets.Common.Data.IllustrationPoints;
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
        public void CreateColumns_GeneralResultSubMechanismIllustrationPointNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateColumns(null, new StackChartData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalResultSubMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateColumns_StackChartDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateColumns(new TestGeneralResultSubMechanismIllustrationPoint(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stackChartData", exception.ParamName);
        }

        [Test]
        public void CreateColumns_SameClosingSituations_ColumnsAddedToStackChartData()
        {
            // Setup
            var stackChartData = new StackChartData();
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var generalResult = new GeneralResultSubMechanismIllustrationPoint(
                windDirection,
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, "Regular",
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, "Regular",
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, "Regular",
                        new TestSubMechanismIllustrationPoint())
                });

            // Call
            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Assert
            string[] columns = stackChartData.Columns.ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(windDirection.Name, columns[0]);
            Assert.AreEqual(windDirection.Name, columns[1]);
            Assert.AreEqual(windDirection.Name, columns[2]);
        }

        [Test]
        public void CreateColumns_DifferentClosingSituations_ColumnsAddedToStackChartData()
        {
            const string closingSituationRegular = "Regular";
            const string closingSituationClosed = "Closed";
            const string closingSituationOpen = "Open";

            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            var stackChartData = new StackChartData();
            var generalResult = new GeneralResultSubMechanismIllustrationPoint(
                windDirection,
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, closingSituationRegular,
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, closingSituationClosed,
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        windDirection, closingSituationOpen,
                        new TestSubMechanismIllustrationPoint())
                });

            // Call
            RingtoetsStackChartDataFactory.CreateColumns(generalResult, stackChartData);

            // Assert
            string[] columns = stackChartData.Columns.ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual($"{windDirection.Name} ({closingSituationRegular})", columns[0]);
            Assert.AreEqual($"{windDirection.Name} ({closingSituationClosed})", columns[1]);
            Assert.AreEqual($"{windDirection.Name} ({closingSituationOpen})", columns[2]);
        }

        [Test]
        public void CreateRows_GeneralResultSubMechanismIllustrationPointNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateRows(null, new StackChartData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalResultSubMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateRows_StackChartDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsStackChartDataFactory.CreateRows(new TestGeneralResultSubMechanismIllustrationPoint(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stackChartData", exception.ParamName);
        }

        [Test]
        public void CreateRows_WithAllData_RowsAddedToStackChartData()
        {
            var stackChartData = new StackChartData();
            var generalResult = new GeneralResultSubMechanismIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
            Enumerable.Empty<Stochast>(),
               new[]
               {
                    new TopLevelSubMechanismIllustrationPoint(
                       WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Punt 1",
                                              new[]
                                              {
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.9),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.43589),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.01),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.01),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 5", -0.099)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Punt 2",
                                              new[]
                                              {
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.43589),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.9),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.02),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.02),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 5", -0.9)
                                              },
                                              Enumerable.Empty<IllustrationPointResult>(), 1)),
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Punt 3",
                                              new[]
                                              {
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 1", -0.43589),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 2", -0.9),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 3", -0.03),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 4", -0.03),
                                                  new TestSubMechanismIllustrationPointStochast("Stochast 5", -0.099)
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