// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Data.SQLite;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    public class IllustrationPointsParser : IHydraRingFileParser
    {
        public GeneralResult Output = new GeneralResult();

        private IDictionary<int, WindDirection> windDirections;
        private IDictionary<int, string> closingSituations;

        public void Parse(string workingDirectory, int sectionId)
        {
            string query = string.Concat(
                IllustrationPointQueries.ClosingSituations,
                IllustrationPointQueries.WindDirections,
                IllustrationPointQueries.GeneralAlphaValues,
                IllustrationPointQueries.GeneralBetaValues);

            try
            {
                using (var reader = new HydraRingDatabaseReader(workingDirectory, query, sectionId))
                {
                    ParseResultsFromReader(reader);
                }
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(Resources.Parse_Cannot_read_result_in_output_file, e);
            }
            catch (HydraRingDatabaseReaderException e)
            {
                throw new HydraRingFileParserException("Er konden geen illustratiepunten worden uitgelezen.", e);
            }
        }

        private void ParseResultsFromReader(HydraRingDatabaseReader reader)
        {
            ParseClosingSituations(reader);
            reader.NextResult();
            ParseWindDirections(reader);
            reader.NextResult();
            ParseAlphaValues(reader);
            reader.NextResult();
            ParseBetaValue(reader);
        }

        private void ParseBetaValue(HydraRingDatabaseReader reader)
        {
            IEnumerable<Dictionary<string, object>> betaValues = GetIterator(reader).TakeWhile(r => r != null).ToArray();
            if (betaValues.Count() != 1)
            {
                throw new HydraRingFileParserException("Ongeldig aantal beta-waarden gevonden in de uitvoer database.");
            }
            Output.Beta = Convert.ToDouble(betaValues.First()[IllustrationPointsDatabaseConstants.BetaValue]);
        }

        private void ParseAlphaValues(HydraRingDatabaseReader reader)
        {
            IEnumerable<Dictionary<string, object>> alphaValues = GetIterator(reader).TakeWhile(r => r != null).ToArray();
            Output.Stochasts = alphaValues.Select(a => new Stochast
            {
                Name = Convert.ToString(a[IllustrationPointsDatabaseConstants.StochastName]),
                Duration = Convert.ToDouble(a[IllustrationPointsDatabaseConstants.Duration]),
                Alpha= Convert.ToDouble(a[IllustrationPointsDatabaseConstants.AlphaValue])
            });
        }

        private void ParseClosingSituations(HydraRingDatabaseReader reader)
        {
            IEnumerable<Dictionary<string, object>> readClosingSituations = GetIterator(reader).TakeWhile(r => r != null).ToArray();
            closingSituations = readClosingSituations.ToDictionary(
                r => Convert.ToInt32(r[IllustrationPointsDatabaseConstants.ClosingSituationId]),
                r => Convert.ToString(r[IllustrationPointsDatabaseConstants.ClosingSituationName]));
        }

        private void ParseWindDirections(HydraRingDatabaseReader reader)
        {
            IEnumerable<Dictionary<string, object>> readWindDirections = GetIterator(reader).TakeWhile(r => r != null).ToArray();
            windDirections = new Dictionary<int, WindDirection>();

            foreach (Dictionary<string, object> readWindDirection in readWindDirections)
            {
                int key = Convert.ToInt32(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionId]);
                string name = Convert.ToString(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionName]);
                double angle = Convert.ToDouble(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionAngle]);
                bool isGoverning = Convert.ToBoolean(readWindDirection[IllustrationPointsDatabaseConstants.IsGoverning]);

                var windDirection = new WindDirection
                {
                    Name = name,
                    Angle = angle
                };
                windDirections[key] = windDirection;

                if (isGoverning)
                {
                    Output.GoverningWind = windDirection;
                }
            }
        }

        private static IEnumerable<Dictionary<string, object>> GetIterator(HydraRingDatabaseReader reader)
        {
            Dictionary<string, object> nextLine = reader.ReadLine();
            while (nextLine != null)
            {
                yield return nextLine;
                nextLine = reader.ReadLine();
            }
        }
    }
}