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

using System;
using System.Reflection;
using Core.Common.Base;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class CalculationGroupContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new CalculationGroupContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ICalculationContext<CalculationGroup, IFailureMechanism>>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnTheSameValueAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            var properties = new CalculationGroupContextProperties
            {
                Data = new TestCalculationGroupContext(calculationGroup, new CalculationGroup(), failureMechanism)
            };

            // Call & Assert
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

            var properties = new CalculationGroupContextProperties
            {
                Data = testCalculationGroupContext
            };

            // Call & Assert
            const string name = "cool new name!";
            properties.Name = name;
            Assert.AreEqual(name, calculationGroup.Name);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Name_GroupHasEditableName_NameShouldNotBeReadonly(bool nameIsEditable)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var properties = new CalculationGroupContextProperties
            {
                Data = new TestCalculationGroupContext(new CalculationGroup("A", nameIsEditable),
                                                       new CalculationGroup(),
                                                       failureMechanism)
            };

            string propertyName = nameof(CalculationGroupContextProperties.Name);
            PropertyInfo nameProperty = properties.GetType().GetProperty(propertyName);

            // Call
            Attribute dynamicReadOnlyAttribute = Attribute.GetCustomAttribute(nameProperty,
                                                                              typeof(DynamicReadOnlyAttribute),
                                                                              true);

            // Assert
            Assert.IsNotNull(dynamicReadOnlyAttribute);
            Assert.AreEqual(!nameIsEditable, DynamicReadOnlyAttribute.IsReadOnly(properties, propertyName));
            mocks.VerifyAll();
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