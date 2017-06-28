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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.IllustrationPoints;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class IStochastEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IStochastEntity) null).Read();

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnStochast()
        {
            // Setup
            var random = new Random(123);
            var mockRepository = new MockRepository();
            var entity = mockRepository.Stub<IStochastEntity>();
            entity.Name = "Description";
            entity.Alpha = random.NextDouble();
            entity.Duration = random.NextDouble();
            mockRepository.ReplayAll();

            // Call
            Stochast illustrationPointResult = entity.Read();

            // Assert
            Assert.AreEqual(entity.Name, illustrationPointResult.Name);
            Assert.AreEqual(entity.Alpha, illustrationPointResult.Alpha, illustrationPointResult.Alpha.GetAccuracy());
            Assert.AreEqual(entity.Duration, illustrationPointResult.Duration, illustrationPointResult.Duration.GetAccuracy());
            mockRepository.VerifyAll();
        }
    }
}