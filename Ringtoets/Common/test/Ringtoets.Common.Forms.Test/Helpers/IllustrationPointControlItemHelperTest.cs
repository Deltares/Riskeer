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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class IllustrationPointControlItemHelperTest
    {
        [Test]
        public void AreClosingSituationsSame_IllustrationPointControlItemsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointControlItemHelper.AreClosingSituationsSame(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("illustrationPointControlItems", exception.ParamName);
        }

        [Test]
        public void AreClosingSituationsSame_EmptyCollection_ReturnTrue()
        {
            // Call
            bool result = IllustrationPointControlItemHelper.AreClosingSituationsSame(Enumerable.Empty<IllustrationPointControlItem>());

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetDifferentIllustrationPointControlItemsWithSameClosingSituation))]
        public void AreClosingSituationsSame_CollectionContainsItemsWithTheSameClosingSituation_ReturnsTrue(IllustrationPointControlItem baselineItem,
                                                                                                            IllustrationPointControlItem itemToCompare)
        {
            // Setup
            var collection = new[]
            {
                baselineItem,
                itemToCompare
            };

            // Call
            bool result = IllustrationPointControlItemHelper.AreClosingSituationsSame(collection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AreClosingSituationsSame_CollectionContainsItemsWithDifferentClosingSituation_ReturnsFalse()
        {
            // Setup
            var source = new TestTopLevelIllustrationPoint();
            const string windDirectionName = "name of the wind";
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            RoundedDouble beta = RoundedDouble.NaN;
            var baseline = new IllustrationPointControlItem(source, windDirectionName, "ClosingSituation", stochasts, beta);

            var itemToCompare = new IllustrationPointControlItem(source, windDirectionName, "Different Closing Situation", stochasts, beta);

            // Call
            bool result = IllustrationPointControlItemHelper.AreClosingSituationsSame(new[]
            {
                baseline,
                itemToCompare
            });

            // Assert
            Assert.IsFalse(result);
        }

        private static IEnumerable<TestCaseData> GetDifferentIllustrationPointControlItemsWithSameClosingSituation()
        {
            const string closingSituation = "ClosingSituation";
            var source = new TestTopLevelIllustrationPoint();
            const string windDirectionName = "name of the wind";
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            RoundedDouble beta = RoundedDouble.NaN;
            var baseline = new IllustrationPointControlItem(source, windDirectionName, closingSituation, stochasts, beta);

            yield return new TestCaseData(baseline, new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                                                     windDirectionName,
                                                                                     closingSituation,
                                                                                     stochasts,
                                                                                     beta))
                .SetName("Different Source");
            yield return new TestCaseData(baseline, new IllustrationPointControlItem(source,
                                                                                     "different name of the wind",
                                                                                     closingSituation,
                                                                                     stochasts,
                                                                                     beta))
                .SetName("Different Wind Direction");
            yield return new TestCaseData(baseline, new IllustrationPointControlItem(source,
                                                                                     "different name of the wind",
                                                                                     closingSituation,
                                                                                     new[]
                                                                                     {
                                                                                         new Stochast(string.Empty, 1, 1)
                                                                                     },
                                                                                     beta))
                .SetName("Different Stochasts");
            yield return new TestCaseData(baseline, new IllustrationPointControlItem(source,
                                                                                     "different name of the wind",
                                                                                     closingSituation,
                                                                                     stochasts,
                                                                                     (RoundedDouble) 13.37))
                .SetName("Different beta");
        }
    }
}