// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class ScenarioRowTest
    {
        [Test]
        public void Constructor_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestScenarioRow<ICalculationScenario>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Test";
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            RoundedDouble contribution = random.NextRoundedDouble();

            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            calculationScenario.Name = name;
            calculationScenario.IsRelevant = isRelevant;
            calculationScenario.Contribution = contribution;

            // Call
            var row = new TestScenarioRow<ICalculationScenario>(calculationScenario);

            // Assert
            Assert.AreSame(calculationScenario, row.CalculationScenario);
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.AreEqual(2, row.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(contribution * 100, row.Contribution, row.Contribution.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChange_NotifyObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            calculationScenario.Expect(cs => cs.NotifyObservers());
            mocks.ReplayAll();

            var row = new TestScenarioRow<ICalculationScenario>(calculationScenario);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, calculationScenario.IsRelevant);
            mocks.VerifyAll();
        }

        [Test]
        public void Contribution_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenario = mocks.Stub<ICalculationScenario>();
            calculationScenario.Expect(cs => cs.NotifyObservers());
            mocks.ReplayAll();

            double newValue = new Random(21).NextDouble(0, 100);

            var row = new TestScenarioRow<ICalculationScenario>(calculationScenario);

            // Call
            row.Contribution = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue / 100, calculationScenario.Contribution, calculationScenario.Contribution.GetAccuracy());
            mocks.VerifyAll();
        }

        private class TestScenarioRow<TCalculationScenario> : ScenarioRow<TCalculationScenario>
            where TCalculationScenario : class, ICalculationScenario
        {
            public TestScenarioRow(TCalculationScenario calculationScenario)
                : base(calculationScenario) {}

            public override string FailureProbability { get; }
        }
    }
}