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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    public class OvertoppingCalculator : HydraRingCalculator, IOvertoppingCalculator
    {
        private readonly string hlcdDirectory;
        private readonly string ringId;
        private readonly ExceedanceProbabilityCalculationExceptionParser exceedanceProbabilityCalculationParser;
        private readonly OvertoppingCalculationWaveHeightExceptionParser waveHeightParser;

        internal OvertoppingCalculator(string hlcdDirectory, string ringId)
        {
            this.hlcdDirectory = hlcdDirectory;
            this.ringId = ringId;
            exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationExceptionParser();
            waveHeightParser = new OvertoppingCalculationWaveHeightExceptionParser();

            ExceedanceProbabilityBeta = double.NaN;
            WaveHeight = double.NaN;
            IsOvertoppingDominant = false;
        }

        public double ExceedanceProbabilityBeta { get; private set; }
        public double WaveHeight { get; private set; }
        public bool IsOvertoppingDominant { get; private set; }

        public void Calculate(OvertoppingCalculationInput input)
        {
            Calculate(hlcdDirectory, ringId, HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return exceedanceProbabilityCalculationParser;
            yield return waveHeightParser;
        }

        protected override void SetOutputs()
        {
            if (exceedanceProbabilityCalculationParser.Output != null)
            {
                ExceedanceProbabilityBeta = exceedanceProbabilityCalculationParser.Output.Beta;
            }
            if (waveHeightParser.Output != null)
            {
                WaveHeight = waveHeightParser.Output.WaveHeight;
            }
            IsOvertoppingDominant = waveHeightParser.Output != null && waveHeightParser.Output.IsOvertoppingDominant;
        }
    }
}