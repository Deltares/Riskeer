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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class MacroStabilityInwardsOutputContextPropertyInfoTest
    {
        private MacroStabilityInwardsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(MacroStabilityInwardsSemiProbabilisticOutputProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MacroStabilityInwardsOutputContext), info.DataType);
            Assert.AreEqual(typeof(MacroStabilityInwardsSemiProbabilisticOutputProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_Always_NewPropertiesWithSemiProbabilisticOutputAsData()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                SemiProbabilisticOutput = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput()
            };

            var context = new MacroStabilityInwardsOutputContext(scenario);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsSemiProbabilisticOutputProperties>(objectProperties);
            Assert.AreSame(scenario.SemiProbabilisticOutput, objectProperties.Data);
        }
    }
}