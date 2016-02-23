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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new HydraulicBoundaryDatabaseProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabaseContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 1.0, 2.0);
            HydraulicBoundaryLocationProperties expectedLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocation);

            HydraulicBoundaryDatabaseContext hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(new HydraulicBoundaryDatabase(), assessmentSectionBaseMock);
            hydraulicBoundaryDatabaseContext.BoundaryDatabase.FilePath = "Test";
            hydraulicBoundaryDatabaseContext.BoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            HydraulicBoundaryDatabaseProperties properties = new HydraulicBoundaryDatabaseProperties
            {
                Data = hydraulicBoundaryDatabaseContext
            };

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseContext.BoundaryDatabase.FilePath, properties.FilePath);
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(HydraulicBoundaryLocationProperties));
            Assert.AreEqual(1, hydraulicBoundaryDatabaseContext.BoundaryDatabase.Locations.Count);

            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = properties.Locations.FirstOrDefault();
            Assert.AreEqual(expectedLocationProperties.Name, hydraulicBoundaryLocationProperties.Name);
            Assert.AreEqual(expectedLocationProperties.Id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(expectedLocationProperties.Location, hydraulicBoundaryLocationProperties.Location);
            Assert.AreEqual(expectedLocationProperties.DesignWaterLevel, hydraulicBoundaryLocationProperties.DesignWaterLevel);
        }
    }
}