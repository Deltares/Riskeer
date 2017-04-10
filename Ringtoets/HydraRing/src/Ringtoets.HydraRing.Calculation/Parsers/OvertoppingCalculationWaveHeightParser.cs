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
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for overtopping results.
    /// </summary>
    public class OvertoppingCalculationWaveHeightParser : IHydraRingFileParser
    {
        private const string sectionIdParameterName = "@sectionId";
        private const string waveHeightColumn = "WaveHeight";
        private const string isOvertoppingDominantColumn = "IsOvertoppingDominant";

        private readonly string query =
            $"SELECT Value as {waveHeightColumn}," +
            $"(case when SubMechanismId is 102 then 1 else 0 end) as {isOvertoppingDominantColumn} FROM " +
            "(SELECT d.OuterIterationId, TidalPeriod, BetaValue, ClosingSituation, WindDirection, LevelType, SubMechanismId " +
            "FROM GoverningWind g " +
            "JOIN DesignBeta d ON d.WindDirection = g.GoverningWind AND d.OuterIterationId = g.OuterIterationId AND d.TidalPeriod = g.PeriodId " +
            $"WHERE LevelType = 7 AND d.SubMechanismId = 102 AND SectionId = {sectionIdParameterName} " +
            "ORDER BY d.OuterIterationId DESC, TidalPeriod, BetaValue " +
            "LIMIT 1) as g " +
            "JOIN " +
            "(SELECT Value FROM GoverningWind g " +
            "JOIN DesignPointResults d ON d.WindDirection = g.GoverningWind AND d.OuterIterationId = g.OuterIterationId AND d.TidalPeriod = g.PeriodId " +
            "JOIN DesignBeta db ON db.WindDirection = d.WindDirection AND db.ClosingSituation = d.ClosingSituation AND db.OuterIterationId = d.OuterIterationId " +
            "AND db.TidalPeriod = d.TidalPeriod " +
            $"WHERE OutputVarId = 3 AND db.LevelType = 7 AND db.SectionId = {sectionIdParameterName} " +
            "ORDER BY d.OuterIterationId DESC, d.TidalPeriod, db.BetaValue " +
            "LIMIT 1) " +
            "Order By g.BetaValue " +
            "LIMIT 1;";

        /// <summary>
        /// Gets the output that was parsed from the output file.
        /// </summary>
        public OvertoppingCalculationWaveHeightOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            Dictionary<string, object> result = HydraRingDatabaseParseHelper.ReadSingleLine(
                workingDirectory,
                query,
                sectionId,
                Resources.OvertoppingCalculationWaveHeightParser_No_overtopping_found_in_output_file);

            ReadResult(result);
        }

        /// <summary>
        /// Reads the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result from the database read.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the result
        /// cannot be converted to the output format.</exception>
        private void ReadResult(IDictionary<string, object> result)
        {
            try
            {
                double waveHeight = Convert.ToDouble(result[waveHeightColumn]);
                bool isOvertoppingDominant = Convert.ToBoolean(result[isOvertoppingDominantColumn]);

                Output = new OvertoppingCalculationWaveHeightOutput(waveHeight,
                                                                    isOvertoppingDominant);
            }
            catch (InvalidCastException e)
            {
                throw new HydraRingFileParserException(Resources.OvertoppingCalculationWaveHeightParser_No_overtopping_found_in_output_file, e);
            }
        }
    }
}