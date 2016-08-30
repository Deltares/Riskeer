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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaterLevelLocationsContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>();

            // Call
            var presentationObject = new GrassCoverErosionOutwardsWaterLevelLocationsContext(assessmentSectionMock, locations);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IAssessmentSection>>(presentationObject);
            Assert.AreSame(assessmentSectionMock, presentationObject.WrappedData);
            Assert.AreSame(locations, presentationObject.Locations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_GrassCoverErosionOutwardsHydraulicBoundaryLocationsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaterLevelLocationsContext(assessmentSectionMock, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("grassCoverErosionOutwardsHydraulicBoundaryLocations", paramName);
        }
    }
}