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

using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

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
            var calculation = new TestCalculation
            {
                Name = name
            };

            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var properties = new CalculationContextProperties
            {
                Data = new TestCalculationContext(calculation, failureMechanismMock)
            };

            // Call & Assert
            Assert.AreEqual(name, properties.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var testCalculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            calculation.Attach(projectObserver);

            var properties = new CalculationContextProperties
            {
                Data = testCalculationContext
            };

            // Call & Assert
            const string newName = "haha";
            properties.Name = newName;
            Assert.AreEqual(newName, calculation.Name);
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
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var testCalculationContext = new TestCalculationContext(calculation, failureMechanismMock);

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

        private class TestCalculationContext : Observable, ICalculationContext<ICalculation, IFailureMechanism>
        {
            public TestCalculationContext(ICalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public ICalculation WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public string Name { get; set; }

            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }
    }
}