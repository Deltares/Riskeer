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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.IllustrationPoints;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((FaultTreeIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnsIllustrationPointNode()
        {
            // Setup
            var random = new Random(21);

            var combinationType = random.NextEnumValue<CombinationType>();
            var entity = new FaultTreeIllustrationPointEntity
            {
                Name = "FaultTreeIllustrationPointEntity",
                Beta = random.NextDouble(),
                CombinationType = Convert.ToByte(combinationType),
                StochastEntities =
                {
                    new StochastEntity
                    {
                        Alpha = random.NextDouble(),
                        Duration = random.NextDouble(),
                        Name = "StochastEntity"
                    }
                }
            };

            // Call
            IllustrationPointNode node = entity.Read();

            // Assert
            var illustrationPoint = node.Data as FaultTreeIllustrationPoint;
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(combinationType, illustrationPoint.CombinationType);

            AssertStochasts(entity.StochastEntities.ToArray(), illustrationPoint.Stochasts.ToArray());

            CollectionAssert.IsEmpty(node.Children);
        }

        private static void AssertStochasts(StochastEntity[] stochastEntities, Stochast[] stochasts)
        {
            Assert.AreEqual(stochastEntities.Length, stochasts.Length);
            for (var i = 0; i < stochasts.Length; i++)
            {
                AssertStochast(stochastEntities[i], stochasts[i]);
            }
        }

        private static void AssertStochast(StochastEntity stochastEntity, Stochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
        }
    }
}