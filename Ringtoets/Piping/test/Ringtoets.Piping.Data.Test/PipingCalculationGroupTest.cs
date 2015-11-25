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
            Assert.AreEqual("Berekening groep", group.Name);
            Assert.IsFalse(group.HasOutput);
            CollectionAssert.IsEmpty(group.Children);
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
            CollectionAssert.DoesNotContain(group.Children, calculation);
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