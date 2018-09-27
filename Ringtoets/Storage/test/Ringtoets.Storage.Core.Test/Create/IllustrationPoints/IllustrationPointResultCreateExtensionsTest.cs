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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Storage.Core.Create.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointResultCreateExtensionsTest
    {
        [Test]
        public void Create_IllustrationPointResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IllustrationPointResult) null).Create(0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("illustrationPointResult", paramName);
        }

        [Test]
        public void Create_ValidIllustrationPointResult_ReturnIllustrationPointResultEntity()
        {
            // Setup
            var random = new Random(123);
            var illustrationPointResult = new IllustrationPointResult("Some description",
                                                                      random.NextDouble());
            int order = random.Next();

            // Call
            IllustrationPointResultEntity entity =
                illustrationPointResult.Create(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPointResult.Description, entity.Description);
            Assert.AreEqual(illustrationPointResult.Value, entity.Value, illustrationPointResult.Value.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }
    }
}