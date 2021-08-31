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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Plugin.Handlers;

namespace Riskeer.DuneErosion.Plugin.Test.Handlers
{
    [TestFixture]
    public class DuneLocationCalculationsForTargetProbabilityChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_DuneLocationCalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForTargetProbabilityChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                new DuneLocationCalculationsForTargetProbability(0.1));

            // Assert
            Assert.IsInstanceOf<IObservablePropertyChangeHandler>(handler);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                new DuneLocationCalculationsForTargetProbability(0.1));

            // Call
            void Call() => handler.SetPropertyValueAfterConfirmation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("setValue", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_Always_ConfirmationRequired()
        {
            // Setup
            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickCancel();
            };

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                new DuneLocationCalculationsForTargetProbability(0.1));

            // Call
            handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de doelkans aanpast, dan worden alle bijbehorende hydraulische belastingen verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationsForTargetProbabilityWithAndWithoutOutput_AllCalculationOutputClearedAndReturnsAllAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = CreateCalculationsForTargetProbabilityWithAndWithoutOutput();
            List<object> expectedAffectedObjects = calculationsForTargetProbability.DuneLocationCalculations
                                                                                   .Where(dlc => dlc.Output != null)
                                                                                   .Cast<object>()
                                                                                   .ToList();

            expectedAffectedObjects.Add(calculationsForTargetProbability);

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(calculationsForTargetProbability);

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            var expectedMessages = new[]
            {
                "Alle bijbehorende hydraulische belastingen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);
            CollectionAssert.IsEmpty(calculationsForTargetProbability.DuneLocationCalculations.Where(dlc => dlc.Output != null));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationsForTargetProbabilityWithoutOutput_ReturnsNoAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1)
            {
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new TestDuneLocation())
                }
            };

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                calculationsForTargetProbability);

            DuneLocationCalculationsForTargetProbability[] expectedAffectedObjects =
            {
                calculationsForTargetProbability
            };

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 0);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationGiven_HandlerExecuted()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                new DuneLocationCalculationsForTargetProbability(0.1));

            var handlerExecuted = false;

            // Call
            handler.SetPropertyValueAfterConfirmation(() => handlerExecuted = true);

            // Assert
            Assert.IsTrue(handlerExecuted);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotGiven_SetValueNotCalledReturnsNoAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            DuneLocationCalculationsForTargetProbability calculationsForTargetProbability = CreateCalculationsForTargetProbabilityWithAndWithoutOutput();

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(calculationsForTargetProbability);

            var propertySet = 0;

            // Call
            IEnumerable<IObservable> affectedObjects = handler.SetPropertyValueAfterConfirmation(() => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            Assert.AreEqual(2, calculationsForTargetProbability.DuneLocationCalculations.Count(dlc => dlc.Output != null));
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationGivenExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new DuneLocationCalculationsForTargetProbabilityChangeHandler(
                new DuneLocationCalculationsForTargetProbability(0.1));

            var expectedException = new Exception();

            // Call
            void Call() => handler.SetPropertyValueAfterConfirmation(() => throw expectedException);

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        private static DuneLocationCalculationsForTargetProbability CreateCalculationsForTargetProbabilityWithAndWithoutOutput()
        {
            var calculationWithOutput1 = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = new TestDuneLocationCalculationOutput()
            };

            var calculationWithOutput2 = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = new TestDuneLocationCalculationOutput()
            };

            return new DuneLocationCalculationsForTargetProbability(0.1)
            {
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new TestDuneLocation()),
                    calculationWithOutput1,
                    new DuneLocationCalculation(new TestDuneLocation()),
                    calculationWithOutput2,
                    new DuneLocationCalculation(new TestDuneLocation())
                }
            };
        }
    }
}