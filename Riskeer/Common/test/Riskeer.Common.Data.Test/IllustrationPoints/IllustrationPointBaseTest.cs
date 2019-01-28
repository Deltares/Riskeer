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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointBaseTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestIllustrationPointBase(null, 12.3);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const string name = "Illustration Point Name";

            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            var illustrationPoint = new TestIllustrationPointBase(name, beta);

            // Assert
            Assert.IsInstanceOf<ICloneable>(illustrationPoint);
            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(5, illustrationPoint.Beta.NumberOfDecimalPlaces);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new TestIllustrationPointBase("Random name", random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        private class TestIllustrationPointBase : IllustrationPointBase
        {
            public TestIllustrationPointBase(string name, double beta)
                : base(name, beta) {}
        }
    }
}