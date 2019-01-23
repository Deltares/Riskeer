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
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Ringtoets.Storage.Core.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IllustrationPointResultEntity) null).Read();

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnIllustrationPointResult()
        {
            // Setup
            var random = new Random(123);
            var entity = new IllustrationPointResultEntity
            {
                Description = "Description",
                Value = random.NextDouble()
            };

            // Call
            IllustrationPointResult illustrationPointResult = entity.Read();

            // Assert
            Assert.AreEqual(entity.Description, illustrationPointResult.Description);
            Assert.AreEqual(entity.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());
        }
    }
}