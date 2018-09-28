// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class ClearResultsTest
    {
        [Test]
        public void ConstructorTest_ValidArguments_ExpectedValues()
        {
            // Setup
            var changedObjectsArray = new IObservable[0];
            var removedObjectsArray = new object[0];

            // Call
            var results = new ClearResults(changedObjectsArray, removedObjectsArray);

            // Assert
            Assert.AreSame(changedObjectsArray, results.ChangedObjects);
            Assert.AreSame(removedObjectsArray, results.RemovedObjects);
        }

        [Test]
        public void Constructor_ChangedObjectsNull_ThrowArgumentNullException()
        {
            // Setup
            var removedObjectsArray = new object[0];

            // Call
            TestDelegate call = () => new ClearResults(null, removedObjectsArray);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("changedObjects", paramName);
        }

        [Test]
        public void Constructor_RemovedObjectsNull_ThrowArgumentNullException()
        {
            // Setup
            var changedObjectsArray = new IObservable[0];

            // Call
            TestDelegate call = () => new ClearResults(changedObjectsArray, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("removedObjects", paramName);
        }
    }
}