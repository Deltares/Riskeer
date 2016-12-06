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
using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin;

namespace Ringtoets.Integration.Forms.Test.PropertyInfos
{
    [TestFixture]
    public class WaveHeightLocationsContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (RingtoetsPlugin plugin = new RingtoetsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                Type propertyObjectType = info.PropertyObjectType;

                // Assert
                Assert.AreEqual(typeof(WaveHeightLocationsContextProperties), propertyObjectType);
            }
        }

        [Test]
        public void GetObjectPropertiesData_Always_ReturnsHydraulicBoundaryDatabase()
        {
            // Setup
            MockRepository mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mockRepository.ReplayAll();

            WaveHeightLocationsContext context = new WaveHeightLocationsContext(assessmentSectionMock);

            using (RingtoetsPlugin plugin = new RingtoetsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                var objectPropertiesData = info.GetObjectPropertiesData(context);

                // Assert
                Assert.AreSame(hydraulicBoundaryDatabase, objectPropertiesData);
            }
            mockRepository.VerifyAll();
        }

        private static PropertyInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaveHeightLocationsContext));
        }
    }
}