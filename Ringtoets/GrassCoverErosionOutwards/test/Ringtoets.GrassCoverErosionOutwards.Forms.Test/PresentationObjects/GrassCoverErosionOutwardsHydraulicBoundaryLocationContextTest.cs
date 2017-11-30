// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocations_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate test = () => new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            // Call
            var presentationObject = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocation, locations);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<HydraulicBoundaryLocation>>(presentationObject);
            Assert.AreSame(locations, presentationObject.HydraulicBoundaryLocations);
            Assert.AreSame(hydraulicBoundaryLocation, presentationObject.WrappedData);
        }

        [TestFixture]
        private class GrassCoverErosionOutwardsHydraulicBoundaryLocationContextEqualsTest
            : EqualsGuidelinesTestFixture<TestGrassCoverErosionOutwardsLocationContext, DerivedTestGrassCoverErosionOutwardsLocationContext>
        {
            private static readonly ObservableList<HydraulicBoundaryLocation> hydraulicLocations = new ObservableList<HydraulicBoundaryLocation>();
            private static readonly HydraulicBoundaryLocation hydraulicLocation = new TestHydraulicBoundaryLocation();

            protected override TestGrassCoverErosionOutwardsLocationContext CreateObject()
            {
                return new TestGrassCoverErosionOutwardsLocationContext(hydraulicLocation, hydraulicLocations);
            }

            protected override DerivedTestGrassCoverErosionOutwardsLocationContext CreateDerivedObject()
            {
                return new DerivedTestGrassCoverErosionOutwardsLocationContext(hydraulicLocation, hydraulicLocations);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new TestGrassCoverErosionOutwardsLocationContext(hydraulicLocation, new ObservableList<HydraulicBoundaryLocation>()))
                    .SetName("LocationsList");
                yield return new TestCaseData(new TestGrassCoverErosionOutwardsLocationContext(new TestHydraulicBoundaryLocation(), hydraulicLocations))
                    .SetName("Location");
            }
        }

        private class TestGrassCoverErosionOutwardsLocationContext : GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
        {
            public TestGrassCoverErosionOutwardsLocationContext(HydraulicBoundaryLocation location,
                                                                ObservableList<HydraulicBoundaryLocation> observable
            )
                : base(location, observable) {}
        }

        private class DerivedTestGrassCoverErosionOutwardsLocationContext : TestGrassCoverErosionOutwardsLocationContext
        {
            public DerivedTestGrassCoverErosionOutwardsLocationContext(HydraulicBoundaryLocation location,
                                                                       ObservableList<HydraulicBoundaryLocation> observable)
                : base(location, observable) {}
        }
    }
}