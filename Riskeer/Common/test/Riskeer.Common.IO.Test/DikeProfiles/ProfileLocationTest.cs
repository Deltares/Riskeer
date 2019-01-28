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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.DikeProfiles;

namespace Ringtoets.Common.IO.Test.DikeProfiles
{
    [TestFixture]
    public class ProfileLocationTest
    {
        [Test]
        public void Constructor_InitializedWithValidValues_CorrectProperties()
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);
            var profileLocation = new ProfileLocation("id", "name", 1.1, referencePoint);

            // Assert
            Assert.AreEqual("id", profileLocation.Id);
            Assert.AreEqual("name", profileLocation.Name);
            Assert.AreEqual(1.1, profileLocation.Offset);
            Assert.AreEqual(referencePoint, profileLocation.Point);
        }

        [Test]
        public void Constructor_InitializedWithValidValues_CorrectPropertyTypes()
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);
            var profileLocation = new ProfileLocation("id", null, 1.1, referencePoint);

            // Assert
            Assert.IsInstanceOf(typeof(string), profileLocation.Id);
            Assert.IsNull(profileLocation.Name);
            Assert.IsInstanceOf(typeof(double), profileLocation.Offset);
            Assert.IsInstanceOf(typeof(Point2D), profileLocation.Point);
        }

        [Test]
        public void Constructor_InitializedWithNullId_ThrowArgumentException()
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);

            // Call
            TestDelegate call = () => new ProfileLocation(null, "aNAME", 1.1, referencePoint);

            // Assert
            const string expectedMessage = "De locatie parameter 'ID' heeft geen waarde.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Constructor_InitializedWithNullPoint_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new ProfileLocation("anID", "aNAME", 1.1, null);

            // Assert
            const string expectedMessage = "De locatie heeft geen coördinaten.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("a 1")]
        [TestCase("a#1")]
        [TestCase("   ")]
        [TestCase("*&(%&$")]
        public void Constructor_InitializedWithInvalidId_ThrowArgumentException(string id)
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);

            // Call
            TestDelegate call = () => new ProfileLocation(id, "aNAME", 1.1, referencePoint);

            // Assert
            const string expectedMessage = "De locatie parameter 'ID' mag uitsluitend uit letters en cijfers bestaan.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Constructor_InitializedWithInvalidId_ThrowArgumentException(double x0)
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);

            // Call
            TestDelegate call = () => new ProfileLocation("anID", "aNAME", x0, referencePoint);

            // Assert
            const string expectedMessage = "De locatie parameter 'X0' bevat een ongeldig getal.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }
    }
}