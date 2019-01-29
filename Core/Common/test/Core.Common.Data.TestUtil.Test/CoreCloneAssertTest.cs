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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;

namespace Core.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class CoreCloneAssertTest
    {
        [Test]
        public void AreObjectClones_TypeSpecificAssertsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(new object(), new object(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AreObjectClones_OriginalAndCloneBothNull_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones<object>(null, null, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreObjectClones_OriginalNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones<object>(null, new object(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreObjectClones_CloneNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(new object(), null, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreObjectClones_CloneOfOtherDataType_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(1.0, 1, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreObjectClones_OriginalAndCloneAreSame_ThrowsAssertionException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(o, o, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreObjectClones_TypeSpecificAsserts_TypeSpecificAssertsCalled()
        {
            // Setup
            var counter = 0;

            // Call
            CoreCloneAssert.AreObjectClones(new object(), new object(), (original, clone) => counter++);

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void AreObjectClones_ObservableObjectDifferentObservers_DoesNotThrow()
        {
            // Setup
            var cloneableObservable = new TestCloneableObservable();
            var clonedCloneableObservable = (TestCloneableObservable) cloneableObservable.Clone();

            // Precondition
            Assert.IsFalse(ReferenceEquals(cloneableObservable.Observers, clonedCloneableObservable.Observers));

            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(cloneableObservable, clonedCloneableObservable, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreObjectClones_ObservableObjectSameObservers_ThrowsAssertionException()
        {
            // Setup
            var observable = new TestObservable();
            var clonedObservable = (TestObservable) observable.Clone();

            // Precondition
            Assert.IsTrue(ReferenceEquals(observable.Observers, clonedObservable.Observers));

            // Call
            TestDelegate test = () => CoreCloneAssert.AreObjectClones(observable, clonedObservable, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_TypeSpecificAssertsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(Enumerable.Empty<object>(), Enumerable.Empty<object>(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AreEnumerationClones_OriginalAndCloneBothNull_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones<IEnumerable<object>>(null, null, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreEnumerationClones_OriginalNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones<IEnumerable<object>>(null, Enumerable.Empty<object>(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_CloneNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(Enumerable.Empty<object>(), null, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_CloneOfOtherDataType_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(Enumerable.Empty<int>(), Enumerable.Empty<double>(), (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_OriginalAndCloneAreSame_ThrowsAssertionException()
        {
            // Setup
            IEnumerable<object> o = Enumerable.Empty<object>();

            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(o, o, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_OriginalAndCloneDifferentLength_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(
                new[]
                {
                    new object(),
                    new object()
                }, new[]
                {
                    new object()
                },
                (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AreEnumerationClones_TypeSpecificAsserts_TypeSpecificAssertsCalledForEachElement()
        {
            // Setup
            var counter = 0;

            // Call
            CoreCloneAssert.AreEnumerationClones(
                new[]
                {
                    new object(),
                    new object()
                }, new[]
                {
                    new object(),
                    new object()
                },
                (original, clone) => counter++);

            // Assert
            Assert.AreEqual(2, counter);
        }

        [Test]
        public void AreEnumerationClones_ObservableListDifferentObservers_DoesNotThrow()
        {
            // Setup
            var cloneableObservableList = new CloneableObservableList<object>();
            var clonedCloneableObservableList = (CloneableObservableList<object>) cloneableObservableList.Clone();

            // Precondition
            Assert.IsFalse(ReferenceEquals(cloneableObservableList.Observers, clonedCloneableObservableList.Observers));

            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(cloneableObservableList, clonedCloneableObservableList, (original, clone) => {});

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AreEnumerationClones_ObservableListSameObservers_ThrowsAssertionException()
        {
            // Setup
            var observableList = new TestObservableList<object>();
            var clonedObservableList = (TestObservableList<object>) observableList.Clone();

            // Precondition
            Assert.IsTrue(ReferenceEquals(observableList.Observers, clonedObservableList.Observers));

            // Call
            TestDelegate test = () => CoreCloneAssert.AreEnumerationClones(observableList, clonedObservableList, (original, clone) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        private class TestObservable : Observable, ICloneable
        {
            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        private class TestObservableList<T> : ObservableList<T>, ICloneable
        {
            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        private class TestCloneableObservable : CloneableObservable {}
    }
}