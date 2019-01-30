// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    public class TestWaveHeightCalculator : TestHydraRingCalculator<WaveHeightCalculationInput>, IWaveHeightCalculator
    {
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public double WaveHeight { get; set; }
        public double ReliabilityIndex { get; set; }
        public bool? Converged { get; set; }
        public string IllustrationPointsParserErrorMessage { get; set; }
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
        public string IllustrationPointsParserErrorMessage { get; set; }
    }

    public class TestHydraulicLoadsCalculator : TestHydraRingCalculator<HydraulicLoadsCalculationInput>, IHydraulicLoadsCalculator
    {
        public double Value { get; set; }
        public double ReliabilityIndex { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public string IllustrationPointsParserErrorMessage { get; set; }
        public bool? Converged { get; set; }
    }

    public class TestDesignWaterLevelCalculator : TestHydraRingCalculator<AssessmentLevelCalculationInput>, IDesignWaterLevelCalculator
    {
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public double DesignWaterLevel { get; set; }
        public double ReliabilityIndex { get; set; }
        public bool? Converged { get; set; }
        public string IllustrationPointsParserErrorMessage { get; set; }
    }

    public class TestStructuresCalculator<TInput> : TestHydraRingCalculator<TInput>, IStructuresCalculator<TInput>
        where TInput : ExceedanceProbabilityCalculationInput
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public string IllustrationPointsParserErrorMessage { get; set; }
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
        public readonly HydraRingCalculationException HydraRingCalculationException = new HydraRingCalculationException();
        private readonly List<T> receivedInputs = new List<T>();
        public event EventHandler CalculationFinishedHandler;

        public bool EndInFailure { get; set; }
        public bool IsCanceled { get; private set; }

        public IEnumerable<T> ReceivedInputs
        {
            get
            {
                return receivedInputs;
            }
        }

        public GeneralResult IllustrationPointsResult { get; set; }

        public void Calculate(T input)
        {
            if (EndInFailure)
            {
                throw HydraRingCalculationException;
            }

            receivedInputs.Add(input);

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