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
using System.Globalization;
using System.IO;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    public class WaveHeightCalculationParser : IHydraRingFileParser
    {
        private class GeneralResult
        {
            public int WindDirection;
            public int ClosingSituation;
            public double Beta;

            public GeneralResult()
            {
                Beta = double.NaN;
            }
        }

        private class OvertoppingResult : GeneralResult
        {
            public double WaveHeight;
        }
        
        private const string overtoppingStart = "Submechanism = Overtopping RTO";
        private const string overflowStart = "Submechanism = Overflow";
        private const string combineWindDirectionStart = "Status = Combined over wind directions";
        private const string overtoppingWaveHeight = "Wave height (Hs)";
        private const string windDirection = "Wind direction";
        private const string closingSituation = "Closing situation";
        private const string beta = "Beta";
        private const char equalsCharacter = '=';

        private readonly List<OvertoppingResult> overtoppingResults;
        private readonly List<GeneralResult> overflowResults;
        private int governingWindDirection;

        public WaveHeightCalculationParser()
        {
            overtoppingResults = new List<OvertoppingResult>();
            overflowResults = new List<GeneralResult>();
        }

        public WaveHeightCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            var fileName = string.Format("{0}{1}", sectionId, HydraRingFileName.OutputFileSuffix);
            var filePath = Path.Combine(workingDirectory, fileName);

            try
            {
                ReadFile(filePath);
                SetOutputs();
            }
            catch
            {
                // ignored
            }
        }

        private void SetOutputs()
        {
            if (overtoppingResults.Any() && overflowResults.Any())
            {
                OvertoppingResult[] governingOvertoppingResults = overtoppingResults.Where(o => o.WindDirection == governingWindDirection).ToArray();
                double minBeta = governingOvertoppingResults.Min(o => o.Beta);

                OvertoppingResult relevantOvertoppingResult = governingOvertoppingResults.First(o => o.Beta.Equals(minBeta));
                GeneralResult governingOverflowResult = overflowResults
                    .First(o => o.WindDirection == governingWindDirection && o.ClosingSituation == relevantOvertoppingResult.ClosingSituation);

                Output = new WaveHeightCalculationOutput(
                    relevantOvertoppingResult.WaveHeight,
                    relevantOvertoppingResult.Beta < governingOverflowResult.Beta);
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
                        var currentLine = file.ReadLine();
                        TryParseOvertoppingSection(currentLine, file);
                        TryParseOverflowSection(currentLine, file);
                        TryParseGoverningWindDirection(currentLine, file);
                    }
                }
            }
        }

        private void TryParseOvertoppingSection(string startLine, StreamReader file)
        {
            if (startLine.Contains(overtoppingStart))
            {
                var overtoppingResult = new OvertoppingResult();
                while (!file.EndOfStream && double.IsNaN(overtoppingResult.Beta))
                {
                    var readLine = file.ReadLine();
                    TryParseWaveHeight(readLine, overtoppingResult);
                    TryParseWindDirection(readLine, overtoppingResult);
                    TryParseClosingSituation(readLine, overtoppingResult);
                    TryParseBeta(readLine, overtoppingResult);
                }
                overtoppingResults.Add(overtoppingResult);
            }
        }

        private void TryParseOverflowSection(string startLine, StreamReader file)
        {
            if (startLine.Contains(overflowStart))
            {
                var overflowResult = new GeneralResult();
                while (!file.EndOfStream && double.IsNaN(overflowResult.Beta))
                {
                    var readLine = file.ReadLine();
                    TryParseWindDirection(readLine, overflowResult);
                    TryParseClosingSituation(readLine, overflowResult);
                    TryParseBeta(readLine, overflowResult);
                }
                overflowResults.Add(overflowResult);
            }
        }

        private void TryParseGoverningWindDirection(string startLine, StreamReader file)
        {
            if (startLine.Contains(combineWindDirectionStart))
            {
                var line = file.ReadLine();
                var governingWindDirectionString = line.Split(equalsCharacter)[1].Trim();
                governingWindDirection = int.Parse(governingWindDirectionString);
            }
        }

        private void TryParseWaveHeight(string line, OvertoppingResult overtoppingResult)
        {
            if (line.Contains(overtoppingWaveHeight))
            {
                var resultAsString = line.Split(equalsCharacter)[1].Trim();
                overtoppingResult.WaveHeight = double.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseWindDirection(string line, GeneralResult generalResult)
        {
            if (line.Contains(windDirection))
            {
                var resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.WindDirection = int.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseClosingSituation(string line, GeneralResult generalResult)
        {
            if (line.Contains(closingSituation))
            {
                var resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.ClosingSituation = int.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseBeta(string line, GeneralResult generalResult)
        {
            if (line.Contains(beta))
            {
                var resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.Beta = double.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }
    }
}