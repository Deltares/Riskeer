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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileLocationTest
    {
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

        [Test]
        public void Constructor_InitializedWithValidValues_CorrectPropertyTypes()
        {
            // Setup
            var referencePoint = new Point2D(2.2, 3.3);
            DikeProfileLocation dikeProfileLocation = new DikeProfileLocation("id", null, 1.1, referencePoint);

            // Assert
            Assert.IsInstanceOf(typeof(string), dikeProfileLocation.Id);
            Assert.IsNull(dikeProfileLocation.Name);
            Assert.IsInstanceOf(typeof(double), dikeProfileLocation.X0);
            Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocation.Point);
        }
    }
}