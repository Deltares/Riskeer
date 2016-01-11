using System;

using Core.Common.Base;

using NUnit.Framework;

using Rhino.Mocks;

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
            Assert.IsInstanceOf<IPipingCalculationItem>(group);
            Assert.IsInstanceOf<Observable>(group);
            Assert.IsTrue(group.IsNameEditable);
            Assert.AreEqual("Nieuwe map", group.Name);
            Assert.IsFalse(group.HasOutput);
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
            Assert.IsInstanceOf<IPipingCalculationItem>(group);
            Assert.IsInstanceOf<Observable>(group);
            Assert.AreEqual(isNameEditable, group.IsNameEditable);
            Assert.AreEqual(newName, group.Name);
            Assert.IsFalse(group.HasOutput);
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
            var calculation = new PipingCalculation();

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
            var calculation = new PipingCalculation();

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
            var calculation = new PipingCalculation();
            var calculationToInsert = new PipingCalculation();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculation);

            // Call
            group.Children.Insert(index, calculationToInsert);

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            Assert.AreSame(calculationToInsert, group.Children[index]);
            CollectionAssert.AreEquivalent(new[]{calculationToInsert,calculation}, group.Children,
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
            CollectionAssert.AreEquivalent(new[] { groupToInsert, existingGroup }, group.Children,
                "Already existing items should have remained in collection and new item should be added.");
        }

        [Test]
        public void HasOutput_NoChildren_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();

            // Precondition
            CollectionAssert.IsEmpty(group.Children);

            // Call
            var hasOutput = group.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void HasOutput_HasChildWithOutput_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var childWithoutOutput = mocks.Stub<IPipingCalculationItem>();
            childWithoutOutput.Stub(c => c.HasOutput).Return(false);

            var childWithOutput = mocks.Stub<IPipingCalculationItem>();
            childWithOutput.Stub(c => c.HasOutput).Return(true);
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            group.Children.Add(childWithoutOutput);
            group.Children.Add(childWithOutput);

            // Call
            var hasOutput = group.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
            mocks.VerifyAll();
        }

        [Test]
        public void HasOutput_HasChildrenAllWithoutOutput_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var child1WithoutOutput = mocks.Stub<IPipingCalculationItem>();
            child1WithoutOutput.Stub(c => c.HasOutput).Return(false);

            var child2WithoutOutput = mocks.Stub<IPipingCalculationItem>();
            child2WithoutOutput.Stub(c => c.HasOutput).Return(false);
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            group.Children.Add(child1WithoutOutput);
            group.Children.Add(child2WithoutOutput);

            // Call
            var hasOutput = group.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
            mocks.VerifyAll();
        }
    }
}