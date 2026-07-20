// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Observers;

namespace Riskeer.Common.Forms.Test.Observers
{
    [TestFixture]
    public class CalculatableFailureMechanismResultObserverTest
    {
        [Test]
        public void Constructor_WithFailureMechanism_ExpectedProperties()
        {
            // Call
            using (var resultObserver = new CalculatableFailureMechanismResultObserver<TestCalculatableFailureMechanism,
                       FailureMechanismSectionResult, TestCalculationWithInput>(new TestCalculatableFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultObserver<TestCalculatableFailureMechanism,
                    FailureMechanismSectionResult>>(resultObserver);
            }
        }

        [Test]
        public void GivenFailureMechanismResultObserverWithCalculationAndAttachedObserver_WhenCalculationNotifiesObservers_ThenAttachedObserverNotified()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            var calculation = new TestCalculationWithInput();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new CalculatableFailureMechanismResultObserver<TestCalculatableFailureMechanism,
                       FailureMechanismSectionResult, TestCalculationWithInput>(failureMechanism))
            {
                var observer = Substitute.For<IObserver>();
                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                observer.Received().UpdateObserver();
            }
        }

        [Test]
        public void GivenFailureMechanismResultObserverWithCalculationAndAttachedObserver_WhenCalculationInputNotifiesObservers_ThenAttachedObserverNotified()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            var calculation = new TestCalculationWithInput();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new CalculatableFailureMechanismResultObserver<TestCalculatableFailureMechanism,
                       FailureMechanismSectionResult, TestCalculationWithInput>(failureMechanism))
            {
                var observer = Substitute.For<IObserver>();
                resultObserver.Attach(observer);

                // When
                calculation.InputParameters.NotifyObservers();

                // Then
                observer.Received().UpdateObserver();
            }
        }

        private class TestCalculationWithInput : CloneableObservable, ICalculation<TestCalculationInput>
        {
            public TestCalculationWithInput()
            {
                InputParameters = new TestCalculationInput();
            }

            public TestCalculationInput InputParameters { get; }

            public string Name { get; set; }

            public bool ShouldCalculate { get; }

            public bool HasOutput { get; }

            public Comment Comments { get; }

            public void ClearOutput()
            {
                throw new NotImplementedException();
            }
        }
    }
}