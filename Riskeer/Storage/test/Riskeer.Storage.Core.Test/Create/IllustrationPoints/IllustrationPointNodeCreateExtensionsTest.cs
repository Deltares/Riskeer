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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.IllustrationPoints
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
        public void Create_IllustrationPointNodeWithFaultTreeIllustrationPointAndStochast_ReturnFaultTreeIllustrationPointEntity()
        {
            // Setup
            var random = new Random(21);

            var illustrationPoint = new FaultTreeIllustrationPoint(
                "Illustration point name",
                random.NextDouble(),
                new[]
                {
                    new Stochast("Stochast",
                                 random.NextDouble(),
                                 random.NextDouble())
                },
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
            CollectionAssert.IsEmpty(entity.SubMechanismIllustrationPointEntities);
            CollectionAssert.IsEmpty(entity.TopLevelFaultTreeIllustrationPointEntities);
            Assert.AreEqual(order, entity.Order);

            StochastEntity entityStochastEntity = entity.StochastEntities.Single();
            Stochast stochast = illustrationPoint.Stochasts.First();
            Assert.AreEqual(stochast.Name, entityStochastEntity.Name);
            Assert.AreEqual(stochast.Alpha, entityStochastEntity.Alpha);
            Assert.AreEqual(stochast.Duration, entityStochastEntity.Duration);
        }

        [Test]
        public void Create_IllustrationPointNodeWithSubMechanismIllustrationPoint_ThrowsNotSupportedException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

            // Call
            TestDelegate call = () => node.Create(0);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Illustration point type '{typeof(TestSubMechanismIllustrationPoint)}' is not supported.", message);
        }

        [Test]
        [TestCaseSource(nameof(GetValidIllustrationPointNodes))]
        public void Create_IllustrationPointNodeWithValidChildren_ReturnFaultTreeIllustrationPointEntity(
            IEnumerable<IllustrationPointNode> children)
        {
            // Setup
            var random = new Random(21);

            var illustrationPoint = new FaultTreeIllustrationPoint("Illustration point name A",
                                                                   random.NextDouble(),
                                                                   Enumerable.Empty<Stochast>(),
                                                                   random.NextEnumValue<CombinationType>());
            int order = random.Next();

            var node = new IllustrationPointNode(illustrationPoint);
            node.SetChildren(children.ToArray());

            // Call
            FaultTreeIllustrationPointEntity entity = node.Create(order);

            // Assert
            AssertFaultTreeIllustrationPointEntity(illustrationPoint, entity);
            AssertIllustrationPointEntities(node.Children.ToArray(),
                                            entity.SubMechanismIllustrationPointEntities.ToArray(),
                                            entity.FaultTreeIllustrationPointEntity1.ToArray());

            Assert.AreEqual(order, entity.Order);
        }

        private static IEnumerable<TestCaseData> GetValidIllustrationPointNodes()
        {
            yield return new TestCaseData(new List<IllustrationPointNode>
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
            }).SetName("SubMechanismIllustrationPoints");

            yield return new TestCaseData(new List<IllustrationPointNode>
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("B"))
            }).SetName("SubMechanismAndFaultTreeIllustrationPoints");

            yield return new TestCaseData(new List<IllustrationPointNode>
            {
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("A")),
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("B"))
            }).SetName("FaultTreeIllustrationPoints");

            var node = new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("A"));
            node.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B")),
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("C"))
            });

            yield return new TestCaseData(new List<IllustrationPointNode>
            {
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd("AA")),
                node
            }).SetName("FaultTreeIllustrationPointsWithChildren");
        }

        private static void AssertIllustrationPointEntities(
            IllustrationPointNode[] children,
            IEnumerable<SubMechanismIllustrationPointEntity> subMechanismIllustrationPointEntities,
            IEnumerable<FaultTreeIllustrationPointEntity> faultTreeIllustrationPointEntity
        )
        {
            for (var i = 0; i < children.Length; i++)
            {
                int nrOfTotalCreatedEntities = subMechanismIllustrationPointEntities.Count() + faultTreeIllustrationPointEntity.Count();
                Assert.AreEqual(nrOfTotalCreatedEntities, children.Length);

                IllustrationPointNode child = children[i];

                var subMechanismIllustrationPoint = child.Data as SubMechanismIllustrationPoint;
                if (subMechanismIllustrationPoint != null)
                {
                    SubMechanismIllustrationPointEntity illustrationPointEntity = subMechanismIllustrationPointEntities.Single(s => s.Order == i);
                    AssertSubMechanismIllustrationPointEntity(subMechanismIllustrationPoint, illustrationPointEntity);
                }

                var faultTreeIllustrationPoint = child.Data as FaultTreeIllustrationPoint;
                if (faultTreeIllustrationPoint != null)
                {
                    FaultTreeIllustrationPointEntity illustrationPointEntity = faultTreeIllustrationPointEntity.Single(f => f.Order == i);
                    AssertFaultTreeIllustrationPointEntity(faultTreeIllustrationPoint, illustrationPointEntity);

                    AssertIllustrationPointEntities(child.Children.ToArray(),
                                                    illustrationPointEntity.SubMechanismIllustrationPointEntities.ToArray(),
                                                    illustrationPointEntity.FaultTreeIllustrationPointEntity1.ToArray());
                }
            }
        }

        private static void AssertSubMechanismIllustrationPointEntity(IllustrationPointBase illustrationPoint,
                                                                      SubMechanismIllustrationPointEntity illustrationPointEntity)
        {
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(illustrationPoint.Beta, illustrationPointEntity.Beta, illustrationPoint.Beta.GetAccuracy());
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, illustrationPointEntity.Name);

            CollectionAssert.IsEmpty(illustrationPointEntity.IllustrationPointResultEntities);
            CollectionAssert.IsEmpty(illustrationPointEntity.SubMechanismIllustrationPointStochastEntities);
        }

        private static void AssertFaultTreeIllustrationPointEntity(FaultTreeIllustrationPoint illustrationPoint,
                                                                   FaultTreeIllustrationPointEntity illustrationPointEntity)
        {
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(illustrationPoint.Beta, illustrationPointEntity.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(illustrationPoint.CombinationType), illustrationPointEntity.CombinationType);
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, illustrationPointEntity.Name);
            CollectionAssert.IsEmpty(illustrationPointEntity.StochastEntities);
        }
    }
}