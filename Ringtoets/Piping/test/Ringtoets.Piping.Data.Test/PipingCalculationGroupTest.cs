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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationGroupTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var group = new PipingCalculationGroup();

            // Assert
            Assert.IsInstanceOf<ICalculationGroup>(group);
            Assert.IsInstanceOf<Observable>(group);
            Assert.IsTrue(group.IsNameEditable);
            Assert.AreEqual("Nieuwe map", group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameterdConstructor_ExpectedValues(bool isNameEditable)
        {
            // Setup
            const string newName = "new Name";

            // Call
            var group = new PipingCalculationGroup(newName, isNameEditable);

            // Assert
            Assert.IsInstanceOf<ICalculationGroup>(group);
            Assert.IsInstanceOf<Observable>(group);
            Assert.AreEqual(isNameEditable, group.IsNameEditable);
            Assert.AreEqual(newName, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void Name_SettingValueWhileNameEditable_ChangeName()
        {
            // Setup
            var group = new PipingCalculationGroup("a", true);

            // Precondtion
            Assert.IsTrue(group.IsNameEditable);

            // Call
            const string newName = "new Name";
            group.Name = newName;

            // Assert
            Assert.AreEqual(newName, group.Name);
        }

        [Test]
        public void Name_SettingValueWhileNameNotEditable_ThrowInvalidOperationException()
        {
            // Setup
            const string originalName = "a";
            var group = new PipingCalculationGroup(originalName, false);

            // Precondtion
            Assert.IsFalse(group.IsNameEditable);

            // Call
            const string newName = "new Name";
            TestDelegate call = () => group.Name = newName;

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual("Kan de naam van deze groep niet aanpassen, omdat 'IsNameEditable' op 'false' staat.", exception.Message);
            Assert.AreEqual(originalName, group.Name);
        }

        [Test]
        public void Children_AddPipingCalculation_CalculationAddedToCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var group = new PipingCalculationGroup();

            // Call
            group.Children.Add(calculation);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.Contains(group.Children, calculation);
        }

        [Test]
        public void Children_RemovePipingCalculation_CalculationRemovedFromCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var group = new PipingCalculationGroup();
            group.Children.Add(calculation);

            // Call
            group.Children.Remove(calculation);

            // Assert
            Assert.AreEqual(0, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, calculation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Children_AddPipingCalculationAtIndex_CalculationAddedToCollectionAtIndex(int index)
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();
            var calculation = new PipingCalculation(generalInputParameters, semiProbabilisticInputParameters);
            var calculationToInsert = new PipingCalculation(generalInputParameters, semiProbabilisticInputParameters);

            var group = new PipingCalculationGroup();
            group.Children.Add(calculation);

            // Call
            group.Children.Insert(index, calculationToInsert);

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            Assert.AreSame(calculationToInsert, group.Children[index]);
            CollectionAssert.AreEquivalent(new[]
            {
                calculationToInsert,
                calculation
            }, group.Children,
                                           "Already existing items should have remained in collection and new item should be added.");
        }

        [Test]
        public void Children_AddPipingCalculationGroup_GroupAddedToCollection()
        {
            // Setup
            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();

            // Call
            group.Children.Add(childGroup);

            // Assert
            CollectionAssert.Contains(group.Children, childGroup);
        }

        [Test]
        public void Children_RemovePipingCalculationGroup_GroupRemovedFromCollection()
        {
            // Setup
            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);

            // Call
            group.Children.Remove(childGroup);

            // Assert
            CollectionAssert.DoesNotContain(group.Children, childGroup);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Children_RemovePipingCalculationGroup_CalculationRemovedFromCollection(int index)
        {
            // Setup
            var existingGroup = new PipingCalculationGroup();
            var groupToInsert = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(existingGroup);

            // Call
            group.Children.Insert(index, groupToInsert);

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            Assert.AreSame(groupToInsert, group.Children[index]);
            CollectionAssert.AreEquivalent(new[]
            {
                groupToInsert,
                existingGroup
            }, group.Children,
                                           "Already existing items should have remained in collection and new item should be added.");
        }
    }
}