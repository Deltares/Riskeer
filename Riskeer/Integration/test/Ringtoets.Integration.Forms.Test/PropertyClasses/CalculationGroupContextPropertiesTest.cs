// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class CalculationGroupContextPropertiesTest
    {
        [Test]
        public void Constructor_CalculationGroupContextNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CalculationGroupContextProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculationContext", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculationGroupContext = new TestCalculationGroupContext(new CalculationGroup(), new CalculationGroup(), failureMechanism);

            // Call
            var properties = new CalculationGroupContextProperties(calculationGroupContext);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ICalculationContext<CalculationGroup, IFailureMechanism>>>(properties);
            Assert.AreEqual(calculationGroupContext, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnTheSameValueAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            // Call
            var properties = new CalculationGroupContextProperties(new TestCalculationGroupContext(calculationGroup, new CalculationGroup(), failureMechanism));

            // Assert
            Assert.AreEqual(calculationGroup.Name, properties.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var testCalculationGroupContext = new TestCalculationGroupContext(calculationGroup, new CalculationGroup(), failureMechanism);

            calculationGroup.Attach(projectObserver);

            // Call
            var properties = new CalculationGroupContextProperties(testCalculationGroupContext);

            // Assert
            const string name = "cool new name!";
            properties.Name = name;
            Assert.AreEqual(name, calculationGroup.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var properties = new CalculationGroupContextProperties(new TestCalculationGroupContext(new CalculationGroup(),
                                                                                                   new CalculationGroup(),
                                                                                                   failureMechanism));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam",
                                                                            "Naam van de map met berekeningen.");
        }

        [Test]
        public void DynamicReadOnlyValidator_WithParentCalculationGroup_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var properties = new CalculationGroupContextProperties(new TestCalculationGroupContext(new CalculationGroup(),
                                                                                                   new CalculationGroup(),
                                                                                                   failureMechanism));

            // Call
            bool isReadOnly = properties.DynamicReadOnlyValidator(null);

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void DynamicReadOnlyValidator_WithoutParentCalculationGroup_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var properties = new CalculationGroupContextProperties(new TestCalculationGroupContext(new CalculationGroup(),
                                                                                                   null,
                                                                                                   failureMechanism));

            // Call
            bool isReadOnly = properties.DynamicReadOnlyValidator(null);

            // Assert
            Assert.IsTrue(isReadOnly);
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }
    }
}