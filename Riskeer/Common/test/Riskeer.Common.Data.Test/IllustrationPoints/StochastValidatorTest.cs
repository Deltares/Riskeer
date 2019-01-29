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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class StochastValidatorTest
    {
        [Test]
        public void ValidateStochasts_StochastsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StochastValidator.ValidateStochasts(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void ValidateStochasts_UniqueStochasts_DoesNotThrow()
        {
            // Setup
            var stochasts = new List<Stochast>
            {
                new Stochast("Stochast A", 1, 2),
                new Stochast("Stochast B", 2, 3),
                new Stochast("Stochast C", 1, 3)
            };

            // Call
            TestDelegate test = () => StochastValidator.ValidateStochasts(stochasts);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void ValidateStochasts_DuplicateStochasts_ThrowsArgumentException()
        {
            // Setup
            var stochasts = new List<Stochast>
            {
                new Stochast("Stochast A", 1, 2),
                new Stochast("Stochast B", 2, 3),
                new Stochast("Stochast B", 1, 3)
            };

            // Call
            TestDelegate test = () => StochastValidator.ValidateStochasts(stochasts);

            // Assert
            const string expectedMessage = "Een of meerdere stochasten hebben dezelfde naam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}