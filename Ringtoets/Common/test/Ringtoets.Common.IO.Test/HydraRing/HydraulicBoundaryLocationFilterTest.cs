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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraulicBoundaryLocationFilterTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryLocationFilter");

        [Test]
        public void Constructor_InvalidCharactersInPath_ThrowsCriticalFileReadException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationFilter(">");

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_SettingsNotExisting_ThrowsCriticalFileReadException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationFilter(Path.Combine(testDataPath, "notExisting"));

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(long.MinValue)]
        [TestCase(303192)]
        [TestCase(303196)]
        [TestCase(602961)]
        [TestCase(603017)]
        [TestCase(long.MaxValue)]
        public void ShouldInclude_NameNotInFilterSet_ReturnsTrue(long id)
        {
            // Setup
            var filter = new HydraulicBoundaryLocationFilter(Path.Combine(testDataPath, "hrd.config.sqlite"));

            // Call
            bool shouldBeIncluded = filter.ShouldInclude(id);

            // Assert
            Assert.IsTrue(shouldBeIncluded);
        }

        [Test]
        [TestCase(303193)]
        [TestCase(603075)]
        public void ShouldInclude_NameInFilterSet_ReturnsFalse(long id)
        {
            // Setup
            var filter = new HydraulicBoundaryLocationFilter(Path.Combine(testDataPath, "hrd.config.sqlite"));

            // Call
            bool shouldBeIncluded = filter.ShouldInclude(id);

            // Assert
            Assert.IsFalse(shouldBeIncluded);
        }
    }
}