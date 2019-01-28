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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class SelectableHydraulicBoundaryLocationHelperTest
    {
        [Test]
        public void GetSortedSelectableHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var point2D = new Point2D(0.0, 0.0);

            // Call
            TestDelegate call = () => SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(null, point2D);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void GetSortedSelectableHydraulicBoundaryLocations_WithLocationsReferencePointNull_ReturnLocationsSortedById()
        {
            // Setup
            var hydraulicBoundaryLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "A", 0, 1),
                new HydraulicBoundaryLocation(4, "C", 0, 2),
                new HydraulicBoundaryLocation(3, "D", 0, 3),
                new HydraulicBoundaryLocation(2, "B", 0, 4)
            };

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations =
                SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, null);

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null))
                                          .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, selectableHydraulicBoundaryLocations);
        }

        [Test]
        public void GetSortedSelectableHydraulicBoundaryLocations_WithLocationsAndPoint_ReturnLocationsSortedByDistanceThenById()
        {
            // Setup
            var hydraulicBoundaryLocations = new[]
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(4, "E", 0, 500),
                new HydraulicBoundaryLocation(6, "F", 0, 100),
                new HydraulicBoundaryLocation(5, "D", 0, 200),
                new HydraulicBoundaryLocation(3, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var point2D = new Point2D(0.0, 0.0);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations =
                SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, point2D);

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, point2D))
                                          .OrderBy(hbl => hbl.Distance)
                                          .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);

            CollectionAssert.AreEqual(expectedList, selectableHydraulicBoundaryLocations);
        }
    }
}