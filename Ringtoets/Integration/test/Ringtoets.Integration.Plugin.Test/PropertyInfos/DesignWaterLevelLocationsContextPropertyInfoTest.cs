﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    public class DesignWaterLevelLocationsContextPropertyInfoTest
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
                Assert.AreEqual(typeof(DesignWaterLevelCalculationsContext), info.DataType);
                Assert.AreEqual(typeof(DesignWaterLevelCalculationsProperties), info.PropertyObjectType);
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

            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                },
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                }
            };

            var context = new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  "Category");

            using (var plugin = new RingtoetsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<DesignWaterLevelCalculationsProperties>(objectProperties);
                Assert.AreSame(hydraulicBoundaryLocationCalculations, objectProperties.Data);
                DesignWaterLevelCalculationProperties[] calculationProperties = ((DesignWaterLevelCalculationsProperties) objectProperties).Calculations;
                CollectionAssert.AreEqual(hydraulicBoundaryLocationCalculations, calculationProperties.Select(p => p.Data));
            }

            mockRepository.VerifyAll();
        }

        private static PropertyInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(DesignWaterLevelCalculationsContext));
        }
    }
}