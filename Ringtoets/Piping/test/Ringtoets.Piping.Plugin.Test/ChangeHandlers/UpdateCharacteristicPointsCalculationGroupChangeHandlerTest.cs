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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.ChangeHandlers;

namespace Ringtoets.Piping.Plugin.Test.ChangeHandlers
{
    [TestFixture]
    public class UpdateCharacteristicPointsCalculationGroupChangeHandlerTest
    {
        [Test]
        public void Constructor_WithoutCalculations_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHandler = mockRepository.StrictMock<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new UpdateEntryAndExitPointsCalculationGroupChangeHandler(null, inquiryHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutInquiryHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new UpdateEntryAndExitPointsCalculationGroupChangeHandler(Enumerable.Empty<PipingCalculation>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("inquiryHandler", paramName);
        }

        [Test]
        public void Constructor_WithParameters_ImplementsExpectedInterface()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHandler = mockRepository.StrictMock<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            var handler = new UpdateEntryAndExitPointsCalculationGroupChangeHandler(Enumerable.Empty<PipingCalculation>(), inquiryHandler);

            // Assert
            Assert.IsInstanceOf<IConfirmDataChangeHandler>(handler);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_WithCalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHandler = mockRepository.StrictMock<IInquiryHelper>();
            mockRepository.ReplayAll();

            IEnumerable<PipingCalculation> calculations = new List<PipingCalculation>
            {
                new PipingCalculationScenario(new GeneralPipingInput())
            };

            var handler = new UpdateEntryAndExitPointsCalculationGroupChangeHandler(calculations, inquiryHandler);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_CalculationsWithoutAndWithOutput_ReturnTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHandler = mockRepository.StrictMock<IInquiryHelper>();
            mockRepository.ReplayAll();

            IEnumerable<PipingCalculation> calculations = new List<PipingCalculation>
            {
                new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Output = new TestPipingOutput()
                },
                new PipingCalculationScenario(new GeneralPipingInput())
            };

            var handler = new UpdateEntryAndExitPointsCalculationGroupChangeHandler(calculations, inquiryHandler);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsTrue(requireConfirmation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_ShowsConfirmationDialogReturnResultOfInquiry(bool expectedResult)
        {
            // Setup
            string message = "Wanneer de intrede- en uittrede punten wijzigen als gevolg van het bijwerken, " +
                             "zullen de resultaten van berekeningen die deze profielschematisaties gebruiken, worden " +
                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            var mockRepository = new MockRepository();
            var inquiryHandler = mockRepository.StrictMock<IInquiryHelper>();
            inquiryHandler.Expect(ih => ih.InquireContinuation(message)).Return(expectedResult);
            mockRepository.ReplayAll();

            var handler = new UpdateEntryAndExitPointsCalculationGroupChangeHandler(Enumerable.Empty<PipingCalculation>(), inquiryHandler);

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedResult, result);
            mockRepository.VerifyAll();
        }
    }
}