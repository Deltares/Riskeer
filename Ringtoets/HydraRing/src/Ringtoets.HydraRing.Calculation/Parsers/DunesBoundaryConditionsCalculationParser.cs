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

using System.Globalization;
using System.IO;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Class for parsing dunes boundary condition results from a dunes boundary conditions calculation.
    /// </summary>
    public class DunesBoundaryConditionsCalculationParser : IHydraRingFileParser
    {
        private const string waterLevelText = "Considered water level";
        private const string waveHeightText = "Computed wave height";
        private const string wavePeriodText = "Computed wave period";

        private const char equalsCharacter = '=';

        private double? waterLevel;
        private double? waveHeight;
        private double? wavePeriod;
        /// <summary>
        /// Gets the output that was parsed from the output file.
        /// </summary>
        public DunesBoundaryConditionsCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string fileName = string.Format("{0}{1}", sectionId, HydraRingFileConstants.OutputFileSuffix);

            try
            {
                ReadFile(Path.Combine(workingDirectory, fileName));
                SetOutput();
            }
            catch
            {
                throw new HydraRingFileParserException();
            }
        }

        private void SetOutput()
        {
            if (waterLevel != null && waveHeight != null && wavePeriod != null)
            {
                Output = new DunesBoundaryConditionsCalculationOutput(waterLevel.Value,
                                                                      waveHeight.Value,
                                                                      wavePeriod.Value);
            }
        }

        private void ReadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var file = new StreamReader(File.OpenRead(filePath)))
                {
                    while (!file.EndOfStream)
                    {
                        string currentLine = file.ReadLine();

                        waterLevel = TryParseWaterLevel(currentLine) ?? waterLevel;
                        waveHeight = TryParseWaveHeight(currentLine) ?? waveHeight;
                        wavePeriod = TryParseWavePeriod(currentLine) ?? wavePeriod;
                    }
                }
            }
        }

        private static double? TryParseWaterLevel(string line)
        {
            if (line.Contains(waterLevelText))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                return ParseStringResult(resultAsString);
            }
            return null;
        }

        private static double? TryParseWaveHeight(string line)
        {
            if (line.Contains(waveHeightText))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                return ParseStringResult(resultAsString);
            }
            return null;
        }

        private static double? TryParseWavePeriod(string line)
        {
            if (line.Contains(wavePeriodText))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                return ParseStringResult(resultAsString);
            }
            return null;
        }

        private static double ParseStringResult(string resultAsString)
        {
            return double.Parse(resultAsString, CultureInfo.InvariantCulture);
        }
    }
}