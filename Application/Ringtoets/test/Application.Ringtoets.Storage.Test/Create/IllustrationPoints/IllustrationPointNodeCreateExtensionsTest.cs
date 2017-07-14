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
using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointNodeCreateExtensionsTest
    {
        [Test]
        public void Create_IllustrationPointNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((IllustrationPointNode) null).Create(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("illustrationPointNode", exception.ParamName);
        }

        [Test]
        public void Create_IllustrationPointNodeWithFaultTreeIllustrationPoint_ReturnFaultTreeIllustrationPointEntity()
        {
            // Setup
            var random = new Random(21);

            var illustrationPoint = new FaultTreeIllustrationPoint("Illustration point name",
                                                                   random.NextDouble(),
                                                                   Enumerable.Empty<Stochast>(),
                                                                   random.NextEnumValue<CombinationType>());
            int order = random.Next();

            var node = new IllustrationPointNode(illustrationPoint);

            // Call
            FaultTreeIllustrationPointEntity entity = node.Create(order);

            // Assert
            Assert.IsNull(entity.ParentFaultTreeIllustrationPointEntityId);
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
            byte expectedCombinationType = Convert.ToByte(illustrationPoint.CombinationType);
            Assert.AreEqual(expectedCombinationType, entity.CombinationType);
            CollectionAssert.IsEmpty(entity.FaultTreeIllustrationPointEntity1);
            CollectionAssert.IsEmpty(entity.StochastEntities);
            CollectionAssert.IsEmpty(entity.SubMechanismIllustrationPointEntities);
            CollectionAssert.IsEmpty(entity.TopLevelFaultTreeIllustrationPointEntities);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_IllustrationPointNodeWithSubMechanismIllustrationPoint_ThrowsInvalidOperationException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

            // Call
            TestDelegate call = () => node.Create(0);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual($"Illustration point type '{typeof(TestSubMechanismIllustrationPoint)}' is not supported.", message);
        }
    }
}