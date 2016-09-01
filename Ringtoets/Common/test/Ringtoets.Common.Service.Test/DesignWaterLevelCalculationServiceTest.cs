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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationServiceTest
    {
        private const string validFile = "HRD dutch coast south.sqlite";
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabasePath_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculationName";
            string validFilePath = Path.Combine(testDataPath, validFile);
            bool valid = false;

            // Call
            Action call = () => valid = DesignWaterLevelCalculationService.Validate(calculationName, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabasePath_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string calculationName = "calculationName";
            string notValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            bool valid = false;

            // Call
            Action call = () => valid = DesignWaterLevelCalculationService.Validate(calculationName, notValidFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Calculate_ValidHydraulicBoundaryLocation_LogStartAndEndAndReturnOutput()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            const string calculationName = "locationName";

            var mockRepository = new MockRepository();
            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Id).Return(1300001).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();

            mockRepository.ReplayAll();
            ReliabilityIndexCalculationOutput output = null;

            // Call
            Action call = () => output = DesignWaterLevelCalculationService.Calculate(calculationMessageProviderMock,
                                                                                      hydraulicBoundaryLocationMock,
                                                                                      validFilePath, "", 30);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.IsNotNull(output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidHydraulicBoundaryLocation_LogStartAndEndAndErrorMessageAndReturnNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string calculationName = "locationName";
            const string calculationFailedMessage = "calculationFailedMessage";

            var mockRepository = new MockRepository();
            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Id).Return(1).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationFailedMessage(locationName)).Return(calculationFailedMessage).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            ReliabilityIndexCalculationOutput output = null;

            // Call
            Action call = () => output = DesignWaterLevelCalculationService.Calculate(calculationMessageProviderMock,
                                                                                      hydraulicBoundaryLocationMock,
                                                                                      validFilePath, "", 30);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(calculationFailedMessage, msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[3]);
            });
            Assert.IsNull(output);
            mockRepository.VerifyAll();
        }
    }
}