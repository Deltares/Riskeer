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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class CalculationContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new CalculationContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ICalculationContext<ICalculation, IFailureMechanism>>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string name = "<very cool name>";
            var calculation = new TestCalculation(name);

            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var properties = new CalculationContextProperties
            {
                Data = new TestCalculationContext(calculation, new CalculationGroup(), failureMechanism)
            };

            // Call & Assert
            Assert.AreEqual(name, properties.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            const int numberOfChangedProperties = 1;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var testCalculationContext = new TestCalculationContext(calculation, new CalculationGroup(), failureMechanism);

            calculation.Attach(projectObserver);

            var properties = new CalculationContextProperties
            {
                Data = testCalculationContext
            };

            // Call
            const string newName = "Some new cool pretty name";
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, calculation.Name);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new CalculationContextProperties();

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";
            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "Naam van de berekening.");
        }

        private class TestCalculationContext : Observable, ICalculationContext<ICalculation, IFailureMechanism>
        {
            public TestCalculationContext(ICalculation wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public ICalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }
    }
}