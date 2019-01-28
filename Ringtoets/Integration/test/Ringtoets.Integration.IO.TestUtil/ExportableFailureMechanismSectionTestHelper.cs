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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanismSection"/>
    /// </summary>
    public static class ExportableFailureMechanismSectionTestHelper
    {
        /// <summary>
        /// Asserts a collection of <see cref="ExportableFailureMechanismSection"/>
        /// against a collection of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="expectedSections">The expected collection of <see cref="FailureMechanismSection"/>.</param>
        /// <param name="actualSections">The actual <see cref="ExportableFailureMechanismSection"/> to assert against.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The number of sections between <paramref name="expectedSections"/> and <paramref name="actualSections"/>
        /// do not match. </item>
        /// <item>The geometry of any the sections are not equal.</item>
        /// </list></exception>
        public static void AssertExportableFailureMechanismSections(IEnumerable<FailureMechanismSection> expectedSections,
                                                                    IEnumerable<ExportableFailureMechanismSection> actualSections)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, actualSections.Count());

            double expectedStartDistance = 0;
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                FailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                ExportableFailureMechanismSection actualSection = actualSections.ElementAt(i);

                double expectedEndDistance = expectedStartDistance + Math2D.Length(expectedSection.Points);

                Assert.AreEqual(expectedStartDistance, actualSection.StartDistance);
                Assert.AreEqual(expectedEndDistance, actualSection.EndDistance);
                CollectionAssert.AreEqual(expectedSection.Points, actualSection.Geometry);

                expectedStartDistance = expectedEndDistance;
            }
        }
    }
}