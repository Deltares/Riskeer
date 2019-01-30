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
    public class IllustrationPointResultTest
    {
        [Test]
        public void Constructor_DescriptionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new IllustrationPointResult(null,
                                                                  new Random(21).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("description", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            const string description = "illustration point";

            var random = new Random(21);
            double value = random.NextDouble();

            // Call
            var illustrationPointResult = new IllustrationPointResult(description, value);

            // Assert
            Assert.IsInstanceOf<ICloneable>(illustrationPointResult);
            Assert.AreEqual(description, illustrationPointResult.Description);
            Assert.AreEqual(value, illustrationPointResult.Value,
                            illustrationPointResult.Value.GetAccuracy());
            Assert.AreEqual(5, illustrationPointResult.Value.NumberOfDecimalPlaces);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new IllustrationPointResult("Random description", random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}