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

using System;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Importer;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelChangeHandler(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ImplementsExpectedInterface()
        {
            // Call
            var handler = new StochasticSoilModelChangeHandler(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelChangeHandler>(handler);
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(new GeneralPipingInput()));

            var handler = new StochasticSoilModelChangeHandler(failureMechanism);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithOutput_ReturnTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var handler = new StochasticSoilModelChangeHandler(failureMechanism);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsTrue(requireConfirmation);
        }

        [Test]
        public void InquireConfirmation_Always_ShowsConfirmationDialog()
        {
            // Setup
            var handler = new StochasticSoilModelChangeHandler(new PipingFailureMechanism());

            string dialogText = string.Empty; 

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogText = tester.Text;
                tester.ClickCancel();
            };

            // Call
            handler.InquireConfirmation();

            // Assert
            const string message = "Wanneer ondergrondschematisaties wijzigen als gevolg van het bijwerken, " +
                                   "zullen de resultaten van berekeningen die deze ondergrondschematisaties worden " +
                                   "verwijderd. Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(message, dialogText);
        }

        [Test]
        public void InquireConfirmation_PressOkInMessageBox_ReturnsTrue()
        {
            // Setup
            var handler = new StochasticSoilModelChangeHandler(new PipingFailureMechanism());
            
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            // Call
            var confirmed = handler.InquireConfirmation();

            // Assert
            Assert.IsTrue(confirmed);
        }

        [Test]
        public void InquireConfirmation_PressCancelInMessageBox_ReturnsFalse()
        {
            // Setup
            var handler = new StochasticSoilModelChangeHandler(new PipingFailureMechanism());
            
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            // Call
            var confirmed = handler.InquireConfirmation();

            // Assert
            Assert.IsFalse(confirmed);
        }
    }
}