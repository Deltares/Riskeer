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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint(null,
                                                                     12.3,
                                                                     Enumerable.Empty<Stochast>(),
                                                                     CombinationType.And);
            ;
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint("Test",
                                                                     12.3,
                                                                     null,
                                                                     CombinationType.And);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_InvalidCombinationType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var invalidEnum = (CombinationType) 9001;

            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint("Test", 12.3,
                                                                     Enumerable.Empty<Stochast>(),
                                                                     invalidEnum);

            // Assert
            const string expectedMessage = "The value of argument 'value' (9001) is invalid for Enum type 'CombinationType'.";
            var exception = 
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
            Assert.AreEqual("value", exception.ParamName);
        }

        [Test]
        [TestCase(CombinationType.And)]
        [TestCase(CombinationType.Or)]
        public void Constructor_ValidArguments_ReturnsExpectedValues(CombinationType combinationType)
        {
            // Setup
            const string name = "Fault tree illustration point name";

            var random = new Random(21);
            double beta = random.NextDouble();

            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();

            // Call
            var illustrationPoint = new FaultTreeIllustrationPoint(name, beta, stochasts, combinationType);

            // Assert
            Assert.IsInstanceOf<IllustrationPointBase>(illustrationPoint);

            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(combinationType, illustrationPoint.CombinationType);
        }
    }
}