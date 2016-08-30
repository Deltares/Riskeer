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

namespace Ringtoets.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Stabiliteit steenzetting", failureMechanism.Name);
            Assert.AreEqual("ZST", failureMechanism.Code);

            Assert.AreEqual("Hydraulische randvoorwaarden", failureMechanism.HydraulicBoundariesCalculationGroup.Name);
            Assert.IsFalse(failureMechanism.HydraulicBoundariesCalculationGroup.IsNameEditable);
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundariesCalculationGroup.Children);

            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);

            Assert.IsInstanceOf<GeneralStabilityStoneCoverWaveConditionsInput>(failureMechanism.GeneralInput);
        }

        [Test]
        public void AddSection_WithSection_AddedStabilityStoneCoverFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<StabilityStoneCoverFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }

        [Test]
        public void CleanAllSections_WithSection_RemoveSectionResults()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
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