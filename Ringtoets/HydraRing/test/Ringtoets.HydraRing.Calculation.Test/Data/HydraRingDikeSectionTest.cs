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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.Test.Data
{
    [TestFixture]
    public class HydraRingDikeSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingDikeSection = new HydraRingDikeSection(1, "Name", 2.2, 3.3, 4.4, 5.5, 6.6, 7.7);

            // Assert
            Assert.AreEqual(1, hydraRingDikeSection.SectionId);
            Assert.AreEqual("Name", hydraRingDikeSection.SectionName);
            Assert.AreEqual(2.2, hydraRingDikeSection.SectionBeginCoordinate);
            Assert.AreEqual(3.3, hydraRingDikeSection.SectionEndCoordinate);
            Assert.AreEqual(4.4, hydraRingDikeSection.SectionLength);
            Assert.AreEqual(5.5, hydraRingDikeSection.CrossSectionXCoordinate);
            Assert.AreEqual(6.6, hydraRingDikeSection.CrossSectionYCoordinate);
            Assert.AreEqual(7.7, hydraRingDikeSection.CrossSectionNormal);
        }
    }
}