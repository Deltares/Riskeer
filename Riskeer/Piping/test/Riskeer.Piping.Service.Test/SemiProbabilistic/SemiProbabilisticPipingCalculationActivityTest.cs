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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Service.SemiProbabilistic;

namespace Riskeer.Piping.Service.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationActivityTest
    {
        [Test]
        public void Constructor_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SemiProbabilisticPipingCalculationActivity(new TestSemiProbabilisticPipingCalculation(),
                                                                          null,
                                                                          RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation();

            // Call
            var activity = new SemiProbabilisticPipingCalculationActivity(calculation,
                                                                          new GeneralPipingInput(),
                                                                          RoundedDouble.NaN);

            // Assert
            Assert.IsInstanceOf<CalculatableActivity>(activity);
            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}'", activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_InvalidPipingCalculation_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var invalidPipingCalculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<TestSemiProbabilisticPipingCalculation>();

            var activity = new SemiProbabilisticPipingCalculationActivity(invalidPipingCalculation,
                                                                          new GeneralPipingInput(),
                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                Assert.AreEqual(8, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{invalidPipingCalculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual(Level.Error, tupleArray[2].Item2);
                Assert.AreEqual(Level.Error, tupleArray[3].Item2);
                Assert.AreEqual(Level.Error, tupleArray[4].Item2);
                Assert.AreEqual(Level.Error, tupleArray[5].Item2);
                Assert.AreEqual(Level.Error, tupleArray[6].Item2);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[7]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidPipingCalculation_PerformPipingValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var validPipingCalculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    new TestHydraulicBoundaryLocation());

            var activity = new SemiProbabilisticPipingCalculationActivity(validPipingCalculation,
                                                                          new GeneralPipingInput(),
                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{validPipingCalculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void Finish_ValidPipingCalculationAndRan_NotifyObserversOfPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var validPipingCalculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                    new TestHydraulicBoundaryLocation());
            validPipingCalculation.Output = null;
            validPipingCalculation.Attach(observer);

            var activity = new SemiProbabilisticPipingCalculationActivity(validPipingCalculation,
                                                                          new GeneralPipingInput(),
                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}