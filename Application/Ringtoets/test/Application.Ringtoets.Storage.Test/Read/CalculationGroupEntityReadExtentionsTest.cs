// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Read;

using NUnit.Framework;

using Ringtoets.Common.Data.Calculation;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class CalculationGroupEntityReadExtentionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(123, "A", 1)]
        [TestCase(1, "b", 0)]
        public void Read_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            long id, string name, byte isEditable)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = id,
                Name = name,
                IsEditable = isEditable,
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.Read(collector);

            // Assert
            Assert.AreEqual(id, group.StorageId);
            Assert.AreEqual(name, group.Name);
            Assert.AreEqual(Convert.ToBoolean(isEditable), group.IsNameEditable);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void Read_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 2,
                        Name = "AA",
                        IsEditable = 1,
                        Order = 0
                    },
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 3,
                        Name = "AB",
                        IsEditable = 0,
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 4,
                                Name = "ABA",
                                IsEditable = 0,
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 5,
                                Name = "ABB",
                                IsEditable = 1,
                                Order = 1
                            }
                        },
                        Order = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.Read(collector);

            // Assert
            Assert.AreEqual(1, rootGroup.StorageId);
            Assert.AreEqual("A", rootGroup.Name);
            Assert.IsFalse(rootGroup.IsNameEditable);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup)rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            Assert.AreEqual(2, rootChildGroup1.StorageId);
            Assert.IsTrue(rootChildGroup1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup)rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);
            Assert.AreEqual(3, rootChildGroup2.StorageId);
            Assert.IsFalse(rootChildGroup2.IsNameEditable);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup)rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            Assert.AreEqual(4, rootChildGroup1Child1.StorageId);
            Assert.IsFalse(rootChildGroup1Child1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup)rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            Assert.AreEqual(5, rootChildGroup1Child2.StorageId);
            Assert.IsTrue(rootChildGroup1Child2.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }
    }
}