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

using System;

using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Controls.Test.PresentationObjects
{
    [TestFixture]
    public class ObservableWrappedObjectContextBaseTest
    {
        [Test]
        public void Constructor_ValidWrappedObjectInstance_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            // Call
            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsInstanceOf<IEquatable<ObservableWrappedObjectContextBase<IObservable>>>(context);
            Assert.AreSame(sourceObject, context.WrappedData);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InputArgumentIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleObservableWrappedObjectContext<IObservable>(null);

            // Assert
            const string expectedMessage = "Wrapped data of context cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);

            // Call
            var isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherValueType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var context1 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            var context2 = new SimpleObservableWrappedObjectContext<ObservableList<object>>(new ObservableList<object>());

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherValueOfSameType_ReturnFalse()
        {
            // Setup
            var sourceObj1 = new object();
            var sourceObj2 = new object();
            var sourceObject1 = new SimpleObservable(sourceObj1);
            var sourceObject2 = new SimpleObservable(sourceObj2);

            // Precondition:
            Assert.IsFalse(sourceObject1.Equals(sourceObject2));

            var context1 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject1);
            object context2 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject2);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);

            // Call
            var isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherEqualInstance_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var context1 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            object context2 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context1.Equals(context2);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var sourceObject = new object();
            var sourceObject1 = new SimpleObservable(sourceObject);
            var sourceObject2 = new SimpleObservable(sourceObject);

            var context1 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject1);
            object context2 = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject2);

            // Precondition:
            Assert.AreEqual(context1, context2);

            // Call
            var hashCode1 = context1.GetHashCode();
            var hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void NotifyObservers_ObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);

            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            context.Attach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect call UpdateObserver on 'observer'
        }

        [Test]
        public void NotifyObservers_ObserverDetached_ObserverIsNoLongerNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);

            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            context.Attach(observer);
            context.Detach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no UpdateObserver calls on 'observer'
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToWrappedData_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);
            sourceObject.Attach(observer);
            
            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            
            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect call UpdateObserver on 'observer'
        }

        [Test]
        public void GivenContextWithAttachedObserver_WhenWrappedDataNotifiesObservers_ThenObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);

            var context = new SimpleObservableWrappedObjectContext<IObservable>(sourceObject);
            context.Attach(observer);

            // Call
            sourceObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect call UpdateObserver on 'observer'
        }

        private class SimpleObservableWrappedObjectContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
        {
            public SimpleObservableWrappedObjectContext(T wrappedData) : base(wrappedData) {}
        }

        private class SimpleObservable : Observable, IEquatable<SimpleObservable>
        {
            private readonly object source;
            public SimpleObservable(object equalitySource)
            {
                source = equalitySource;
            }

            public bool Equals(SimpleObservable other)
            {
                return source.Equals(other.source);
            }

            public override bool Equals(object obj)
            {
                return Equals((SimpleObservable)obj);
            }

            public override int GetHashCode()
            {
                return source.GetHashCode();
            }
        }
    }
}