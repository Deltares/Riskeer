// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class IllustrationPointControlItemComparerTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var comparer = new IllustrationPointControlItemComparer();

            // Assert
            Assert.IsInstanceOf<IComparer<IllustrationPointControlItem>>(comparer);
            Assert.IsInstanceOf<IComparer>(comparer);
        }

        [Test]
        public void Compare_EqualControlItems_Returns0()
        {
            // Setup
            var source = new TestTopLevelIllustrationPoint();

            const string windDirectionName = "Name of the wind";
            const string closingSituation = "Situation of closing";
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            var beta = (RoundedDouble) 3.14;

            var itemOne = new IllustrationPointControlItem(source,
                                                           windDirectionName,
                                                           closingSituation,
                                                           stochasts,
                                                           beta);

            var itemTwo = new IllustrationPointControlItem(source,
                                                           windDirectionName,
                                                           closingSituation,
                                                           stochasts,
                                                           beta);

            var comparer = new IllustrationPointControlItemComparer();

            // Call
            int result = comparer.Compare(itemOne, itemTwo);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnequalItems))]
        public void Compare_UnequalControlItems_ReturnsDifferentThan0(IllustrationPointControlItem itemOne,
                                                                      IllustrationPointControlItem itemTwo)
        {
            // Setup
            var comparer = new IllustrationPointControlItemComparer();

            // Call
            int result = comparer.Compare(itemOne, itemTwo);

            // Assert
            Assert.AreNotEqual(0, result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnsupportedCases))]
        public void Compare_UnsupportedObjects_ThrowsArgumentException(object itemOne,
                                                                       object itemTwo)
        {
            // Setup
            var comparer = new IllustrationPointControlItemComparer();

            // Call
            TestDelegate call = () => comparer.Compare(itemOne, itemTwo);

            // Assert
            string expectedMessage = "Arguments must be of type 'IllustrationPointControlItem', but found:" +
                                     $"x: {itemOne.GetType()} and y: {itemTwo.GetType()}";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        private static IEnumerable<TestCaseData> GetUnsupportedCases()
        {
            yield return new TestCaseData(new object(),
                                          new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(), "SSE", "Regular", Enumerable.Empty<Stochast>(), RoundedDouble.NaN))
                .SetName("x");
            yield return new TestCaseData(new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(), "SSE", "Regular", Enumerable.Empty<Stochast>(), RoundedDouble.NaN),
                                          new object())
                .SetName("y");
            yield return new TestCaseData(new object(),
                                          new object())
                .SetName("x and y");
        }

        private static IEnumerable<TestCaseData> GetUnequalItems()
        {
            var source = new TestTopLevelIllustrationPoint();
            const string windDirectionName = "Name of the wind";
            const string closingSituation = "Situation of closing";
            IEnumerable<Stochast> stochasts = new Stochast[0];
            var beta = (RoundedDouble) 3.14;

            yield return new TestCaseData(new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta),
                                          new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(), windDirectionName, closingSituation, stochasts, beta))
                .SetName("Source reference");
            yield return new TestCaseData(new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta),
                                          new IllustrationPointControlItem(source, "A different name", closingSituation, stochasts, beta))
                .SetName("Wind direction name");
            yield return new TestCaseData(new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta),
                                          new IllustrationPointControlItem(source, windDirectionName, "A different closing situation", stochasts, beta))
                .SetName("Closing situation");
            yield return new TestCaseData(new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta),
                                          new IllustrationPointControlItem(source, windDirectionName, closingSituation, new Stochast[0], beta))
                .SetName("Stochasts reference");
            yield return new TestCaseData(new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta),
                                          new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, (RoundedDouble) 30))
                .SetName("Beta");
        }
    }
}