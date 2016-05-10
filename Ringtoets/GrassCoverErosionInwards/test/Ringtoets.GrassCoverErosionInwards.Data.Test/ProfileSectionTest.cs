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
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class ProfileSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var begin = new Point2D(1.1, 2.2);
            var end = new Point2D(3.3, 4.4);

            // Call
            ProfileSection profileSection = new ProfileSection(begin, end);

            // Assert
            Assert.IsNotNull(profileSection);
            Assert.AreEqual(begin, profileSection.StartingPoint);
            Assert.AreEqual(end, profileSection.EndingPoint);
        }

        [Test]
        public void Constructor_StartingPointNull_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new ProfileSection(null, new Point2D(3.3, 4.4));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("startingPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_EndingPointNull_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new ProfileSection(new Point2D(3.3, 4.4), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("endingPoint", exception.ParamName);
        }
    }
}