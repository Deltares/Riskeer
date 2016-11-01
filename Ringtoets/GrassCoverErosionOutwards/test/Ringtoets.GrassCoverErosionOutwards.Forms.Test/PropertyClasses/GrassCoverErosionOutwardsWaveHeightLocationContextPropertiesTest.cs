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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsWaveHeightLocationContextProperties();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged, "Ja")]
        [TestCase(CalculationConvergence.CalculatedNotConverged, "Nee")]
        [TestCase(CalculationConvergence.NotCalculated, "")]
        public void GetProperties_ValidData_ReturnsExpectedValues(CalculationConvergence convergenceReached, string expectedConvergenceValue)
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var waveHeight = (RoundedDouble) 1234;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y)
            {
                WaveHeight = waveHeight,
                WaveHeightCalculationConvergence = convergenceReached
            };
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsWaveHeightLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveHeightLocationContextProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Point2D coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(expectedConvergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 567.0, 890.0);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsWaveHeightLocationContext(locations, hydraulicBoundaryLocation);

            var properties = new GrassCoverErosionOutwardsWaveHeightLocationContextProperties
            {
                Data = context
            };

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });

            PropertyDescriptor idProperty = dynamicProperties[0];
            PropertyDescriptor nameProperty = dynamicProperties[1];
            PropertyDescriptor locationProperty = dynamicProperties[2];
            PropertyDescriptor waveHeightProperty = dynamicProperties[3];
            PropertyDescriptor convergenceProperty = dynamicProperties[4];

            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            const string expectedCategory = "Algemeen";
            const string expectedIdDisplayName = "ID";
            const string expectedIdDescription = "ID van de hydraulische randvoorwaardenlocatie in de database.";
            const string expectedNameDisplayName = "Naam";
            const string expectedNameDescription = "Naam van de hydraulische randvoorwaardenlocatie.";
            const string expectedLocationDisplayName = "Coördinaten [m]";
            const string expectedLocationDescription = "Coördinaten van de hydraulische randvoorwaardenlocatie.";
            const string expectedWaveHeightDisplayName = "Golfhoogte bij doorsnede-eis [m]";
            const string expectedWaveHeightDescription = "Berekende golfhoogte bij doorsnede-eis.";
            const string expectedConvergenceDisplayName = "Convergentie";
            const string expectedConvergenceDescription = "Is convergentie bereikt in de berekening van de golfhoogte bij doorsnede-eis?";

            Assert.IsNotNull(idProperty);
            Assert.IsTrue(idProperty.IsReadOnly);
            Assert.IsTrue(idProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, idProperty.Category);
            Assert.AreEqual(expectedIdDisplayName, idProperty.DisplayName);
            Assert.AreEqual(expectedIdDescription, idProperty.Description);

            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.IsTrue(nameProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, nameProperty.Category);
            Assert.AreEqual(expectedNameDisplayName, nameProperty.DisplayName);
            Assert.AreEqual(expectedNameDescription, nameProperty.Description);

            Assert.IsNotNull(locationProperty);
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.IsTrue(locationProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, locationProperty.Category);
            Assert.AreEqual(expectedLocationDisplayName, locationProperty.DisplayName);
            Assert.AreEqual(expectedLocationDescription, locationProperty.Description);

            Assert.IsNotNull(waveHeightProperty);
            Assert.IsTrue(waveHeightProperty.IsReadOnly);
            Assert.IsTrue(waveHeightProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, waveHeightProperty.Category);
            Assert.AreEqual(expectedWaveHeightDisplayName, waveHeightProperty.DisplayName);
            Assert.AreEqual(expectedWaveHeightDescription, waveHeightProperty.Description);

            Assert.IsNotNull(convergenceProperty);
            Assert.IsTrue(convergenceProperty.IsReadOnly);
            Assert.IsTrue(convergenceProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, convergenceProperty.Category);
            Assert.AreEqual(expectedConvergenceDisplayName, convergenceProperty.DisplayName);
            Assert.AreEqual(expectedConvergenceDescription, convergenceProperty.Description);
        }
    }
}