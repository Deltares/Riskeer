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
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutput_WithOutput_ClearsCalculationsOutput()
        {
            // Setup
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            PipingCalculation calculation1 = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };
            
            PipingCalculation calculation2 = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);

            // Call
            PipingDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            foreach (PipingCalculation calculation in failureMechanism.CalculationsGroup.Children.Cast<PipingCalculation>())
            {
                Assert.IsNull(calculation.Output);
            }
        }

        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            PipingCalculation calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            // Call
            PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocations_WithHydraulicBoundaryLocation_ClearsHydraulicBoundaryLocation()
        {
            // Setup
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);
            
            PipingCalculation calculation1 = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            PipingCalculation calculation2 = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);

            // Call
            PipingDataSynchronizationService.ClearHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (PipingCalculation calculation in failureMechanism.CalculationsGroup.Children.Cast<PipingCalculation>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }
    }
}