// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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