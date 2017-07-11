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
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointRowTest
    {
        [Test]
        public void Constructor_IllustrationPointNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double beta = 123.789;

            var illustrationPoint = new TopLevelSubMechanismIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                "Regular",
                new TestSubMechanismIllustrationPoint(beta));

            // Call
            var row = new IllustrationPointRow(illustrationPoint);

            // Assert
            TestHelper.AssertTypeConverter<IllustrationPointRow, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointRow.Probability));

            TestHelper.AssertTypeConverter<IllustrationPointRow, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointRow.Reliability));

            double expectedProbability = StatisticsConverter.ReliabilityToProbability(illustrationPoint.SubMechanismIllustrationPoint.Beta);

            Assert.AreSame(illustrationPoint, row.IllustrationPoint);
            Assert.AreEqual(illustrationPoint.WindDirection.Name, row.WindDirection);
            Assert.AreEqual(illustrationPoint.ClosingSituation, row.ClosingSituation);
            Assert.AreEqual(expectedProbability, row.Probability);
            Assert.AreEqual(beta, row.Reliability);
        }
    }
}