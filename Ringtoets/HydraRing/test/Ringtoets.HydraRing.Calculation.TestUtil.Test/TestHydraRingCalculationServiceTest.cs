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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class TestHydraRingCalculationServiceTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var testService = new TestHydraRingCalculationService();

            // Assert
            Assert.IsInstanceOf<IHydraRingCalculationService>(testService);
            Assert.IsNull(testService.Parsers);
            Assert.IsNull(testService.HlcdDirectory);
            Assert.IsNull(testService.HydraRingCalculationInput);
            Assert.IsNull(testService.RingId);
            Assert.AreEqual(HydraRingUncertaintiesType.None, testService.UncertaintiesType);
        }

        [Test]
        public void PerformCalculation_Always_SetsValues()
        {
            // Setup
            var parsers = new IHydraRingFileParser[]
            {
                new WaveConditionsCalculationParser()
            };
            string hlcdDirectory = "C:/temp";
            string ringId = "205";
            var input = new TestInput(1);
            var uncertaintiesType = HydraRingUncertaintiesType.All;

            var testService = new TestHydraRingCalculationService();

            // Call
            testService.PerformCalculation(hlcdDirectory, ringId, uncertaintiesType, input, parsers);

            // Assert
            Assert.AreEqual(hlcdDirectory, testService.HlcdDirectory);
            Assert.AreEqual(ringId, testService.RingId);
            Assert.AreEqual(uncertaintiesType, testService.UncertaintiesType);
            Assert.AreSame(input, testService.HydraRingCalculationInput);
            Assert.AreSame(parsers, testService.Parsers);
        }

        private class TestInput : HydraRingCalculationInput
        {
            public TestInput(long hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int CalculationTypeId
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int VariableId
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override HydraRingSection Section
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}