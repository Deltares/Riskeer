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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointStochastTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            const string name = "Stochast name";

            var random = new Random(21);
            double duration = random.NextDouble();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();

            // Call
            var stochast = new SubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            // Assert
            Assert.IsInstanceOf<Stochast>(stochast);
            Assert.AreEqual(name, stochast.Name);
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());

            Assert.AreEqual(realization, stochast.Realization,
                            stochast.Realization.GetAccuracy());
            Assert.AreEqual(5, stochast.Realization.NumberOfDecimalPlaces);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new SubMechanismIllustrationPointStochast("Random name",
                                                                     random.NextDouble(),
                                                                     random.NextDouble(),
                                                                     random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}