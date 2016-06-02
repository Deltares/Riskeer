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

using NUnit.Framework;

using Ringtoets.Common.Data.Calculation;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class CalculationGroupCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var group = new CalculationGroup();

            // Call
            TestDelegate call = () => group.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase(true, 0)]
        [TestCase(false, 123)]
        public void Create_GroupWithoutChildren_CreateEntity(bool isNameEditable, int order)
        {
            // Setup
            const string name = "blaballab";
            var group = new CalculationGroup(name, isNameEditable);

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, order);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(Convert.ToByte(isNameEditable), entity.IsEditable);
            Assert.AreEqual(order, entity.Order);

            CollectionAssert.IsEmpty(entity.CalculationGroupEntity1);
            CollectionAssert.IsEmpty(entity.FailureMechanismEntities);
            Assert.IsNull(entity.ParentCalculationGroupEntityId);
        }

        [Test]
        public void Create_GroupWithChildren_CreateEntities()
        {
            // Setup
            const string name = "blaballab";
            var group = new CalculationGroup(name, true);
            group.Children.Add(new CalculationGroup("A", true)
            {
                Children =
                {
                    new CalculationGroup("AA", false),
                    new CalculationGroup("AB", true)
                }
            });
            group.Children.Add(new CalculationGroup("B", false));

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(1, entity.IsEditable);
            Assert.AreEqual(0, entity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(1, childEntity1.IsEditable);
            Assert.AreEqual(0, childEntity1.Order);
            Assert.AreEqual(2, childEntity1.CalculationGroupEntity1.Count);
            CalculationGroupEntity childEntity1ChildEntity1 = childEntity1.CalculationGroupEntity1.ElementAt(0);
            Assert.AreEqual("AA", childEntity1ChildEntity1.Name);
            Assert.AreEqual(0, childEntity1ChildEntity1.IsEditable);
            Assert.AreEqual(0, childEntity1ChildEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1ChildEntity1.CalculationGroupEntity1);
            CalculationGroupEntity childEntity1ChildEntity2 = childEntity1.CalculationGroupEntity1.ElementAt(1);
            Assert.AreEqual("AB", childEntity1ChildEntity2.Name);
            Assert.AreEqual(1, childEntity1ChildEntity2.IsEditable);
            Assert.AreEqual(1, childEntity1ChildEntity2.Order);
            CollectionAssert.IsEmpty(childEntity1ChildEntity2.CalculationGroupEntity1);

            CalculationGroupEntity childEntity2 = childGroupEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(0, childEntity2.IsEditable);
            Assert.AreEqual(1, childEntity2.Order);
            CollectionAssert.IsEmpty(childEntity2.CalculationGroupEntity1);
        }
    }
}