using Core.Common.Utils.Events;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Events
{
    [TestFixture]
    public class NotifyCollectionChangeEventArgsTest
    {
        [Test]
        public void ParameteredConstructor_PassArgumentValuesNotPassingOldElementValue_EventArgsPropertiesSet()
        {
            // Setup
            var changeAction = NotifyCollectionChangeAction.Remove;
            var elementValue = new object();
            var currentIndex = 1;
            var previousIndex = 2;

            // Call
            var args = new NotifyCollectionChangeEventArgs(changeAction, elementValue, currentIndex, previousIndex);

            // Assert
            Assert.AreEqual(changeAction, args.Action);
            Assert.AreEqual(currentIndex, args.Index);
            Assert.AreEqual(elementValue, args.Item);
            Assert.AreEqual(previousIndex, args.OldIndex);
            Assert.IsNull(args.OldItem);
            Assert.IsFalse(args.Cancel);
        }

        [Test]
        public void ParameteredConstructor_PassArgumentValuesWithOldElementValue_EventArgsPropertiesSet()
        {
            // Setup
            var changeAction = NotifyCollectionChangeAction.Remove;
            var elementValue = new object();
            var oldElementValue = new object();
            var currentIndex = 1;
            var previousIndex = 2;

            // Call
            var args = new NotifyCollectionChangeEventArgs(changeAction, elementValue, currentIndex, previousIndex, oldElementValue);

            // Assert
            Assert.AreEqual(changeAction, args.Action);
            Assert.AreEqual(currentIndex, args.Index);
            Assert.AreEqual(elementValue, args.Item);
            Assert.AreEqual(previousIndex, args.OldIndex);
            Assert.AreEqual(oldElementValue, args.OldItem);
            Assert.IsFalse(args.Cancel);
        }

        [Test]
        public void CreateCollectionResetArgs_ReturnInitializedEventArguments()
        {
            // Call
            var args = NotifyCollectionChangeEventArgs.CreateCollectionResetArgs();

            // Assert
            Assert.AreEqual(NotifyCollectionChangeAction.Reset, args.Action);
            Assert.AreEqual(-1, args.Index);
            Assert.IsNull(args.Item);
            Assert.AreEqual(-1, args.OldIndex);
            Assert.IsNull(args.OldItem);
            Assert.IsFalse(args.Cancel);
        }

        [Test]
        public void CreateCollectionAddArgs_ReturnInitializedEventArguments()
        {
            // Setup
            var element = new object();
            var index = 1;

            // Call
            var args = NotifyCollectionChangeEventArgs.CreateCollectionAddArgs(element, index);

            // Assert
            Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
            Assert.AreEqual(index, args.Index);
            Assert.AreSame(element, args.Item);
            Assert.AreEqual(-1, args.OldIndex);
            Assert.IsNull(args.OldItem);
            Assert.IsFalse(args.Cancel);
        }
    }
}