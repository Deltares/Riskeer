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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PropertyClasses;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingStochasticSoilModelPropertiesTest
    {
        private const int stochasticSoilModelNamePropertyIndex = 0;
        private const int stochasticSoilModelGeometryPropertyIndex = 1;
        private const int stochasticSoilModelStochasticSoilProfilesPropertyIndex = 2;

        [Test]
        public void Constructor_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilModelProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidStochasticSoilModel_ExpectedValues()
        {
            // Setup
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Call
            var properties = new PipingStochasticSoilModelProperties(stochasticSoilModel);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingStochasticSoilModel>>(properties);
            TestHelper.AssertTypeConverter<PipingStochasticSoilModelProperties, ExpandableArrayConverter>(nameof(PipingStochasticSoilModelProperties.Geometry));
            TestHelper.AssertTypeConverter<PipingStochasticSoilModelProperties, ExpandableArrayConverter>(nameof(PipingStochasticSoilModelProperties.StochasticSoilProfiles));
            Assert.AreSame(stochasticSoilModel, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var stochasticSoilModel = new PipingStochasticSoilModel("Name", new[]
            {
                new Point2D(1.0, 2.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });

            // Call
            var properties = new PipingStochasticSoilModelProperties(stochasticSoilModel);

            // Assert
            Assert.AreEqual(stochasticSoilModel.Name, properties.Name);
            CollectionAssert.AreEqual(stochasticSoilModel.Geometry, properties.Geometry);

            PipingStochasticSoilProfile[] stochasticSoilProfiles = stochasticSoilModel.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(stochasticSoilProfiles.Length, properties.StochasticSoilProfiles.Length);
            Assert.AreSame(stochasticSoilProfiles[0], properties.StochasticSoilProfiles[0].Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Call
            var properties = new PipingStochasticSoilModelProperties(stochasticSoilModel);

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