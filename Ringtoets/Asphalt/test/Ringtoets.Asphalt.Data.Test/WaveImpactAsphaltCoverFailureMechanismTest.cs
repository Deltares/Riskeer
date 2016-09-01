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

namespace Ringtoets.Asphalt.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Golfklappen op asfaltbekleding", failureMechanism.Name);
            Assert.AreEqual("AGK", failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
        }

        [Test]
        public void AddSection_WithSection_AddedWaveImpactAsphaltCoverFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<WaveImpactAsphaltCoverFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }

        [Test]
        public void CleanAllSections_WithSection_RemoveSectionResults()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }
    }
}