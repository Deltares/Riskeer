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
    /// <summary>
    /// Class for parsing wave condition results from a wave conditions calculation calculation.
    /// </summary>
    public class WaveConditionsCalculationParser : IHydraRingFileParser
    {
        private const string qVariantStart = "Submechanism = Q-variant";
        private const string waveAngle = "Wave angle";
        private const string waveHeight = "Wave height";
        private const string wavePeriod = "Wave period";
        private const string reductionFactor = "reduction factor";
        
        private const char equalsCharacter = '=';

        private readonly List<Result> results;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationParser"/>.
        /// </summary>
        public WaveConditionsCalculationParser()
        {
            results = new List<Result>();
        }

        /// <summary>
        /// Gets the output that was parsed form the output file.
        /// </summary>
        public WaveConditionsCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string fileName = string.Format("{0}{1}", sectionId, HydraRingFileName.OutputFileSuffix);

            try
            {
                ReadFile(Path.Combine(workingDirectory, fileName));
                SetOutput();
            }
            catch
            {
                // ignored
            }
        }

        private void SetOutput()
        {
            if (results != null && results.Any())
            {
                var finalResult = results.Last();

                if (finalResult != null)
                {
                    Output = new WaveConditionsCalculationOutput(finalResult.WaveHeight, finalResult.WavePeriod, finalResult.WaveAngle);
                }
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
                        TryParseQVariant(currentLine, file);
                    }
                }
            }
        }

        private void TryParseQVariant(string startLine, StreamReader file)
        {
            if (startLine.Contains(qVariantStart))
            {
                var result = new Result();
                while (!file.EndOfStream)
                {
                    string readLine = file.ReadLine();
                    TryParseWaveAngle(readLine, result);
                    TryParseWaveHeight(readLine, result);
                    TryParseWavePeriod(readLine, result);
                }
                results.Add(result);
            }
        }

        private void TryParseWaveAngle(string line, Result result)
        {
            if (line.Contains(waveAngle) && !line.Contains(reductionFactor))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                result.WaveAngle= ParseStringResult(resultAsString);
            }
        }

        private void TryParseWaveHeight(string line, Result result)
        {
            if (line.Contains(waveHeight))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                result.WaveHeight = ParseStringResult(resultAsString);
            }
        }

        private void TryParseWavePeriod(string line, Result result)
        {
            if (line.Contains(wavePeriod))
            {
                string resultAsString = line.Split(equalsCharacter)[1].Trim();
                result.WavePeriod = ParseStringResult(resultAsString);
            }
        }

        private static double ParseStringResult(string resultAsString)
        {
            return double.Parse(resultAsString, CultureInfo.InvariantCulture);
        }

        private class Result
        {
            public double WaveHeight { get; set; }
            public double WavePeriod { get; set; }
            public double WaveAngle { get; set; }
        }
    }
}