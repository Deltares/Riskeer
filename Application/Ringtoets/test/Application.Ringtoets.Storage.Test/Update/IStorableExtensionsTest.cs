// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class IStorableExtensionsTest
    {
        [Test]
        [TestCase(0, true)]
        [TestCase(-1, true)]
        [TestCase(Int32.MinValue, true)]
        [TestCase(1, false)]
        [TestCase(5, false)]
        [TestCase(Int32.MaxValue, false)]
        public void IsNew_DifferentIds_ExpectedResult(int val, bool isNew)
        {
            // Setup
            var mocks = new MockRepository();
            var storable = mocks.StrictMock<IStorable>();
            storable.Expect(s => s.StorageId).Return(val);
            mocks.ReplayAll();

            // Call
            var result = storable.IsNew();

            // Assert
            Assert.AreEqual(isNew, result);
        }
    }
}