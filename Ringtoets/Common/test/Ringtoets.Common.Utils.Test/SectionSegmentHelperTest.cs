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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Utils.Test
{
    [TestFixture]
    public class SectionSegmentHelperTest
    {
        [Test]
        public void MakeSectionSegments_Always_ReturnSectionSegmentsFromFailureMechanismSection()
        {
            // Setup
            FailureMechanismSection[] failureMechanismSections = 
            {
                new FailureMechanismSection(string.Empty, new[] { new Point2D(0, 0) }),
                new FailureMechanismSection(string.Empty, new[] { new Point2D(1, 1) }),
                new FailureMechanismSection(string.Empty, new[] { new Point2D(2, 2) }),
                new FailureMechanismSection(string.Empty, new[] { new Point2D(3, 3) })
            };

            // Call
            var segmentSections = SectionSegmentsHelper.MakeSectionSegments(failureMechanismSections);

            // Assert
            Assert.AreEqual(failureMechanismSections.Length, segmentSections.Length);
            CollectionAssert.AreEqual(failureMechanismSections, segmentSections.Select(ss => ss.Section));
        }
    }
}