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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>();

            // Call
            TestDelegate test = () => new TestGrassCoverErosionOutwardsLocationContext(locations, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("grassCoverErosionOutwardsHydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(1, "name", 2.0, 3.0));
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            };

            // Call
            var presentationObject = new TestGrassCoverErosionOutwardsLocationContext(locations, grassCoverErosionOutwardsHydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>>>(presentationObject);
            Assert.AreSame(locations, presentationObject.WrappedData);
            Assert.AreSame(grassCoverErosionOutwardsHydraulicBoundaryLocation, presentationObject.GrassCoverErosionOutwardsHydraulicBoundaryLocation);
        }

        private class TestGrassCoverErosionOutwardsLocationContext : GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
        {
            public TestGrassCoverErosionOutwardsLocationContext(ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation> observable,
                                                                GrassCoverErosionOutwardsHydraulicBoundaryLocation location)
                : base(observable, location) {}
        }
    }
}