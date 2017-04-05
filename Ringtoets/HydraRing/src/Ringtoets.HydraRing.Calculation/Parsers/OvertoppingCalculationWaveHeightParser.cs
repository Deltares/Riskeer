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
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for overtopping wave height results.
    /// </summary>
    public class OvertoppingCalculationWaveHeightParser : IHydraRingFileParser
    {
        private const string overtoppingStart = "Submechanism = Overtopping RTO";
        private const string overflowStart = "Submechanism = Overflow";
        private const string combineWindDirectionStart = "Status = Combined over wind directions";
        private const string governingWindDirectionStart = "Governing wind direction";
        private const string overtoppingWaveHeight = "Wave height (Hs)";
        private const string windDirection = "Wind direction";
        private const string closingSituation = "Closing situation";
        private const string beta = "Beta";
        private const char equalsCharacter = '=';

        private readonly List<OvertoppingResult> overtoppingResults;
        private readonly List<GeneralResult> overflowResults;
        private int governingWindDirection = -1;

        /// <summary>
        /// Creates a new instance of <see cref="OvertoppingCalculationWaveHeightParser"/>.
        /// </summary>
        public OvertoppingCalculationWaveHeightParser()
        {
            overtoppingResults = new List<OvertoppingResult>();
            overflowResults = new List<GeneralResult>();
        }

        /// <summary>
        /// Gets the output that was parsed from the output file.
        /// </summary>
        public OvertoppingCalculationWaveHeightOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string fileName = string.Format("{0}{1}", sectionId, HydraRingFileConstants.OutputFileSuffix);

            try
            {
                ReadFile(Path.Combine(workingDirectory, fileName));
                SetOutputs();
            }
            catch
            {
                throw new HydraRingFileParserException();
            }
        }

        private void SetOutputs()
        {
            OvertoppingResult relevantOvertoppingResult = null;
            foreach (OvertoppingResult overtoppingResult in overtoppingResults.Where(o => o.WindDirection == governingWindDirection))
            {
                if (relevantOvertoppingResult == null || overtoppingResult.Beta < relevantOvertoppingResult.Beta)
                {
                    relevantOvertoppingResult = overtoppingResult;
                }
            }

            if (relevantOvertoppingResult != null && overflowResults.Any())
            {
                GeneralResult governingOverflowResult = overflowResults
                    .First(o => o.WindDirection == governingWindDirection && o.ClosingSituation == relevantOvertoppingResult.ClosingSituation);

                Output = new OvertoppingCalculationWaveHeightOutput(
                    relevantOvertoppingResult.WaveHeight,
                    relevantOvertoppingResult.Beta < governingOverflowResult.Beta);
            }
        }

        private void ReadFile(string filePath)
        {
            using (var file = new StreamReader(File.OpenRead(filePath)))
            {
                while (!file.EndOfStream)
                {
                    string currentLine = file.ReadLine();
                    TryParseOvertoppingSection(currentLine, file);
                    TryParseOverflowSection(currentLine, file);
                    TryParseCombinationWindDirectionSection(currentLine, file);
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
                    string readLine = file.ReadLine();
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
                    string readLine = file.ReadLine();
                    TryParseWindDirection(readLine, overflowResult);
                    TryParseClosingSituation(readLine, overflowResult);
                    TryParseBeta(readLine, overflowResult);
                }
                overflowResults.Add(overflowResult);
            }
        }

        private void TryParseCombinationWindDirectionSection(string startLine, StreamReader file)
        {
            if (startLine.Contains(combineWindDirectionStart))
            {
                while (!file.EndOfStream && governingWindDirection == -1)
                {
                    string readLine = file.ReadLine();
                    TryParseGoverningWindDirection(readLine);
                }
            }
        }

        private void TryParseGoverningWindDirection(string line)
        {
            if (line.Contains(governingWindDirectionStart))
            {
                string governingWindDirectionString = line.Split(equalsCharacter)[1].Trim();
                governingWindDirection = int.Parse(governingWindDirectionString);
            }
        }

        private void TryParseWaveHeight(string line, OvertoppingResult overtoppingResult)
        {
            if (line.Contains(overtoppingWaveHeight))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                overtoppingResult.WaveHeight = double.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseWindDirection(string line, GeneralResult generalResult)
        {
            if (line.Contains(windDirection))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.WindDirection = int.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseClosingSituation(string line, GeneralResult generalResult)
        {
            if (line.Contains(closingSituation))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.ClosingSituation = int.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private void TryParseBeta(string line, GeneralResult generalResult)
        {
            if (line.Contains(beta))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                generalResult.Beta = double.Parse(resultAsString, CultureInfo.InvariantCulture);
            }
        }

        private class GeneralResult
        {
            public GeneralResult()
            {
                Beta = double.NaN;
            }

            public int WindDirection { get; set; }
            public int ClosingSituation { get; set; }
            public double Beta { get; set; }
        }

        private class OvertoppingResult : GeneralResult
        {
            public double WaveHeight { get; set; }
        }
    }
}