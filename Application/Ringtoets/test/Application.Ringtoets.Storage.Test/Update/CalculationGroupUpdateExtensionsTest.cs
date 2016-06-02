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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Calculation;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class CalculationGroupUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities rintoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                StorageId = 1
            };

            // Call
            TestDelegate call = () => calculationGroup.Update(null, rintoetsEntities);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationGroup = new CalculationGroup
            {
                StorageId = 1
            };

            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => calculationGroup.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_GroupHasNotBeenSaved_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                StorageId = 3
            };

            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            string message = Assert.Throws<EntityNotFoundException>(call).Message;
            Assert.AreEqual("Het object 'CalculationGroupEntity' met id '3' is niet gevonden.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_GroupPropertiesChanged_EntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = CreateCalculationGroupWithoutChildren(true);

            var groupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = calculationGroup.StorageId,
                IsEditable = Convert.ToByte(calculationGroup.IsNameEditable),
                Name = "<original name>"
            };
            ringtoetsEntities.CalculationGroupEntities.Add(groupEntity);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(calculationGroup.Name, groupEntity.Name);

            CollectionAssert.IsEmpty(groupEntity.CalculationGroupEntity1,
                "No changes to the children expected.");
            mocks.VerifyAll();
        }

        [Test]
        public void Update_GroupWithReadonlyNameWithoutChildren_NoChangedToEntity()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = CreateCalculationGroupWithoutChildren(false);

            byte originalIsEditableValue = Convert.ToByte(calculationGroup.IsNameEditable);
            var groupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = calculationGroup.StorageId,
                IsEditable = originalIsEditableValue,
                Name = calculationGroup.Name
            };
            ringtoetsEntities.CalculationGroupEntities.Add(groupEntity);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(calculationGroup.Name, groupEntity.Name);
            Assert.AreEqual(originalIsEditableValue, groupEntity.IsEditable);

            CollectionAssert.IsEmpty(groupEntity.CalculationGroupEntity1,
                "No changes to the children expected.");
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ChildGroupsReordered_EntitiesUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = CreateCalculationGroupWith2ChildGroups();

            var childGroupEntity1 = new CalculationGroupEntity
            {
                CalculationGroupEntityId = ((CalculationGroup)calculationGroup.Children[0]).StorageId,
                Name = "A",
                Order = 1
            };
            var childGroupEntity2 = new CalculationGroupEntity
            {
                CalculationGroupEntityId = ((CalculationGroup)calculationGroup.Children[1]).StorageId,
                Name = "A",
                Order = 0
            };
            var groupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = calculationGroup.StorageId,
                IsEditable = Convert.ToByte(calculationGroup.IsNameEditable),
                Name = "<original name>",
                CalculationGroupEntity1 =
                {
                    childGroupEntity1,
                    childGroupEntity2
                }
            };
            ringtoetsEntities.CalculationGroupEntities.Add(groupEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroupEntity1);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroupEntity2);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, childGroupEntity1.Order);
            Assert.AreEqual(calculationGroup.Children[0].Name, childGroupEntity1.Name);
            Assert.AreEqual(1, childGroupEntity2.Order);
            Assert.AreEqual(calculationGroup.Children[1].Name, childGroupEntity2.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ChildGroupInserted_EntitiesUpdatedAndNewEntityCreated()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = CreateCalculationGroupWith2ChildGroups();
            
            CalculationGroupEntity childGroupEntity1 = CreateExpectedEmptyGroupEntity((CalculationGroup)calculationGroup.Children[0], 0);
            CalculationGroupEntity childGroupEntity2 = CreateExpectedEmptyGroupEntity((CalculationGroup)calculationGroup.Children[1], 1);
            var groupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = calculationGroup.StorageId,
                IsEditable = Convert.ToByte(calculationGroup.IsNameEditable),
                Name = "<original name>",
                CalculationGroupEntity1 =
                {
                    childGroupEntity1,
                    childGroupEntity2
                }
            };
            ringtoetsEntities.CalculationGroupEntities.Add(groupEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroupEntity1);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroupEntity2);

            var insertedGroup = new CalculationGroup("<newly inserted group>", false);
            const int insertedIndex = 1;
            calculationGroup.Children.Insert(insertedIndex, insertedGroup);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            CalculationGroupEntity[] updatedChildGroupEntities = groupEntity.CalculationGroupEntity1
                                                                            .OrderBy(cge => cge.Order)
                                                                            .ToArray();
            Assert.AreEqual(3, updatedChildGroupEntities.Length);
            Assert.AreSame(childGroupEntity1, updatedChildGroupEntities[0]);
            Assert.AreEqual(0, childGroupEntity1.Order);
            Assert.AreEqual(calculationGroup.Children[0].Name, childGroupEntity1.Name);

            var newGroupEntity = updatedChildGroupEntities[insertedIndex];
            Assert.AreEqual(insertedIndex, newGroupEntity.Order);
            Assert.AreEqual(insertedGroup.Name, newGroupEntity.Name);
            Assert.AreEqual(0, newGroupEntity.IsEditable);

            Assert.AreSame(childGroupEntity2, updatedChildGroupEntities[2]);
            Assert.AreEqual(2, childGroupEntity2.Order);
            Assert.AreEqual(calculationGroup.Children[2].Name, childGroupEntity2.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ChildGroupAdded_RootEntityUpdatedAndNewEntityCreated()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = CreateCalculationGroupWithoutChildren(true);

            CalculationGroupEntity rootGroupEntity = CreateExpectedEmptyGroupEntity(calculationGroup, 0);
            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);

            var newGroup = new CalculationGroup("<newly added group>", false);
            calculationGroup.Children.Add(newGroup);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            CalculationGroupEntity[] updatedChildGroupEntities = rootGroupEntity.CalculationGroupEntity1
                                                                                .OrderBy(cge => cge.Order)
                                                                                .ToArray();
            Assert.AreEqual(1, updatedChildGroupEntities.Length);
            var newGroupEntity = updatedChildGroupEntities[0];
            Assert.AreEqual(0, newGroupEntity.Order);
            Assert.AreEqual(newGroup.Name, newGroupEntity.Name);
            Assert.AreEqual(0, newGroupEntity.IsEditable);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ExistingChildGroupDragged_EntitiesUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var draggedGroup = new CalculationGroup("<Dragged group>", false)
            {
                StorageId = 876
            };
            CalculationGroup calculationGroup = CreateCalculationGroupWith2ChildGroups();
            ((CalculationGroup)calculationGroup.Children[0]).Children.Add(draggedGroup);

            CalculationGroupEntity childGroup1Entity = CreateExpectedEmptyGroupEntity((CalculationGroup)calculationGroup.Children[0], 0);
            CalculationGroupEntity childGroup2Entity = CreateExpectedEmptyGroupEntity((CalculationGroup)calculationGroup.Children[1], 1);
            CalculationGroupEntity fillerGroupEntity = CreateExpectedEmptyGroupEntity(new CalculationGroup(), 0);
            childGroup2Entity.CalculationGroupEntity1.Add(fillerGroupEntity);
            CalculationGroupEntity draggedGroupEntity = CreateExpectedEmptyGroupEntity(draggedGroup, 1);
            childGroup2Entity.CalculationGroupEntity1.Add(draggedGroupEntity);
            draggedGroupEntity.CalculationGroupEntity2 = childGroup2Entity;

            CalculationGroupEntity rootGroupEntity = CreateExpectedEmptyGroupEntity(calculationGroup, 0);
            rootGroupEntity.CalculationGroupEntity1.Add(childGroup1Entity);
            rootGroupEntity.CalculationGroupEntity1.Add(childGroup2Entity);

            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroup1Entity);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroup2Entity);
            ringtoetsEntities.CalculationGroupEntities.Add(fillerGroupEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(draggedGroupEntity);

            var registry = new PersistenceRegistry();

            // Call
            calculationGroup.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, draggedGroupEntity.Order);
            CollectionAssert.Contains(childGroup1Entity.CalculationGroupEntity1, draggedGroupEntity);
            CollectionAssert.DoesNotContain(childGroup2Entity.CalculationGroupEntity1, draggedGroupEntity);

            Assert.AreEqual(0, fillerGroupEntity.Order);
            mocks.VerifyAll();
        }

        private static CalculationGroup CreateCalculationGroupWithoutChildren(bool isNameEditable)
        {
            return new CalculationGroup("<GroupWithoutChildren>", isNameEditable)
            {
                StorageId = 5689467
            };
        }

        private CalculationGroup CreateCalculationGroupWith2ChildGroups()
        {
            return new CalculationGroup("<GroupWithTwoChildGroups", true)
            {
                StorageId = 4857,
                Children =
                {
                    new CalculationGroup("<Child1>", true)
                    {
                        StorageId = 98
                    },
                    new CalculationGroup("<Child2>", true)
                    {
                        StorageId = 65
                    }
                }
            };
        }

        private CalculationGroupEntity CreateExpectedEmptyGroupEntity(CalculationGroup group, int order)
        {
            return new CalculationGroupEntity
            {
                CalculationGroupEntityId = group.StorageId,
                Name = group.Name,
                IsEditable = Convert.ToByte(group.IsNameEditable),
                Order = order
            };
        }
    }
}