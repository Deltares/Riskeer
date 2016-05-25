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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DistributionPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new SimpleDistributionProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IDistribution>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Standaardafwijking", properties.ToString());
            Assert.AreEqual("SimpleDestributionType", properties.DistributionType);
        }

        [Test]
        public void Data_SetNewDistributionContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var distribution = new SimpleDistribution
            {
                Mean = new RoundedDouble(1, 1.1),
                StandardDeviation = new RoundedDouble(2, 2.2)
            };

            var properties = new SimpleDistributionProperties();

            // Call
            properties.Data = distribution;

            // Assert
            Assert.AreEqual(distribution.Mean, properties.Mean);
            Assert.AreEqual(distribution.StandardDeviation, properties.StandardDeviation);
        }

        [Test]
        public void SetProperties_MeanWithoutObserverable_ThrowsArgumentException()
        {
            // Setup
            var properties = new SimpleDistributionProperties
            {
                Data = new SimpleDistribution(),
            };

            // Call
            TestDelegate test = () => properties.Mean = new RoundedDouble(2, 20);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void SetProperties_StandardDeviationWithoutObserverable_ThrowsArgumentException()
        {
            // Setup
            var properties = new SimpleDistributionProperties
            {
                Data = new SimpleDistribution()
            };

            // Call
            TestDelegate test = () => properties.StandardDeviation = new RoundedDouble(2, 20);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void SetProperties_MeanWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new SimpleObserverableDistributionProperties(observerableMock)
            {
                Data = new SimpleDistribution(),
            };
            mockRepository.ReplayAll();
            RoundedDouble newMeanValue = new RoundedDouble(3, 20);

            // Call
            properties.Mean = newMeanValue;

            // Assert
            Assert.AreEqual(newMeanValue, properties.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_StandardDeviationWithObserverable_ValueSetNotifyObservers()
        {
            // Setup
            var observerableMock = mockRepository.StrictMock<IObservable>();
            observerableMock.Expect(o => o.NotifyObservers()).Repeat.Once();
            var properties = new SimpleObserverableDistributionProperties(observerableMock)
            {
                Data = new SimpleDistribution(),
            };
            mockRepository.ReplayAll();
            RoundedDouble newStandardDeviationValue = new RoundedDouble(3, 20);

            // Call
            properties.StandardDeviation = newStandardDeviationValue;

            // Assert
            Assert.AreEqual(newStandardDeviationValue, properties.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Call
            var properties = new SimpleDistributionProperties
            {
                Data = new SimpleDistribution()
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor distributionTypeProperty = dynamicProperties[0];
            Assert.IsNotNull(distributionTypeProperty);
            Assert.IsTrue(distributionTypeProperty.IsReadOnly);

            PropertyDescriptor meanProperty = dynamicProperties[1];
            Assert.IsNotNull(meanProperty);
            Assert.IsFalse(meanProperty.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanProperty.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de normale verdeling.", meanProperty.Description);

            PropertyDescriptor standardDeviationProperty = dynamicProperties[2];
            Assert.IsNotNull(standardDeviationProperty);
            Assert.IsFalse(standardDeviationProperty.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", standardDeviationProperty.DisplayName);
            Assert.AreEqual("De standaardafwijking van de normale verdeling.", standardDeviationProperty.Description);
        }

        private class SimpleDistributionProperties : DistributionProperties
        {
            public override string DistributionType
            {
                get
                {
                    return "SimpleDestributionType";
                }
            }
        }

        private class SimpleObserverableDistributionProperties : DistributionProperties
        {
            public SimpleObserverableDistributionProperties(IObservable observerable)
            {
                Observerable = observerable;
            }

            public override string DistributionType
            {
                get
                {
                    return "SimpleDestributionType";
                }
            }
        }

        private class SimpleDistribution : IDistribution
        {
            public RoundedDouble Mean { get; set; }
            public RoundedDouble StandardDeviation { get; set; }
        }
    }
}