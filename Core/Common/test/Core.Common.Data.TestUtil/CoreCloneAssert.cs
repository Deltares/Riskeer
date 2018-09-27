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
using System.Collections;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;

namespace Core.Common.Data.TestUtil
{
    /// <summary>
    /// Class for asserting whether two objects are clones.
    /// </summary>
    public static class CoreCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones. Some general clone assertions are performed, followed by the type specific
        /// assertions (provided via <paramref name="typeSpecificAsserts"/>).
        /// </summary>
        /// <typeparam name="T">The type of the objects to assert.</typeparam>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <param name="typeSpecificAsserts">The action for performing the <typeparamref name="T"/>
        /// specific assertions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeSpecificAsserts"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreObjectClones<T>(T original, object clone, Action<T, T> typeSpecificAsserts)
        {
            if (typeSpecificAsserts == null)
            {
                throw new ArgumentNullException(nameof(typeSpecificAsserts));
            }

            if (original == null && clone == null)
            {
                return;
            }

            Assert.IsNotNull(original);
            Assert.IsInstanceOf<T>(clone);
            Assert.AreNotSame(original, clone);

            var observable = original as IObservable;
            if (observable != null)
            {
                Assert.AreNotSame(observable.Observers, ((IObservable) clone).Observers);
            }

            typeSpecificAsserts(original, (T) clone);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones. Some general clone assertions are performed, followed by the type specific
        /// assertions on a per element basis (provided via <paramref name="typeSpecificAsserts"/>).
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumerations to assert.</typeparam>
        /// <param name="original">The original enumeration.</param>
        /// <param name="clone">The cloned enumeration.</param>
        /// <param name="typeSpecificAsserts">The action for performing the <typeparamref name="T"/>
        /// specific assertions on a per element basis.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeSpecificAsserts"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreEnumerationClones<T>(IEnumerable<T> original, object clone, Action<T, T> typeSpecificAsserts)
        {
            if (typeSpecificAsserts == null)
            {
                throw new ArgumentNullException(nameof(typeSpecificAsserts));
            }

            if (original == null && clone == null)
            {
                return;
            }

            Assert.IsNotNull(original);
            Assert.IsInstanceOf<IEnumerable<T>>(clone);
            Assert.AreNotSame(original, clone);

            var observable = original as IObservable;
            if (observable != null)
            {
                Assert.AreNotSame(observable.Observers, ((IObservable) clone).Observers);
            }

            CollectionAssert.AreEqual(original, (IEnumerable<T>) clone, new AreClonesComparer<T>(typeSpecificAsserts));
        }

        private class AreClonesComparer<T> : IComparer
        {
            private readonly Action<T, T> typeSpecificAsserts;

            public AreClonesComparer(Action<T, T> typeSpecificAsserts)
            {
                this.typeSpecificAsserts = typeSpecificAsserts;
            }

            public int Compare(object x, object y)
            {
                AreObjectClones((T) x, y, typeSpecificAsserts);

                return 0;
            }
        }
    }
}