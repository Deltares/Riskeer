// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;

namespace Core.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class CloneAssertTest
    {
        [Test]
        public void AreClones_ObjectsWithTypeSpecificAssertsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones(new object(), new object(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AreClones_OriginalObjectAndClonedObjectBothNull_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones<object>(null, null, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreClones_OriginalObjectNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones<object>(null, new object(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ClonedObjectNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones(new object(), null, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ClonedObjectOfOtherDataType_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones(1.0, 1, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_OriginalObjectAndClonedObjectAreSame_ThrowsAssertionException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate test = () => CloneAssert.AreClones(o, o, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ObjectsWithTypeSpecificAsserts_TypeSpecificAssertsCalled()
        {
            // Setup
            var counter = 0;

            // Call
            CloneAssert.AreClones(new object(), new object(), (original, clone) => counter++);

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void AreClones_EnumerationsWithTypeSpecificAssertsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones(Enumerable.Empty<object>(), Enumerable.Empty<object>(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AreClones_OriginalEnumerationAndClonedEnumerationBothNull_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones<IEnumerable<object>>(null, null, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreClones_OriginalEnumerationNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones<IEnumerable<object>>(null, Enumerable.Empty<object>(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ClonedEnumerationNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones(Enumerable.Empty<object>(), null, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ClonedEnumerationOfOtherDataType_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CloneAssert.AreClones<int>(Enumerable.Empty<int>(), Enumerable.Empty<double>(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_OriginalEnumerationAndClonedEnumerationAreSame_ThrowsAssertionException()
        {
            // Setup
            IEnumerable<object> o = Enumerable.Empty<object>();

            // Call
            TestDelegate test = () => CloneAssert.AreClones(o, o, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreClones_ObjectsWithTypeSpecificAsserts_TypeSpecificAssertsCalledForEachElement()
        {
            // Setup
            var counter = 0;

            // Call
            CloneAssert.AreClones<object>(new[]
            {
                new object(),
                new object()
            }, new[]
            {
                new object(),
                new object()
            }, (original, clone) => counter++);

            // Assert
            Assert.AreEqual(2, counter);
        }
    }
}