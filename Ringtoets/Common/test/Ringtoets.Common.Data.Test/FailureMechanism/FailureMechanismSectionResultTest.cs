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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            var section = CreateSection();

            // Call
            var sectionResult = new FailureMechanismSectionResult(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
            CollectionAssert.IsEmpty(sectionResult.CalculationScenarios);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        private static FailureMechanismSection CreateSection()
        {
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var section = new FailureMechanismSection("test", points);
            return section;
        }
    }
}