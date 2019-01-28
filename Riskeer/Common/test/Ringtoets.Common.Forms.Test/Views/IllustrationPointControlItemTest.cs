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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointControlItemTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const string windDirectionName = "Wind direction name";
            const string closingSituation = "Closing situation";

            const int nrOfDecimals = 10;
            var beta = new RoundedDouble(nrOfDecimals, 13.37);

            var source = new TestTopLevelIllustrationPoint();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();

            // Call
            var item = new IllustrationPointControlItem(source,
                                                        windDirectionName,
                                                        closingSituation,
                                                        stochasts,
                                                        beta);
            // Assert
            Assert.AreSame(source, item.Source);
            Assert.AreEqual(nrOfDecimals, item.Beta.NumberOfDecimalPlaces);
            Assert.AreEqual(beta, item.Beta, item.Beta.GetAccuracy());
            Assert.AreSame(stochasts, item.Stochasts);
        }
    }
}