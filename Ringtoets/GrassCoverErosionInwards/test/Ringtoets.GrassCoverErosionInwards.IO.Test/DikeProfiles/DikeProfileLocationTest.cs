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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileLocationTest
    {
        [Test]
        [TestCase("Id", null, "name", 0.0)]
        [TestCase("Naam", "id", null, 0.0)]
        public void Constructor_InitializedWithNullParameter_ThrownArgumentException(string parameterName, string idValue, string nameValue, double x0Value)
        {
            // Call
            TestDelegate call = () => new DikeProfileLocation(idValue, nameValue, x0Value, new Point2D(0.0, 0.0));

            // Assert
            var expectedMessage = string.Format("Fout bij het aanmaken van een dijk profiel locatie: {0} is ongeldig.", parameterName);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InitializedWithNullPoint_ThrownArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfileLocation("anID", "aNAME", 0.0, null);

            // Assert
            var expectedMessage = "Fout bij het aanmaken van een dijk profiel locatie: Punt is ongeldig.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InitializedWithInvalidX0_ThrownArgumentException()
        {
            // Call
            TestDelegate call = () => new DikeProfileLocation("id", "name", double.NaN, new Point2D(0.0, 0.0));

            // Assert
            var expectedMessage = "Fout bij het aanmaken van een dijk profiel locatie: X0 is ongeldig.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InitializedWithValidValues_CorrectProperties()
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);
            DikeProfileLocation dikeProfileLocation = new DikeProfileLocation("id", "name", 1.1, referencePoint);

            // Assert
            Assert.AreEqual("id", dikeProfileLocation.Id);
            Assert.AreEqual("name", dikeProfileLocation.Name);
            Assert.AreEqual(1.1, dikeProfileLocation.X0);
            Assert.AreEqual(referencePoint, dikeProfileLocation.Point);
        }
    }
}