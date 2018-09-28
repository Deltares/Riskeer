// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartDataPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int visiblePropertyIndex = 2;

        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Call
            var properties = new TestChartDataProperties();

            Assert.IsInstanceOf<ObjectProperties<ChartData>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewChartDataContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var chartData = new TestChartData("TestChart");
            var properties = new TestChartDataProperties();

            // Call
            properties.Data = chartData;

            // Assert
            Assert.AreEqual(chartData.Name, properties.Name);
            Assert.AreEqual(chartData.IsVisible, properties.IsVisible);
            Assert.AreEqual("Test type string", properties.Type);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 1;

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var chartData = new TestChartData("Test");

            chartData.Attach(observer);

            var properties = new TestChartDataProperties
            {
                Data = chartData
            };

            // Call
            properties.IsVisible = false;

            // Assert
            Assert.IsFalse(chartData.IsVisible);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var chartDataContext = new TestChartData("TestChart");

            // Call
            var properties = new TestChartDataProperties
            {
                Data = chartDataContext
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van deze gegevensreeks.",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[typePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            generalCategory,
                                                                            "Type",
                                                                            "Het type van de data die wordt weergegeven in de gegevensreeks.",
                                                                            true);

            PropertyDescriptor isVisibleProperty = dynamicProperties[visiblePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isVisibleProperty,
                                                                            generalCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of de gegevensreeks wordt weergegeven.");
        }

        private class TestChartDataProperties : ChartDataProperties<ChartData>
        {
            public override string Type
            {
                get
                {
                    return "Test type string";
                }
            }
        }
    }
}