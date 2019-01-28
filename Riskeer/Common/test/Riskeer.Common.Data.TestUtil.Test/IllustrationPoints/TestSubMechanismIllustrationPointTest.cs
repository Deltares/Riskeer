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

using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestSubMechanismIllustrationPointTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration Point", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
            Assert.AreEqual(3.14, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }

        [Test]
        public void ParameteredConstructor_WithBeta_ExpectedValues()
        {
            // Setup
            const double beta = 1.23;

            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint(beta);

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration Point", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }

        [Test]
        public void ParameteredConstructor_WithStochasts_ExpectedValues()
        {
            // Setup
            var stochasts = new[]
            {
                new SubMechanismIllustrationPointStochast("Stochast A", 3.0, 0.5, 12.0)
            };

            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint(stochasts);

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration Point", illustrationPoint.Name);
            Assert.AreEqual(3.14, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());

            CollectionAssert.IsNotEmpty(illustrationPoint.Stochasts);
            Assert.AreEqual("Stochast A", illustrationPoint.Stochasts.First().Name);
            Assert.AreEqual(3.0, illustrationPoint.Stochasts.First().Duration);
            Assert.AreEqual(0.5, illustrationPoint.Stochasts.First().Alpha);
            Assert.AreEqual(12.0, illustrationPoint.Stochasts.First().Realization);
        }

        [Test]
        public void ParameteredConstructor_WithName_ExpectedValues()
        {
            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint("Testing Name");

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Testing Name", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            Assert.AreEqual(3.14, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }
    }
}