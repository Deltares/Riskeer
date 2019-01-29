// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartDataCollectionPropertiesTest
    {
        private const int namePropertyIndex = 0;

        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Call
            var properties = new ChartDataCollectionProperties();

            Assert.IsInstanceOf<ObjectProperties<ChartDataCollection>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewChartDataCollectionInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("Test");
            var properties = new ChartDataCollectionProperties();

            // Call
            properties.Data = chartDataCollection;

            // Assert
            Assert.AreEqual(chartDataCollection.Name, properties.Name);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("Test");

            // Call
            var properties = new ChartDataCollectionProperties
            {
                Data = chartDataCollection
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van deze gegevensreeks.",
                                                                            true);
        }
    }
}