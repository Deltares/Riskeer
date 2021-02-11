﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class DesignWaterLevelCalculationsGroupContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(DesignWaterLevelCalculationsGroupContext), info.DataType);
                Assert.AreEqual(typeof(DesignWaterLevelCalculationsGroupProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsDataCorrectly()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>();
            var context = new DesignWaterLevelCalculationsGroupContext(locations, new AssessmentSectionStub());

            using (var plugin = new RiskeerPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<DesignWaterLevelCalculationsGroupProperties>(objectProperties);
                Assert.AreSame(locations, objectProperties.Data);
            }
        }

        private static PropertyInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(DesignWaterLevelCalculationsGroupContext));
        }
    }
}