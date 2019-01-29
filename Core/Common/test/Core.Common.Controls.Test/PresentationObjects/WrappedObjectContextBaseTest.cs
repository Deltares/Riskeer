// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Test.PresentationObjects
{
    [TestFixture]
    public class WrappedObjectContextBaseTest
    {
        [Test]
        public void Constructor_ValidWrappedObjectInstance_ExpectedValues()
        {
            // Setup
            var sourceObject = new object();

            // Call
            var context = new SimpleWrappedObjectContext<object>(sourceObject);

            // Assert
            Assert.IsInstanceOf<IEquatable<WrappedObjectContextBase<object>>>(context);
            Assert.AreSame(sourceObject, context.WrappedData);
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

        [TestFixture]
        private class WrappedObjectContextEqualsTest : EqualsTestFixture<WrappedObjectContextBase<SimpleEquatable>>
        {
            private static readonly object sourceObject = new object();

            protected override WrappedObjectContextBase<SimpleEquatable> CreateObject()
            {
                var wrappedObject = new SimpleEquatable(sourceObject);
                return new SimpleWrappedObjectContext<SimpleEquatable>(wrappedObject);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                var wrappedObject = new SimpleEquatable(sourceObject);
                yield return new TestCaseData(new AnotherSimpleWrappedObjectContext<SimpleEquatable>(wrappedObject))
                    .SetName("ContextType");

                var differentWrappedObject = new SimpleEquatable(new object());
                yield return new TestCaseData(new SimpleWrappedObjectContext<SimpleEquatable>(differentWrappedObject))
                    .SetName("Wrapped data");
            }
        }

        private class SimpleWrappedObjectContext<T> : WrappedObjectContextBase<T>
        {
            public SimpleWrappedObjectContext(T wrappedData) : base(wrappedData) {}
        }

        private class AnotherSimpleWrappedObjectContext<T> : WrappedObjectContextBase<T>
        {
            public AnotherSimpleWrappedObjectContext(T wrappedData) : base(wrappedData) {}
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