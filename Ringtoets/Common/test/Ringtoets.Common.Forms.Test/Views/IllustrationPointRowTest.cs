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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
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
        public void Constructor_IllustrationPointControlItemNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPointControlItem", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var source = new TestTopLevelIllustrationPoint();
            const string windDirectionName = "Name";
            const string closingSituation = "Closing situation";

            const int nrOfDecimals = 10;
            var beta = new RoundedDouble(nrOfDecimals, 123.789);

            var controlItem = new IllustrationPointControlItem(source,
                                                               windDirectionName,
                                                               closingSituation,
                                                               Enumerable.Empty<Stochast>(),
                                                               beta);

            // Call
            var row = new IllustrationPointRow(controlItem);

            // Assert
            TestHelper.AssertTypeConverter<IllustrationPointRow, NoProbabilityValueDoubleConverter>(
                nameof(IllustrationPointRow.Probability));

            TestHelper.AssertTypeConverter<IllustrationPointRow, NoValueRoundedDoubleConverter>(
                nameof(IllustrationPointRow.Reliability));

            double expectedProbability = StatisticsConverter.ReliabilityToProbability(controlItem.Beta);

            Assert.AreSame(controlItem, row.IllustrationPointControlItem);
            Assert.AreEqual(controlItem.WindDirectionName, row.WindDirection);
            Assert.AreEqual(controlItem.ClosingSituation, row.ClosingSituation);
            Assert.AreEqual(expectedProbability, row.Probability);
            Assert.AreEqual(beta, row.Reliability);
            Assert.AreEqual(nrOfDecimals, row.Reliability.NumberOfDecimalPlaces);
        }
    }
}