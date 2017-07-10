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
using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointResultCreateExtensionsTest
    {
        [Test]
        public void CreateIllustrationPointResultEntity_IllustrationPointResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IllustrationPointResult) null).CreateIllustrationPointResultEntity(0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("illustrationPointResult", paramName);
        }

        [Test]
        public void CreateIllustrationPointResultEntity_ValidIllustrationPointResult_ReturnEntity()
        {
            // Setup
            var random = new Random(123);
            var illustrationPointResult = new IllustrationPointResult("Some description",
                                                                      random.NextDouble());
            int order = random.Next();

            // Call
            IllustrationPointResultEntity entity =
                illustrationPointResult.CreateIllustrationPointResultEntity(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPointResult.Description, entity.Description);
            Assert.AreEqual(illustrationPointResult.Value, entity.Value, illustrationPointResult.Value.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }
    }
}