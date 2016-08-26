﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie buitentalud", failureMechanism.Name);
            Assert.AreEqual("GEBU", failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual("Berekeningen", failureMechanism.HydraulicBoundariesCalculationGroup.Name);
            Assert.IsFalse(failureMechanism.HydraulicBoundariesCalculationGroup.IsNameEditable);
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundariesCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations);
        }

        [Test]
        public void AddSection_WithSection_AddedGrassCoverErosionOutwardsFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<GrassCoverErosionOutwardsFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }

        [Test]
        public void CleanAllSections_WithSection_RemoveSectionResults()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
        
        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_ValidHydraulicBoundaryLocations_SetsGrassCoverErosionOutwardsHydraulicBoundaryLocations()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1,"",2,3);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(new[] { hydraulicBoundaryLocation });

           // Assert
            Assert.AreEqual(1, failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.Count);
            var firstGrassCoverErosionOutwardsHydraulicBoundaryLocation = failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.First();
            Assert.AreSame(hydraulicBoundaryLocation, firstGrassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation);
        }

        [Test]
        public void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ClearsGrassCoverErosionOutwardsHydraulicBoundaryLocations()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1,"",2,3);
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(new[] { hydraulicBoundaryLocation });

            // Precondition
            Assert.AreEqual(1, failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.Count);
            var firstGrassCoverErosionOutwardsHydraulicBoundaryLocation = failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.First();
            Assert.AreSame(hydraulicBoundaryLocation, firstGrassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation);

            // Call
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(null);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations);
        }
    }
}