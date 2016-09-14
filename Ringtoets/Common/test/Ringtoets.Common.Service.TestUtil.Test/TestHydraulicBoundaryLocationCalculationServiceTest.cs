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

using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.TestUtil.Test
{
    [TestFixture]
    public class TestHydraulicBoundaryLocationCalculationServiceTest
    {
        [Test]
        public void Validate_Always_ReturnTrue()
        {
            // Setup
            var service = new TestHydraulicBoundaryLocationCalculationService();

            // Call
            bool validated = service.Validate(string.Empty, string.Empty);

            // Assert
            Assert.IsTrue(validated);
        }

        [Test]
        public void Calculate_Always_SetsInputParameters()
        {
            // Setup
            const double norm = 12.34;
            const string filePath = "C:/temp";
            const string ringId = "11-1";

            var mockRepository = new MockRepository();
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var service = new TestHydraulicBoundaryLocationCalculationService();

            // Call
            service.Calculate(hydraulicBoundaryLocation,
                              filePath,
                              ringId,
                              norm,
                              calculationMessageProviderMock);

            // Assert
            Assert.AreSame(calculationMessageProviderMock, service.MessageProvider);
            Assert.AreSame(hydraulicBoundaryLocation, service.HydraulicBoundaryLocation);
            Assert.AreEqual(filePath, service.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(ringId, service.RingId);
            Assert.AreEqual(norm, service.Norm);
        }

        [Test]
        public void Calculate_SetCalculationConvergenceOutputDefault_ReturnsExpectedValue()
        {
            // Setup
            const double norm = 12.34;

            var mockRepository = new MockRepository();
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var service = new TestHydraulicBoundaryLocationCalculationService();

            // Call
            ReliabilityIndexCalculationOutput output = service.Calculate(hydraulicBoundaryLocation,
                                                                         string.Empty,
                                                                         string.Empty,
                                                                         norm,
                                                                         calculationMessageProviderMock);

            // Assert
            var expectedOutput = new ReliabilityIndexCalculationOutput(norm, StatisticsConverter.NormToBeta(norm));
            AssertReliabilityIndexCalculationOutput(expectedOutput, output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_SetCalculationConvergenceOutputCalculatedConverged_ReturnsExpectedValue()
        {
            // Setup
            const double norm = 12.34;

            var mockRepository = new MockRepository();
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var service = new TestHydraulicBoundaryLocationCalculationService
            {
                CalculationConvergenceOutput = CalculationConvergence.CalculatedConverged
            };

            // Call
            ReliabilityIndexCalculationOutput output = service.Calculate(hydraulicBoundaryLocation,
                                                                         string.Empty,
                                                                         string.Empty,
                                                                         norm,
                                                                         calculationMessageProviderMock);

            // Assert
            Assert.AreSame(calculationMessageProviderMock, service.MessageProvider);
            var expectedOutput = new ReliabilityIndexCalculationOutput(norm, StatisticsConverter.NormToBeta(norm));
            AssertReliabilityIndexCalculationOutput(expectedOutput, output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_SetCalculationConvergenceOutputNotCalculated_ReturnsNull()
        {
            // Setup
            const double norm = 12.34;

            var mockRepository = new MockRepository();
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var service = new TestHydraulicBoundaryLocationCalculationService
            {
                CalculationConvergenceOutput = CalculationConvergence.NotCalculated
            };

            // Call
            ReliabilityIndexCalculationOutput output = service.Calculate(hydraulicBoundaryLocation,
                                                                         string.Empty,
                                                                         string.Empty,
                                                                         norm,
                                                                         calculationMessageProviderMock);

            // Assert
            Assert.AreSame(calculationMessageProviderMock, service.MessageProvider);
            Assert.IsNull(output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_SetCalculationConvergenceOutputCalculatedNotConverged_ReturnsExpectedValue()
        {
            // Setup
            const double norm = 12.34;

            var mockRepository = new MockRepository();
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var service = new TestHydraulicBoundaryLocationCalculationService
            {
                CalculationConvergenceOutput = CalculationConvergence.CalculatedNotConverged
            };

            // Call
            ReliabilityIndexCalculationOutput output = service.Calculate(hydraulicBoundaryLocation,
                                                                         string.Empty,
                                                                         string.Empty,
                                                                         norm,
                                                                         calculationMessageProviderMock);

            // Assert
            Assert.AreSame(calculationMessageProviderMock, service.MessageProvider);
            var expectedOutput = new ReliabilityIndexCalculationOutput(norm, norm);
            AssertReliabilityIndexCalculationOutput(expectedOutput, output);
            mockRepository.VerifyAll();
        }

        private static void AssertReliabilityIndexCalculationOutput(ReliabilityIndexCalculationOutput expected, ReliabilityIndexCalculationOutput actual)
        {
            Assert.AreEqual(expected.CalculatedReliabilityIndex, actual.CalculatedReliabilityIndex);
            Assert.AreEqual(expected.Result, actual.Result, 1e-6);
        }
    }
}