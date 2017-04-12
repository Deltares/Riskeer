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
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Calculator
{
    public class TestHydraRingCalculatorFactory : IHydraRingCalculatorFactory
    {
        public readonly TestDesignWaterLevelCalculator DesignWaterLevelCalculator = new TestDesignWaterLevelCalculator();
        public readonly TestOvertoppingCalculator OvertoppingCalculator = new TestOvertoppingCalculator();
        public readonly TestDikeHeightCalculator DikeHeightCalculator = new TestDikeHeightCalculator();
        public readonly TestOvertoppingRateCalculator OvertoppingRateCalculator = new TestOvertoppingRateCalculator();
        public readonly TestWaveConditionsCosineCalculator WaveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();
        public readonly TestWaveHeightCalculator WaveHeightCalculator = new TestWaveHeightCalculator();
        public readonly TestStructuresOvertoppingCalculator StructuresOvertoppingCalculator = new TestStructuresOvertoppingCalculator();
        public readonly TestStructuresClosureCalculator StructuresClosureCalculator = new TestStructuresClosureCalculator();
        public readonly TestStructuresStabilityPointCalculator StructuresStabilityPointCalculator = new TestStructuresStabilityPointCalculator();
        public readonly TestDunesBoundaryConditionsCalculator DunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();

        public IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(string hlcdDirectory)
        {
            DesignWaterLevelCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return DesignWaterLevelCalculator;
        }

        public IOvertoppingCalculator CreateOvertoppingCalculator(string hlcdDirectory)
        {
            OvertoppingCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return OvertoppingCalculator;
        }

        public IHydraulicLoadsCalculator CreateDikeHeightCalculator(string hlcdDirectory)
        {
            DikeHeightCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return DikeHeightCalculator;
        }

        public IHydraulicLoadsCalculator CreateOvertoppingRateCalculator(string hlcdDirectory)
        {
            OvertoppingRateCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return OvertoppingRateCalculator;
        }

        public IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(string hlcdDirectory)
        {
            WaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return WaveConditionsCosineCalculator;
        }

        public IWaveHeightCalculator CreateWaveHeightCalculator(string hlcdDirectory)
        {
            WaveHeightCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return WaveHeightCalculator;
        }

        public IStructuresOvertoppingCalculator CreateStructuresOvertoppingCalculator(string hlcdDirectory)
        {
            StructuresOvertoppingCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return StructuresOvertoppingCalculator;
        }

        public IStructuresClosureCalculator CreateStructuresClosureCalculator(string hlcdDirectory)
        {
            StructuresClosureCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return StructuresClosureCalculator;
        }

        public IStructuresStabilityPointCalculator CreateStructuresStabilityPointCalculator(string hlcdDirectory)
        {
            StructuresStabilityPointCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return StructuresStabilityPointCalculator;
        }

        public IDunesBoundaryConditionsCalculator CreateDunesBoundaryConditionsCalculator(string hlcdDirectory)
        {
            DunesBoundaryConditionsCalculator.HydraulicBoundaryDatabaseDirectory = hlcdDirectory;
            return DunesBoundaryConditionsCalculator;
        }
    }

    public class TestWaveHeightCalculator : TestHydraRingCalculator<WaveHeightCalculationInput>, IWaveHeightCalculator
    {
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public double WaveHeight { get; set; }
        public double ReliabilityIndex { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestWaveConditionsCosineCalculator : TestHydraRingCalculator<WaveConditionsCosineCalculationInput>, IWaveConditionsCosineCalculator
    {
        public double WaveDirection { get; private set; }
        public double ReliabilityIndex { get; private set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public double WaveHeight { get; set; }
        public double WaveAngle { get; set; }
        public double WavePeakPeriod { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestOvertoppingCalculator : TestHydraRingCalculator<OvertoppingCalculationInput>, IOvertoppingCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public double WaveHeight { get; set; }
        public bool IsOvertoppingDominant { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
    }

    public class TestDikeHeightCalculator : TestHydraRingCalculator<HydraulicLoadsCalculationInput>, IHydraulicLoadsCalculator
    {
        public double Value { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestOvertoppingRateCalculator : TestHydraRingCalculator<HydraulicLoadsCalculationInput>, IHydraulicLoadsCalculator
    {
        public double Value { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestDesignWaterLevelCalculator : TestHydraRingCalculator<AssessmentLevelCalculationInput>, IDesignWaterLevelCalculator
    {
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public double DesignWaterLevel { get; set; }
        public double ReliabilityIndex { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestStructuresOvertoppingCalculator : TestHydraRingCalculator<StructuresOvertoppingCalculationInput>, IStructuresOvertoppingCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
    }

    public class TestStructuresClosureCalculator : TestHydraRingCalculator<StructuresClosureCalculationInput>, IStructuresClosureCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
    }

    public class TestStructuresStabilityPointCalculator : TestHydraRingCalculator<StructuresStabilityPointCalculationInput>, IStructuresStabilityPointCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
    }

    public class TestDunesBoundaryConditionsCalculator : TestHydraRingCalculator<DunesBoundaryConditionsCalculationInput>, IDunesBoundaryConditionsCalculator
    {
        public double WaterLevel { get; set; }
        public double WaveHeight { get; set; }
        public double WavePeriod { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestHydraRingCalculator<T>
    {
        public readonly List<T> ReceivedInputs = new List<T>();
        public event EventHandler CalculationFinishedHandler;
        public string HydraulicBoundaryDatabaseDirectory { get; set; }
        public bool EndInFailure { get; set; }
        public bool IsCanceled { get; set; }

        public void Calculate(T input)
        {
            if (EndInFailure)
            {
                throw new HydraRingCalculationException();
            }
            ReceivedInputs.Add(input);

            CalculationFinished(EventArgs.Empty);
        }

        public void Cancel()
        {
            IsCanceled = true;
        }

        private void CalculationFinished(EventArgs e)
        {
            CalculationFinishedHandler?.Invoke(this, e);
        }
    }
}