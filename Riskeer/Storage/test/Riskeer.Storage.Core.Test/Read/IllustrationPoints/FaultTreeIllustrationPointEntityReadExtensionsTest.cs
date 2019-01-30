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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Read.IllustrationPoints
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
        public void Read_ValidEntityWithStochasts_ReturnsIllustrationPointNode()
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

        [Test]
        public void Read_ValidEntityWithIllustrationPoints_ReturnsIllustrationPointNode()
        {
            // Setup
            var random = new Random(21);
            var entity = new FaultTreeIllustrationPointEntity
            {
                Name = "FaultTreeIllustrationPointEntity",
                Beta = random.NextDouble(),
                CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                SubMechanismIllustrationPointEntities =
                {
                    new SubMechanismIllustrationPointEntity
                    {
                        Name = "SubMechanismIllustrationPointEntity",
                        Beta = random.NextDouble(),
                        Order = 10
                    }
                },
                FaultTreeIllustrationPointEntity1 =
                {
                    new FaultTreeIllustrationPointEntity
                    {
                        Name = "FaultTreeIllustrationPointEntity",
                        Beta = random.NextDouble(),
                        CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                        Order = 5
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
            Assert.AreEqual((CombinationType) entity.CombinationType, illustrationPoint.CombinationType);

            IllustrationPointNode[] children = node.Children.ToArray();
            Assert.AreEqual(2, children.Length);

            AssertFaultTreeIllustrationPointNode(entity.FaultTreeIllustrationPointEntity1.First(), children[0]);
            AssertSubMechanismIllustrationPointNode(entity.SubMechanismIllustrationPointEntities.First(), children[1]);
        }

        [Test]
        public void Read_ValidEntityWithNestedIllustrationPoints_ReturnsIllustrationPointNodeWithNesting()
        {
            // Setup
            var random = new Random(21);
            var entity = new FaultTreeIllustrationPointEntity
            {
                Name = "FaultTreeIllustrationPointEntity",
                Beta = random.NextDouble(),
                CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                SubMechanismIllustrationPointEntities =
                {
                    new SubMechanismIllustrationPointEntity
                    {
                        Name = "SubMechanismIllustrationPointEntity",
                        Beta = random.NextDouble(),
                        Order = 0
                    }
                },
                FaultTreeIllustrationPointEntity1 =
                {
                    new FaultTreeIllustrationPointEntity
                    {
                        Name = "FaultTreeIllustrationPointEntityChild",
                        Beta = random.NextDouble(),
                        CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                        Order = 1,
                        FaultTreeIllustrationPointEntity1 =
                        {
                            new FaultTreeIllustrationPointEntity
                            {
                                Name = "FaultTreeIllustrationPointEntityChildsChild",
                                Beta = random.NextDouble(),
                                CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                                Order = 0
                            },
                            new FaultTreeIllustrationPointEntity
                            {
                                Name = "FaultTreeIllustrationPointEntityChildsSecondChild",
                                Beta = random.NextDouble(),
                                CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                                Order = 1
                            }
                        }
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
            Assert.AreEqual((CombinationType) entity.CombinationType, illustrationPoint.CombinationType);

            IllustrationPointNode[] children = node.Children.ToArray();
            Assert.AreEqual(2, children.Length);

            AssertIllustrationPointNodes(entity.FaultTreeIllustrationPointEntity1, children);
            AssertIllustrationPointNodes(entity.SubMechanismIllustrationPointEntities, children);
        }

        private static void AssertIllustrationPointNodes(ICollection<FaultTreeIllustrationPointEntity> entities,
                                                         IEnumerable<IllustrationPointNode> nodes)
        {
            FaultTreeIllustrationPointEntity[] entitiesArray = entities.OrderBy(ip => ip.Order).ToArray();
            IllustrationPointNode[] faultTreeNodes = nodes.Where(n => n.Data.GetType() == typeof(FaultTreeIllustrationPoint)).ToArray();

            Assert.AreEqual(entitiesArray.Length, faultTreeNodes.Length);

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                FaultTreeIllustrationPointEntity entity = entitiesArray[i];
                IllustrationPointNode node = faultTreeNodes[i];

                AssertFaultTreeIllustrationPointNode(entity, node);
            }
        }

        private static void AssertIllustrationPointNodes(ICollection<SubMechanismIllustrationPointEntity> entities,
                                                         IEnumerable<IllustrationPointNode> nodes)
        {
            SubMechanismIllustrationPointEntity[] entitiesArray = entities.OrderBy(ip => ip.Order).ToArray();
            IllustrationPointNode[] subMechanismNodes = nodes.Where(n => n.Data.GetType() == typeof(SubMechanismIllustrationPoint)).ToArray();

            Assert.AreEqual(entitiesArray.Length, subMechanismNodes.Length);

            for (var i = 0; i < entitiesArray.Length; i++)
            {
                SubMechanismIllustrationPointEntity entity = entitiesArray[i];
                IllustrationPointNode node = subMechanismNodes[i];

                AssertSubMechanismIllustrationPointNode(entity, node);
            }
        }

        private static void AssertFaultTreeIllustrationPointNode(FaultTreeIllustrationPointEntity entity,
                                                                 IllustrationPointNode node)
        {
            var illustrationPoint = node.Data as FaultTreeIllustrationPoint;
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual((CombinationType) entity.CombinationType, illustrationPoint.CombinationType);
            AssertStochasts(entity.StochastEntities.ToArray(), illustrationPoint.Stochasts.ToArray());

            AssertIllustrationPointNodes(entity.FaultTreeIllustrationPointEntity1, node.Children);
            AssertIllustrationPointNodes(entity.SubMechanismIllustrationPointEntities, node.Children);
        }

        private static void AssertSubMechanismIllustrationPointNode(SubMechanismIllustrationPointEntity entity,
                                                                    IllustrationPointNode node)
        {
            var illustrationPoint = node.Data as SubMechanismIllustrationPoint;
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            AssertStochasts(entity.SubMechanismIllustrationPointStochastEntities.ToArray(), illustrationPoint.Stochasts.ToArray());

            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
            CollectionAssert.IsEmpty(node.Children);
        }

        private static void AssertStochasts(SubMechanismIllustrationPointStochastEntity[] stochastEntities,
                                            SubMechanismIllustrationPointStochast[] stochasts)
        {
            Assert.AreEqual(stochastEntities.Length, stochasts.Length);

            for (var i = 0; i < stochastEntities.Length; i++)
            {
                SubMechanismIllustrationPointStochastEntity stochastEntity = stochastEntities[i];
                SubMechanismIllustrationPointStochast stochast = stochasts[i];

                Assert.AreEqual(stochastEntity.Name, stochast.Name);
                Assert.AreEqual(stochastEntity.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
                Assert.AreEqual(stochastEntity.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
                Assert.AreEqual(stochastEntity.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
            }
        }

        private static void AssertStochasts(StochastEntity[] stochastEntities, Stochast[] stochasts)
        {
            Assert.AreEqual(stochastEntities.Length, stochasts.Length);

            for (var i = 0; i < stochastEntities.Length; i++)
            {
                StochastEntity stochastEntity = stochastEntities[i];
                Stochast stochast = stochasts[i];

                Assert.AreEqual(stochastEntity.Name, stochast.Name);
                Assert.AreEqual(stochastEntity.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
                Assert.AreEqual(stochastEntity.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            }
        }
    }
}