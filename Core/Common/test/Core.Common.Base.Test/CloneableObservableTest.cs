﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

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
            var mocks = new MockRepository();
            var original = new TestCloneableObservable();
            var observer = mocks.Stub<IObserver>();

            original.Attach(observer);

            mocks.ReplayAll();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, (o, c) =>
            {
                CollectionAssert.IsEmpty(c.Observers);
            });

            mocks.VerifyAll();
        }

        private class TestCloneableObservable : CloneableObservable {}

    }
}