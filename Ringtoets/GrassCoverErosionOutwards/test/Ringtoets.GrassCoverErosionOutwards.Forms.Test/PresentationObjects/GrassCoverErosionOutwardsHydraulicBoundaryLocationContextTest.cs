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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationContextTest
    {
        [Test]
        public void Constructor_NullHydraulicBoundaryLocation_ThrowsArgumentNullException()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            TestDelegate test = () => new TestGrassCoverErosionOutwardsLocationContext(locations, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2.0, 3.0);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            // Call
            var presentationObject = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocation>>>(presentationObject);
            Assert.AreSame(locations, presentationObject.WrappedData);
            Assert.AreSame(hydraulicBoundaryLocation, presentationObject.HydraulicBoundaryLocation);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            var context = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var hydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);

            var context = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ToOtherWithDifferentLocationListAndSameLocation_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocationList1 = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocationList2 = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList1, hydraulicBoundaryLocation);
            var context2 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList2, hydraulicBoundaryLocation);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameLocationListAndDifferentLocation_ReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "First name", 2.0, 3.0);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Second name", 4.0, 5.0);
            var context1 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation1);
            var context2 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation2);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherWithSameLocationListAndSameLocation_ReturnTrue()
        {
            // Setup
            var hydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);
            var context2 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var hydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Name", 2.0, 3.0);
            var context1 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);
            var context2 = new TestGrassCoverErosionOutwardsLocationContext(hydraulicBoundaryLocationList, hydraulicBoundaryLocation);

            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        private class TestGrassCoverErosionOutwardsLocationContext : GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
        {
            public TestGrassCoverErosionOutwardsLocationContext(ObservableList<HydraulicBoundaryLocation> observable,
                                                                HydraulicBoundaryLocation location)
                : base(observable, location) {}
        }
    }
}