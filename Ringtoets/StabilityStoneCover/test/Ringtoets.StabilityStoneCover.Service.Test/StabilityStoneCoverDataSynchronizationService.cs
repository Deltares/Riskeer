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
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverDataSynchronizationServiceTest
    {
        [Test]
        public void ClearWaveConditionsCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculationOutput_WithCalculation_OutputNull()
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation()
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                {
                    new WaveConditionsOutput(12.0, 4.3, 0.4, 49)
                }, new[]
                {
                    new WaveConditionsOutput(6.0, 3.7, 4.3, 29)
                })
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }
    }
}