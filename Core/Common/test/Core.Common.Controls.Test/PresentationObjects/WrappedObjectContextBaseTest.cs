﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test.PresentationObjects
{
    [TestFixture]
    public class WrappedObjectContextBaseTest
    {
        [Test]
        public void Constructor_ValidWrappedObjectInstance_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            // Call
            var context = new SimpleWrappedObjectContext<object>(sourceObject);

            // Assert
            Assert.IsInstanceOf<IEquatable<WrappedObjectContextBase<object>>>(context);
            Assert.AreSame(sourceObject, context.WrappedData);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InputArgumentIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleWrappedObjectContext<object>(null);

            // Assert
            const string expectedMessage = "Wrapped data of context cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            var context = new SimpleWrappedObjectContext<object>(sourceObject);

            // Call
            var isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            var context = new SimpleWrappedObjectContext<object>(sourceObject);

            // Call
            var isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            var context1 = new SimpleWrappedObjectContext<object>(sourceObject);
            var context2 = new SimpleWrappedObjectContext<IEnumerable<object>>(Enumerable.Empty<object>());

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedData_ReturnFalse()
        {
            // Setup
            var sourceObj1 = new object();
            var sourceObj2 = new object();
            var sourceObject1 = new SimpleEquatable(sourceObj1);
            var sourceObject2 = new SimpleEquatable(sourceObj2);

            // Precondition:
            Assert.IsFalse(sourceObject1.Equals(sourceObject2));

            var context1 = new SimpleWrappedObjectContext<SimpleEquatable>(sourceObject1);
            object context2 = new SimpleWrappedObjectContext<SimpleEquatable>(sourceObject2);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithDifferentContextType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            var context1 = new SimpleWrappedObjectContext<object>(sourceObject);
            object context2 = new AnotherSimpleWrappedObjectContext<object>(sourceObject);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context1.Equals(context2);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithSameWrappedData_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceObject = mocks.Stub<object>();
            mocks.ReplayAll();

            var context1 = new SimpleWrappedObjectContext<object>(sourceObject);
            object context2 = new SimpleWrappedObjectContext<object>(sourceObject);

            // Call
            var isEqual1 = context1.Equals(context2);
            var isEqual2 = context1.Equals(context2);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);

            mocks.VerifyAll();
        }

        private class SimpleWrappedObjectContext<T> : WrappedObjectContextBase<T>
        {
            public SimpleWrappedObjectContext(T wrappedData) : base(wrappedData) {}
        }

        private class AnotherSimpleWrappedObjectContext<T> : WrappedObjectContextBase<T>
        {
            public AnotherSimpleWrappedObjectContext(T wrappedData) : base(wrappedData) { }
        }

        private class SimpleEquatable : IEquatable<SimpleEquatable>
        {
            private readonly object source;

            public SimpleEquatable(object equalitySource)
            {
                source = equalitySource;
            }

            public override bool Equals(object obj)
            {
                return Equals((SimpleEquatable) obj);
            }

            public override int GetHashCode()
            {
                return source.GetHashCode();
            }

            public bool Equals(SimpleEquatable other)
            {
                return source.Equals(other.source);
            }
        }
    }
}