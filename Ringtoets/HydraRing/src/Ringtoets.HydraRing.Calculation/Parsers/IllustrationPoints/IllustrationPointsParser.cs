// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.Properties;
using Riskeer.HydraRing.Calculation.Readers;

namespace Riskeer.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// Parser for transforming values for illustration points from the database into a data structure.
    /// </summary>
    public class IllustrationPointsParser : IHydraRingFileParser
    {
        private readonly Dictionary<ThreeKeyIndex, List<Stochast>> faultTreeStochasts = new Dictionary<ThreeKeyIndex, List<Stochast>>();
        private readonly Dictionary<ThreeKeyIndex, double> faultTreeBetaValues = new Dictionary<ThreeKeyIndex, double>();

        private readonly Dictionary<ThreeKeyIndex, List<SubMechanismIllustrationPointStochast>> subMechanismStochasts = new Dictionary<ThreeKeyIndex, List<SubMechanismIllustrationPointStochast>>();
        private readonly Dictionary<ThreeKeyIndex, double> subMechanismBetaValues = new Dictionary<ThreeKeyIndex, double>();
        private readonly Dictionary<ThreeKeyIndex, List<IllustrationPointResult>> subMechanismResults = new Dictionary<ThreeKeyIndex, List<IllustrationPointResult>>();

        private double beta = double.NaN;
        private WindDirection governingWindDirection;
        private IEnumerable<Stochast> stochasts;

        private IDictionary<int, WindDirection> windDirections;
        private IDictionary<int, string> closingSituations;
        private IDictionary<int, string> subMechanisms;
        private IDictionary<int, string> faultTrees;

        /// <summary>
        /// The result of parsing the illustration points in the Hydra-Ring database.
        /// </summary>
        public GeneralResult Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string query = string.Concat(
                IllustrationPointQueries.ClosingSituations,
                IllustrationPointQueries.WindDirections,
                IllustrationPointQueries.SubMechanisms,
                IllustrationPointQueries.FaultTrees,
                IllustrationPointQueries.GeneralAlphaValues,
                IllustrationPointQueries.GeneralBetaValues,
                IllustrationPointQueries.FaultTreeAlphaValues,
                IllustrationPointQueries.FaultTreeBetaValues,
                IllustrationPointQueries.SubMechanismAlphaValues,
                IllustrationPointQueries.SubMechanismBetaValues,
                IllustrationPointQueries.SubMechanismIllustrationPointResults,
                IllustrationPointQueries.RecursiveFaultTree);

            try
            {
                using (var reader = new HydraRingDatabaseReader(workingDirectory, query, sectionId))
                {
                    ParseResultsFromReader(reader);
                }
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Could_not_read_illustration_point_data, e);
            }
        }

        /// <summary>
        /// Parses the illustration point database results.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when parsing the results from <paramref name="reader"/>.</exception>
        private void ParseResultsFromReader(HydraRingDatabaseReader reader)
        {
            ParseClosingSituations(reader);
            ProceedOrThrow(reader);
            ParseWindDirections(reader);
            ProceedOrThrow(reader);
            ParseSubMechanisms(reader);
            ProceedOrThrow(reader);
            ParseFaultTrees(reader);
            ProceedOrThrow(reader);
            ParseGeneralAlphaValues(reader);
            ProceedOrThrow(reader);
            ParseGeneralBetaValue(reader);
            ProceedOrThrow(reader);
            ParseFaultTreeAlphaValues(reader);
            ProceedOrThrow(reader);
            ParseFaultTreeBetaValues(reader);
            ProceedOrThrow(reader);
            ParseSubMechanismAlphaValues(reader);
            ProceedOrThrow(reader);
            ParseSubMechanismBetaValues(reader);
            ProceedOrThrow(reader);
            ParseSubMechanismResults(reader);
            ProceedOrThrow(reader);
            Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> rootIllustrationPoints =
                ParseFaultTree(reader)
                ?? GetSubMechanismAsRootIllustrationPoint();

            Output = new GeneralResult(beta, governingWindDirection, stochasts, rootIllustrationPoints);
        }

        /// <summary>
        /// Proceeds <paramref name="reader"/> to the next result in the data set. 
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown there was no other result in the data set.</exception>
        private static void ProceedOrThrow(HydraRingDatabaseReader reader)
        {
            if (!reader.NextResult())
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Could_not_read_illustration_point_data);
            }
        }

        /// <summary>
        /// Parses <see cref="Stochast"/> objects from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the read <see cref="IllustrationPointsDatabaseConstants.AlphaValue"/>
        /// or <see cref="IllustrationPointsDatabaseConstants.Duration"/> is <see cref="DBNull"/>.</exception>
        private void ParseFaultTreeAlphaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readFaultTreeAlphaValue in GetIterator(reader))
            {
                int faultTreeId = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.FaultTreeId]);
                int windDirectionId = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string name = Convert.ToString(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.StochastName]);
                double duration = ConvertToDouble(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.Duration],
                                                  IllustrationPointsDatabaseConstants.Duration);
                double alpha = ConvertToDouble(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.AlphaValue],
                                               IllustrationPointsDatabaseConstants.AlphaValue);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, faultTreeId);
                if (!faultTreeStochasts.ContainsKey(key))
                {
                    faultTreeStochasts[key] = new List<Stochast>();
                }

                faultTreeStochasts[key].Add(new Stochast(name, duration, alpha));
            }
        }

        /// <summary>
        /// Parses fault tree beta values from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when:
        /// <list type="bullet">
        /// <item>The read <see cref="IllustrationPointsDatabaseConstants.BetaValue"/> is <see cref="DBNull"/>;</item>
        /// <item>Multiple values for beta of illustration point found.</item>
        /// </list>
        /// </exception>
        private void ParseFaultTreeBetaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readFaultTreeBetaValue in GetIterator(reader))
            {
                int faultTreeId = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.FaultTreeId]);
                int windDirectionId = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                double faultTreeBeta = ConvertToDouble(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.BetaValue],
                                                       IllustrationPointsDatabaseConstants.BetaValue);
                var threeKeyIndex = new ThreeKeyIndex(windDirectionId, closingSituationid, faultTreeId);
                if (faultTreeBetaValues.ContainsKey(threeKeyIndex))
                {
                    throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
                }

                faultTreeBetaValues[threeKeyIndex] = faultTreeBeta;
            }
        }

        /// <summary>
        /// Parses <see cref="SubMechanismIllustrationPointStochast"/> objects from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the read <see cref="IllustrationPointsDatabaseConstants.Duration"/>, 
        /// <see cref="IllustrationPointsDatabaseConstants.AlphaValue"/>, or <see cref="IllustrationPointsDatabaseConstants.Realization"/>
        /// is <see cref="DBNull"/>.</exception>
        private void ParseSubMechanismAlphaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismAlphaValue in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string name = Convert.ToString(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.StochastName]);
                double duration = ConvertToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.Duration],
                                                  IllustrationPointsDatabaseConstants.Duration);
                double alpha = ConvertToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.AlphaValue],
                                               IllustrationPointsDatabaseConstants.AlphaValue);
                double realization = ConvertToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.Realization],
                                                     IllustrationPointsDatabaseConstants.Realization);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (!subMechanismStochasts.ContainsKey(key))
                {
                    subMechanismStochasts[key] = new List<SubMechanismIllustrationPointStochast>();
                }

                subMechanismStochasts[key].Add(new SubMechanismIllustrationPointStochast(name, duration, alpha, realization));
            }
        }

        /// <summary>
        /// Parses sub mechanism beta values from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when:
        /// <list type="bullet">
        /// <item>The read <see cref="IllustrationPointsDatabaseConstants.BetaValue"/> is <see cref="DBNull"/>;</item>
        /// <item>Multiple values for beta of illustration point found.</item>
        /// </list>
        /// </exception>
        private void ParseSubMechanismBetaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismBetaValue in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                double subMechanismBeta = ConvertToDouble(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.BetaValue],
                                                          IllustrationPointsDatabaseConstants.BetaValue);

                var threeKeyIndex = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (subMechanismBetaValues.ContainsKey(threeKeyIndex))
                {
                    throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
                }

                subMechanismBetaValues[threeKeyIndex] = subMechanismBeta;
            }
        }

        /// <summary>
        /// Parses <see cref="IllustrationPointResult"/> objects from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the read <see cref="IllustrationPointsDatabaseConstants.IllustrationPointResultValue"/> 
        /// is <see cref="DBNull"/>.</exception>
        private void ParseSubMechanismResults(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismResult in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string description = Convert.ToString(readSubMechanismResult[IllustrationPointsDatabaseConstants.IllustrationPointResultDescription]);
                double value = ConvertToDouble(readSubMechanismResult[IllustrationPointsDatabaseConstants.IllustrationPointResultValue],
                                               IllustrationPointsDatabaseConstants.IllustrationPointResultValue);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (!subMechanismResults.ContainsKey(key))
                {
                    subMechanismResults[key] = new List<IllustrationPointResult>();
                }

                subMechanismResults[key].Add(new IllustrationPointResult(description, value));
            }
        }

        private Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> ParseFaultTree(HydraRingDatabaseReader reader)
        {
            IEnumerable<Tuple<int, WindDirection, int, string>> windDirectionClosingSituations =
                GetAllWindDirectionClosingSituationCombinations();

            Dictionary<string, object>[] readFaultTrees = GetIterator(reader).ToArray();
            if (readFaultTrees.Length > 0)
            {
                IEnumerable<Tuple<int?, int, Type, CombinationType>> results = CreateResultTuples(readFaultTrees);

                var rootIllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
                foreach (Tuple<int, WindDirection, int, string> windDirectionClosingSituation in windDirectionClosingSituations)
                {
                    Tuple<int?, int, Type, CombinationType> root = results.Single(r => !r.Item1.HasValue);

                    IllustrationPointTreeNode illustrationPointTreeNode = BuildFaultTree(windDirectionClosingSituation, root.Item2, root.Item4, results);

                    if (illustrationPointTreeNode != null)
                    {
                        rootIllustrationPoints[CreateFaultTreeKey(windDirectionClosingSituation)] = illustrationPointTreeNode;
                    }
                }

                return rootIllustrationPoints;
            }

            return null;
        }

        private Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> GetSubMechanismAsRootIllustrationPoint()
        {
            var rootIllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
            int subMechanismId = subMechanisms.First().Key;

            foreach (Tuple<int, WindDirection, int, string> windDirectionClosingSituation in GetAllWindDirectionClosingSituationCombinations())
            {
                IllustrationPointTreeNode illustrationPointTreeNode = BuildSubMechanism(
                    windDirectionClosingSituation,
                    subMechanismId);

                if (illustrationPointTreeNode != null)
                {
                    rootIllustrationPoints[CreateFaultTreeKey(windDirectionClosingSituation)] = illustrationPointTreeNode;
                }
            }

            return rootIllustrationPoints;
        }

        private static IEnumerable<Tuple<int?, int, Type, CombinationType>> CreateResultTuples(IEnumerable<Dictionary<string, object>> readFaultTrees)
        {
            var results = new List<Tuple<int?, int, Type, CombinationType>>();

            foreach (Dictionary<string, object> readFaultTree in readFaultTrees)
            {
                object parentIdObject = readFaultTree[IllustrationPointsDatabaseConstants.RecursiveFaultTreeParentId];
                int? parentId = parentIdObject != DBNull.Value ? Convert.ToInt32(parentIdObject) : (int?) null;
                int id = Convert.ToInt32(readFaultTree[IllustrationPointsDatabaseConstants.RecursiveFaultTreeId]);
                string type = Convert.ToString(readFaultTree[IllustrationPointsDatabaseConstants.RecursiveFaultTreeType]);
                string combine = Convert.ToString(readFaultTree[IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine]);

                results.Add(Tuple.Create(
                                parentId,
                                id,
                                type == "faulttree" ? typeof(FaultTreeIllustrationPoint) : typeof(SubMechanismIllustrationPoint),
                                combine == "and" ? CombinationType.And : CombinationType.Or));
            }

            return results;
        }

        private static WindDirectionClosingSituation CreateFaultTreeKey(Tuple<int, WindDirection, int, string> windDirectionClosingSituation)
        {
            return new WindDirectionClosingSituation(windDirectionClosingSituation.Item2, windDirectionClosingSituation.Item4);
        }

        private IEnumerable<Tuple<int, WindDirection, int, string>> GetAllWindDirectionClosingSituationCombinations()
        {
            return windDirections.SelectMany(windDirection =>
                                                 closingSituations.Select(
                                                     closingSituation => Tuple.Create(windDirection.Key, windDirection.Value, closingSituation.Key, closingSituation.Value)))
                                 .ToArray();
        }

        private IllustrationPointTreeNode BuildFaultTree(
            Tuple<int, WindDirection, int, string> windDirectionClosingSituation,
            int faultTreeId,
            CombinationType combinationType,
            IEnumerable<Tuple<int?, int, Type, CombinationType>> results)
        {
            var dataKey = new ThreeKeyIndex(windDirectionClosingSituation.Item1, windDirectionClosingSituation.Item3, faultTreeId);
            var faultTreeIllustrationPointStochasts = new List<Stochast>();

            if (!faultTreeBetaValues.ContainsKey(dataKey))
            {
                return null;
            }

            if (faultTreeStochasts.ContainsKey(dataKey))
            {
                AddRange(faultTreeIllustrationPointStochasts, faultTreeStochasts[dataKey]);
            }

            var illustrationPoint = new FaultTreeIllustrationPoint(faultTrees[faultTreeId],
                                                                   faultTreeBetaValues[dataKey],
                                                                   faultTreeIllustrationPointStochasts,
                                                                   combinationType);

            var node = new IllustrationPointTreeNode(illustrationPoint);
            node.SetChildren(results.Where(r => r.Item1 == faultTreeId)
                                    .Select(child => child.Item3 == typeof(FaultTreeIllustrationPoint)
                                                         ? BuildFaultTree(windDirectionClosingSituation, child.Item2, child.Item4, results)
                                                         : BuildSubMechanism(windDirectionClosingSituation, child.Item2)).ToArray());
            return node;
        }

        private IllustrationPointTreeNode BuildSubMechanism(Tuple<int, WindDirection, int, string> windDirectionClosingSituation, int subMechanismId)
        {
            var dataKey = new ThreeKeyIndex(windDirectionClosingSituation.Item1, windDirectionClosingSituation.Item3, subMechanismId);

            var illustrationPointStochasts = new List<SubMechanismIllustrationPointStochast>();
            var illustrationPointResults = new List<IllustrationPointResult>();

            if (!subMechanismBetaValues.ContainsKey(dataKey))
            {
                return null;
            }

            double subMechanismIllustrationPointBeta = subMechanismBetaValues[dataKey];

            if (subMechanismStochasts.ContainsKey(dataKey))
            {
                AddRange(illustrationPointStochasts, subMechanismStochasts[dataKey]);
            }

            if (subMechanismResults.ContainsKey(dataKey))
            {
                AddRange(illustrationPointResults, subMechanismResults[dataKey]);
            }

            string subMechanismIllustrationPointName = subMechanisms[subMechanismId];
            var illustrationPoint = new SubMechanismIllustrationPoint(subMechanismIllustrationPointName,
                                                                      illustrationPointStochasts,
                                                                      illustrationPointResults,
                                                                      subMechanismIllustrationPointBeta);

            return new IllustrationPointTreeNode(illustrationPoint);
        }

        private static void AddRange<T>(ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            foreach (T item in itemsToAdd)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Parses beta values from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when:
        /// <list type="bullet">
        /// <item>The read <see cref="IllustrationPointsDatabaseConstants.BetaValue"/> is <see cref="DBNull"/>;</item>
        /// <item>Multiple values for beta of illustration point found.</item>
        /// </list>
        /// </exception>
        private void ParseGeneralBetaValue(HydraRingDatabaseReader reader)
        {
            Dictionary<string, object>[] betaValues = GetIterator(reader).ToArray();
            if (betaValues.Length > 1)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
            }

            if (betaValues.Length == 0)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_No_values_for_beta_of_illustration_point_found);
            }

            beta = ConvertToDouble(betaValues[0][IllustrationPointsDatabaseConstants.BetaValue],
                                   IllustrationPointsDatabaseConstants.BetaValue);
        }

        /// <summary>
        /// Converts <paramref name="doubleValue"/> to <see cref="double"/>.
        /// </summary>
        /// <param name="doubleValue">The object to convert.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The converted double.</returns>
        /// <exception cref="HydraRingFileParserException">Thrown when <paramref name="doubleValue"/> 
        /// is <see cref="DBNull"/>.</exception>
        /// <seealso cref="Convert.ToDouble(object)"/>
        private static double ConvertToDouble(object doubleValue, string identifier)
        {
            if (doubleValue.Equals(DBNull.Value))
            {
                throw new HydraRingFileParserException(string.Format(Resources.IllustrationPointsParser_Parse_Column_0_is_Null, identifier));
            }

            return Convert.ToDouble(doubleValue);
        }

        /// <summary>
        /// Parses <see cref="Stochast"/> objects from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the read <see cref="IllustrationPointsDatabaseConstants.Duration"/> 
        /// or <see cref="IllustrationPointsDatabaseConstants.AlphaValue"/> is <see cref="DBNull"/>.</exception>
        private void ParseGeneralAlphaValues(HydraRingDatabaseReader reader)
        {
            stochasts = GetIterator(reader).Select(a =>
            {
                string name = Convert.ToString(a[IllustrationPointsDatabaseConstants.StochastName]);
                double duration = ConvertToDouble(a[IllustrationPointsDatabaseConstants.Duration], IllustrationPointsDatabaseConstants.Duration);
                double alpha = ConvertToDouble(a[IllustrationPointsDatabaseConstants.AlphaValue], IllustrationPointsDatabaseConstants.AlphaValue);
                return new Stochast(name, duration, alpha);
            }).ToArray();
        }

        private void ParseClosingSituations(HydraRingDatabaseReader reader)
        {
            closingSituations = GetIterator(reader).ToDictionary(
                r => Convert.ToInt32(r[IllustrationPointsDatabaseConstants.ClosingSituationId]),
                r => Convert.ToString(r[IllustrationPointsDatabaseConstants.ClosingSituationName]));
        }

        /// <summary>
        /// Parses <see cref="WindDirection"/> objects from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the read <see cref="IllustrationPointsDatabaseConstants.WindDirectionAngle"/> 
        /// is <see cref="DBNull"/> or when no governing wind direction is found.</exception>
        private void ParseWindDirections(HydraRingDatabaseReader reader)
        {
            windDirections = new Dictionary<int, WindDirection>();

            foreach (Dictionary<string, object> readWindDirection in GetIterator(reader))
            {
                int key = Convert.ToInt32(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionId]);
                string name = Convert.ToString(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionName]);
                double angle = ConvertToDouble(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionAngle],
                                               IllustrationPointsDatabaseConstants.WindDirectionAngle);
                bool isGoverning = Convert.ToBoolean(readWindDirection[IllustrationPointsDatabaseConstants.IsGoverning]);

                var windDirection = new WindDirection(name, angle);
                windDirections[key] = windDirection;

                if (isGoverning)
                {
                    governingWindDirection = windDirection;
                }
            }

            if (governingWindDirection == null)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_No_governing_wind_direction_found);
            }
        }

        private void ParseSubMechanisms(HydraRingDatabaseReader reader)
        {
            subMechanisms = GetIterator(reader).ToDictionary(
                r => Convert.ToInt32(r[IllustrationPointsDatabaseConstants.SubMechanismId]),
                r => Convert.ToString(r[IllustrationPointsDatabaseConstants.SubMechanismName]));
        }

        private void ParseFaultTrees(HydraRingDatabaseReader reader)
        {
            faultTrees = GetIterator(reader).ToDictionary(
                r => Convert.ToInt32(r[IllustrationPointsDatabaseConstants.FaultTreeId]),
                r => Convert.ToString(r[IllustrationPointsDatabaseConstants.FaultTreeName]));
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

        private struct ThreeKeyIndex
        {
            private readonly int windDirectionId;
            private readonly int closingSituationId;
            private readonly int illustrationPointId;

            public ThreeKeyIndex(int windDirectionId, int closingSituationId, int illustrationPointId)
            {
                this.windDirectionId = windDirectionId;
                this.closingSituationId = closingSituationId;
                this.illustrationPointId = illustrationPointId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (GetType() != obj.GetType())
                {
                    return false;
                }

                return Equals((ThreeKeyIndex) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = windDirectionId;
                    hashCode = (hashCode * 397) ^ closingSituationId;
                    hashCode = (hashCode * 397) ^ illustrationPointId;
                    return hashCode;
                }
            }

            private bool Equals(ThreeKeyIndex other)
            {
                return windDirectionId == other.windDirectionId
                       && closingSituationId == other.closingSituationId
                       && illustrationPointId == other.illustrationPointId;
            }
        }
    }
}