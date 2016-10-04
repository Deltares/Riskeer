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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Calculator
{
    public class TestHydraRingCalculatorFactory : IHydraRingCalculatorFactory {

        public readonly TestDesignWaterLevelCalculator DesignWaterLevelCalculator = new TestDesignWaterLevelCalculator();
        public readonly TestDikeHeightCalculator DikeHeightCalculator = new TestDikeHeightCalculator();
        public readonly TestOvertoppingCalculator OvertoppingCalculator = new TestOvertoppingCalculator();
        public readonly TestWaveConditionsCosineCalculator WaveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();
        public readonly TestWaveHeightCalculator WaveHeightCalculator = new TestWaveHeightCalculator();

        public IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(string hlcdDirectory, string ringId)
        {
            DesignWaterLevelCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            DesignWaterLevelCalculator.RingId = ringId;
            return DesignWaterLevelCalculator;
        }

        public IDikeHeightCalculator CreateDikeHeightCalculator(string hlcdDirectory, string ringId)
        {
            DikeHeightCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            DikeHeightCalculator.RingId = ringId;
            return DikeHeightCalculator;
        }

        public IOvertoppingCalculator CreateOvertoppingCalculator(string hlcdDirectory, string ringId)
        {
            OvertoppingCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            OvertoppingCalculator.RingId = ringId;
            return OvertoppingCalculator;
        }

        public IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(string hlcdDirectory, string ringId)
        {
            WaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            WaveConditionsCosineCalculator.RingId = ringId;
            return WaveConditionsCosineCalculator;
        }

        public IWaveHeightCalculator CreateWaveHeightCalculator(string hlcdDirectory, string ringId)
        {
            WaveHeightCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            WaveHeightCalculator.RingId = ringId;
            return WaveHeightCalculator;
        }
    }

    public class TestWaveHeightCalculator : TestHydraRingCalculator<WaveHeightCalculationInput>, IWaveHeightCalculator
    {
        public double WaveHeight { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputFileContent { get; set; }
    }

    public class TestWaveConditionsCosineCalculator : TestHydraRingCalculator<WaveConditionsCosineCalculationInput>, IWaveConditionsCosineCalculator
    {
        public double WaveHeight { get; set; }
        public double WaveAngle { get; set; }
        public double WavePeakPeriod { get; set; }
        public string OutputFileContent { get; set; }
    }

    public class TestOvertoppingCalculator : TestHydraRingCalculator<OvertoppingCalculationInput>, IOvertoppingCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public double WaveHeight { get; set; }
        public bool IsOvertoppingDominant { get; set; }
        public string OutputFileContent { get; set; }
    }

    public class TestDikeHeightCalculator : TestHydraRingCalculator<DikeHeightCalculationInput>, IDikeHeightCalculator {
        public double DikeHeight { get; set; }
        public string OutputFileContent { get; set; }
    }

    public class TestDesignWaterLevelCalculator : TestHydraRingCalculator<AssessmentLevelCalculationInput>, IDesignWaterLevelCalculator {
        public double DesignWaterLevel { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputFileContent { get; set; }
    }

    public class TestHydraRingCalculator<T>
    {
        public bool IsCanceled = false;
        public readonly List<T> ReceivedInputs = new List<T>();
        public string RingId { get; set; }
        public string HydraulicBoundaryDatabaseDirectory { get; set; }
        public bool EndInFailure { get; set; }

        public void Calculate(T input)
        {
            if (EndInFailure)
            {
                throw new HydraRingFileParserException();
            }
            ReceivedInputs.Add(input);
        }


        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}