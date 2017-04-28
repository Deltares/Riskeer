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
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenarioRowTest
    {
        [Test]
        public void Constructor_WithoutPipingCalculation_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingScenarioRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pipingCalculation", paramName);
        }

        [Test]
        public void Constructor_WithPipingCalculationWithSemiProbabilisticOutput_PropertiesFromPipingCalculation()
        {
            // Setup
            var random = new Random(21);
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Call
            var row = new PipingScenarioRow(calculation);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreEqual(calculation.IsRelevant, row.IsRelevant);
            Assert.AreEqual(calculation.Contribution * 100, row.Contribution);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(calculation.SemiProbabilisticOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(calculation.SemiProbabilisticOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(calculation.SemiProbabilisticOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(calculation.SemiProbabilisticOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);
        }

        [Test]
        public void Constructor_WithPipingCalculationWithoutSemiProbabilisticOutput_PropertiesFromPipingCalculation()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            var row = new PipingScenarioRow(calculation);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreEqual(calculation.IsRelevant, row.IsRelevant);
            Assert.AreEqual(calculation.Contribution * 100, row.Contribution);
            Assert.AreEqual("-", row.FailureProbabilityPiping);
            Assert.AreEqual("-", row.FailureProbabilityUplift);
            Assert.AreEqual("-", row.FailureProbabilityHeave);
            Assert.AreEqual("-", row.FailureProbabilitySellmeijer);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChange_NotifyObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.Attach(observer);

            var row = new PipingScenarioRow(calculation);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, calculation.IsRelevant);

            mocks.VerifyAll();
        }

        [Test]
        public void Contribution_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            int newValue = new Random().Next(0, 100);

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingScenarioRow(calculation);

            var counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation
            })
            {
                // Call
                row.Contribution = (RoundedDouble) newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(new RoundedDouble(2, newValue), calculation.Contribution * 100);
            }
        }
    }
}