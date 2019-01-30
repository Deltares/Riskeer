// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PropertyClasses;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class PipingStochasticSoilProfilePropertyInfoTest
    {
        private PipingPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(PipingStochasticSoilProfileProperties));
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
            Assert.AreEqual(typeof(PipingStochasticSoilProfile), info.DataType);
            Assert.AreEqual(typeof(PipingStochasticSoilProfileProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithStochasticSoilProfile_NewPropertiesWithInputAsData()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            IObjectProperties objectProperties = info.CreateInstance(stochasticSoilProfile);

            // Assert
            Assert.IsInstanceOf<PipingStochasticSoilProfileProperties>(objectProperties);
            Assert.AreSame(stochasticSoilProfile, objectProperties.Data);
        }
    }
}