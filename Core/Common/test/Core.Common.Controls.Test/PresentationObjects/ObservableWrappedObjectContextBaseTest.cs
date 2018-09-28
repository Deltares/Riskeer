// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
            var sourceObject = new SimpleObservable(new object());

            // Call
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsInstanceOf<WrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(sourceObject, context.WrappedData);
            CollectionAssert.IsEmpty(context.Observers);
        }

        [Test]
        public void Observers_ValidWrappedObjectInstance_ReturnsObserversOfWrappedObject()
        {
            // Setup
            var sourceObject = new SimpleObservable(new object());
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

            // Call & Assert
            Assert.AreSame(sourceObject.Observers, context.Observers);
        }

        [Test]
        public void NotifyObservers_ObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

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
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

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
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

            sourceObject.Attach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect call UpdateObserver on 'observer'
        }

        [Test]
        public void GivenContextWithAttachedObserver_WhenWrappedDataNotifiesObservers_ThenObserverIsNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var equalitySource = new object();
            var sourceObject = new SimpleObservable(equalitySource);
            var context = new SimpleObservableWrappedObjectContext(sourceObject);

            context.Attach(observer);

            // When
            sourceObject.NotifyObservers();

            // Then
            mocks.VerifyAll(); // Expect call UpdateObserver on 'observer'
        }

        private class SimpleObservableWrappedObjectContext : ObservableWrappedObjectContextBase<IObservable>
        {
            public SimpleObservableWrappedObjectContext(IObservable wrappedData) : base(wrappedData) {}
        }

        private class SimpleObservable : Observable, IEquatable<SimpleObservable>
        {
            private readonly object source;

            public SimpleObservable(object equalitySource)
            {
                source = equalitySource;
            }

            public override bool Equals(object obj)
            {
                return Equals((SimpleObservable) obj);
            }

            public override int GetHashCode()
            {
                return source.GetHashCode();
            }

            public bool Equals(SimpleObservable other)
            {
                return source.Equals(other.source);
            }
        }
    }
}