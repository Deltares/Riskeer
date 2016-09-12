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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataCalculationServiceTest
    {
        [Test]
        public void CalculationConverged_WithNullOutput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataCalculationService.CalculationConverged(null, 1.0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CalculationConverged_WithConvergedResults_CalculationConvergedTrue()
        {
            // Setup
            double norm = 1.0e3;
            double reliabilityIndex = StatisticsConverter.NormToBeta(norm);
            var output = new ReliabilityIndexCalculationOutput(reliabilityIndex, reliabilityIndex);

            // Call
            bool calculationConverged = RingtoetsCommonDataCalculationService.CalculationConverged(output, norm);

            // Assert
            Assert.IsTrue(calculationConverged);
        }

        [Test]
        public void CalculationConverged_WithoutConvergedResults_CalculationConvergedFalse()
        {
            // Setup
            var output = new ReliabilityIndexCalculationOutput(5.0e-3, 5.0e-3);
            double norm = 1;

            // Call
            bool calculationConverged = RingtoetsCommonDataCalculationService.CalculationConverged(output, norm);

            // Assert
            Assert.IsFalse(calculationConverged);
        }
    }
}
