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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class WaveHeightLocationsContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(WaveHeightLocationsContext), info.DataType);
                Assert.AreEqual(typeof(WaveHeightLocationsProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsDataCorrectly()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var random = new Random();
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();
            var hydraulicBoundaryLocationsLookup = new Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation>
            {
                {
                    new TestHydraulicBoundaryLocation(),
                    new HydraulicBoundaryLocationCalculation
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                    }
                },
                {
                    new TestHydraulicBoundaryLocation(),
                    new HydraulicBoundaryLocationCalculation
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                    }
                }
            };

            hydraulicBoundaryLocations.AddRange(hydraulicBoundaryLocationsLookup.Keys);

            var context = new WaveHeightLocationsContext(hydraulicBoundaryLocations,
                                                         assessmentSection,
                                                         () => 0.01,
                                                         hbl => hydraulicBoundaryLocationsLookup[hbl],
                                                         "Category");

            using (var plugin = new RingtoetsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<WaveHeightLocationsProperties>(objectProperties);
                Assert.AreSame(hydraulicBoundaryLocations, objectProperties.Data);
                WaveHeightLocationProperties[] locationProperties = ((WaveHeightLocationsProperties) objectProperties).Locations;
                CollectionAssert.AreEqual(hydraulicBoundaryLocationsLookup.Keys, locationProperties.Select(p => p.Data));
                CollectionAssert.AreEqual(hydraulicBoundaryLocationsLookup.Values.Select(c => c.Output.Result), locationProperties.Select(p => p.WaveHeight));
            }

            mockRepository.VerifyAll();
        }

        private static PropertyInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaveHeightLocationsContext));
        }
    }
}