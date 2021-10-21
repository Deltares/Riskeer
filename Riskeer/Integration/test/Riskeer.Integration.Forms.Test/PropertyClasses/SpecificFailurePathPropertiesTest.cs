// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SpecificFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int isRelevantPropertyIndex = 1;
        
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SpecificFailurePathProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("data", paramName);
        }
        
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failurePath = new SpecificFailurePath
            {
                IsRelevant = random.NextBoolean()
            };
            
            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<SpecificFailurePath>>(properties);
            Assert.AreEqual(failurePath.Name, properties.Name);
            Assert.AreEqual(failurePath.IsRelevant, properties.IsRelevant);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var random = new Random(21);
            var failurePath = new SpecificFailurePath
            {
                IsRelevant = random.NextBoolean()
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "Naam van het faalpad.");

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);
        }        
        
        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            const int numberOfChangedProperties = 1;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            // Setup
            var random = new Random(21);
            var failurePath = new SpecificFailurePath
            {
                IsRelevant = random.NextBoolean()
            };

            // Call
            var properties = new SpecificFailurePathProperties(failurePath);

            failurePath.Attach(projectObserver);

            // Call
            const string newName = "Some new cool pretty name";
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, failurePath.Name);

            mocks.VerifyAll();
        }
    }
}