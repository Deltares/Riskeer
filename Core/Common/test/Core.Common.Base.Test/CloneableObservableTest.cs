// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Data.TestUtil;
using NSubstitute;
using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class CloneableObservableTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var observable = new TestCloneableObservable();

            // Assert
            Assert.IsInstanceOf<Observable>(observable);
            Assert.IsInstanceOf<ICloneable>(observable);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithEmptyObservers()
        {
            // Setup
            var original = new TestCloneableObservable();
            var observer = Substitute.For<IObserver>();

            original.Attach(observer);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, (o, c) =>
            {
                CollectionAssert.IsEmpty(c.Observers);
            });
        }

        private class TestCloneableObservable : CloneableObservable {}
    }
}