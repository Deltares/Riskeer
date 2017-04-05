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
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the dunes boundary conditions of Hydra-Ring calculations.
    /// </summary>
    public class DunesBoundaryConditionsCalculationParser : IHydraRingFileParser
    {
        private const string sectionIdParameterName = "@sectionId";
        private const string waveHeightColumnName = "WaveHeight";
        private const string wavePeriodColumnName = "WavePeriod";
        private const string waterLevelColumnName = "WaterLevel";

        private readonly string query = "SELECT " +
                                        $"max(case when OutputVarId is 3 then d.Value end) {waveHeightColumnName}, " +
                                        $"max(case when OutputVarId is 4 then d.Value end) {wavePeriodColumnName}, " +
                                        $"max(case when OutputVarId is 23 then d.Value end) {waterLevelColumnName} " +
                                        "FROM DesignPointResults as d " +
                                        $"WHERE SectionId = {sectionIdParameterName} " +
                                        "GROUP BY OuterIterationId " +
                                        "ORDER BY OuterIterationId DESC " +
                                        "LIMIT 1;";

        /// <summary>
        /// Gets the output that was parsed from the output file.
        /// </summary>
        public DunesBoundaryConditionsCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            HydraRingDatabaseParseHelper.Parse(workingDirectory,
                                               query,
                                               sectionId,
                                               Resources.DunesBoundaryConditionsCalculationParser_Parse_No_dunes_hydraulic_boundaries_found_in_output_file,
                                               ReadResult);
        }

        /// <summary>
        /// Reads the result of the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to get the result from.</param>
        /// <exception cref="InvalidCastException">Thrown when the result
        /// cannot be converted to the output format.</exception>
        private void ReadResult(HydraRingDatabaseReader reader)
        {
            double waveHeight = Convert.ToDouble(reader.ReadColumn(waveHeightColumnName));
            double wavePeriod = Convert.ToDouble(reader.ReadColumn(wavePeriodColumnName));
            double waterLevel = Convert.ToDouble(reader.ReadColumn(waterLevelColumnName));

            Output = new DunesBoundaryConditionsCalculationOutput(waterLevel,
                                                                  waveHeight,
                                                                  wavePeriod);
        }
    }
}