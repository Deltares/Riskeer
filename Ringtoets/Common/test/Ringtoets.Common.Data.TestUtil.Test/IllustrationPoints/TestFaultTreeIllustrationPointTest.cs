﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestFaultTreeIllustrationPointTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var illustrationPoint = new TestFaultTreeIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration Point", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            Assert.AreEqual(3.14, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const double beta = 1.23;

            // Call
            var illustrationPoint = new TestFaultTreeIllustrationPoint(beta);

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration Point", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }
    }
}