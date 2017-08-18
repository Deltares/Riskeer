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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StochasticSoilModelPropertiesTest
    {
        private const int stochasticSoilModelNamePropertyIndex = 0;
        private const int stochasticSoilModelGeometryPropertyIndex = 1;
        private const int stochasticSoilModelStochasticSoilProfilesPropertyIndex = 2;

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new StochasticSoilModelProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StochasticSoilModel>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel("Name");
            stochasticSoilModel.Geometry.Add(new Point2D(1.0, 2.0));
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("MacroStabilityInwardsSoilProfile", 0, new List<MacroStabilityInwardsSoilLayer1D>
            {
                new MacroStabilityInwardsSoilLayer1D(10)
            }, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = soilProfile
            };
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            // Call
            var properties = new StochasticSoilModelProperties
            {
                Data = stochasticSoilModel
            };

            // Assert
            Assert.AreEqual(stochasticSoilModel.Name, properties.Name);
            Assert.AreEqual(stochasticSoilModel.Geometry[0], properties.Geometry[0]);

            Assert.IsInstanceOf<StochasticSoilProfileProperties[]>(properties.StochasticSoilProfiles);
            Assert.AreEqual(1, properties.StochasticSoilProfiles.Length);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel("Name");
            stochasticSoilModel.Geometry.Add(new Point2D(1.0, 2.0));
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("MacroStabilityInwardsSoilProfile", 0, new List<MacroStabilityInwardsSoilLayer1D>
            {
                new MacroStabilityInwardsSoilLayer1D(10)
            }, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = soilProfile
            };
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            // Call
            var properties = new StochasticSoilModelProperties
            {
                Data = stochasticSoilModel
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor stochasticSoilModelNameProperty = dynamicProperties[stochasticSoilModelNamePropertyIndex];
            Assert.IsNotNull(stochasticSoilModelNameProperty);
            Assert.IsTrue(stochasticSoilModelNameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", stochasticSoilModelNameProperty.Category);
            Assert.AreEqual("Naam", stochasticSoilModelNameProperty.DisplayName);
            Assert.AreEqual("Naam van het stochastische ondergrondmodel.", stochasticSoilModelNameProperty.Description);

            PropertyDescriptor stochasticSoilModelGeometryProperty = dynamicProperties[stochasticSoilModelGeometryPropertyIndex];
            Assert.IsNotNull(stochasticSoilModelGeometryProperty);
            Assert.IsTrue(stochasticSoilModelGeometryProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", stochasticSoilModelGeometryProperty.Category);
            Assert.AreEqual("Geometrie", stochasticSoilModelGeometryProperty.DisplayName);
            Assert.AreEqual("Geometrie uit de database.", stochasticSoilModelGeometryProperty.Description);

            PropertyDescriptor stochasticSoilModelStochasticSoilProfilesProperty = dynamicProperties[stochasticSoilModelStochasticSoilProfilesPropertyIndex];
            Assert.IsNotNull(stochasticSoilModelStochasticSoilProfilesProperty);
            Assert.IsTrue(stochasticSoilModelStochasticSoilProfilesProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", stochasticSoilModelStochasticSoilProfilesProperty.Category);
            Assert.AreEqual("Ondergrondschematisaties", stochasticSoilModelStochasticSoilProfilesProperty.DisplayName);
            Assert.AreEqual("Ondergrondschematisaties uit de database.", stochasticSoilModelStochasticSoilProfilesProperty.Description);
        }
    }
}