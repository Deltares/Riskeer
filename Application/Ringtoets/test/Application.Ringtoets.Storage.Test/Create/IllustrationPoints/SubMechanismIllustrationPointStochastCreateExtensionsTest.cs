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
    public class SubMechanismIllustrationPointStochastCreateExtensionsTest
    {
        [Test]
        public void CreateSubMechanismIllustrationPointStochastEntity_StochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubMechanismIllustrationPointStochast) null).CreateSubMechanismIllustrationPointStochastEntity(0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("subMechanismIllustrationPointStochast", paramName);
        }

        [Test]
        public void CreateSubMechanismIllustrationPointStochastEntity_ValidRealizedStochast_ReturnSubMechanismIllustrationPointStochastEntity()
        {
            // Setup
            var random = new Random(123);
            var stochast = new SubMechanismIllustrationPointStochast("Some description",
                                                                     random.NextDouble(),
                                                                     random.NextDouble(),
                                                                     random.NextDouble());
            int order = random.Next();

            // Call
            SubMechanismIllustrationPointStochastEntity entity = stochast.CreateSubMechanismIllustrationPointStochastEntity(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(stochast.Name, entity.Name);
            Assert.AreEqual(stochast.Alpha, entity.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochast.Duration, entity.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(stochast.Realization, entity.Realization, stochast.Realization.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }
    }
}