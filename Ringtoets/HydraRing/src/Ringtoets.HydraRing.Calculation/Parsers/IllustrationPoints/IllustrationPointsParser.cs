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
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// Parser for transforming values for illustration points from the database into a data structure.
    /// </summary>
    public class IllustrationPointsParser : IHydraRingFileParser
    {
        /// <summary>
        /// The result of parsing the illustration points in the Hydra-Ring database.
        /// </summary>
        public readonly GeneralResult Output = new GeneralResult();

        private readonly Dictionary<ThreeKeyIndex, IList<Stochast>> faultTreeStochasts = new Dictionary<ThreeKeyIndex, IList<Stochast>>();
        private readonly Dictionary<ThreeKeyIndex, double> faultTreeBetaValues = new Dictionary<ThreeKeyIndex, double>();

        private readonly Dictionary<ThreeKeyIndex, IList<RealizedStochast>> subMechanismStochasts = new Dictionary<ThreeKeyIndex, IList<RealizedStochast>>();
        private readonly Dictionary<ThreeKeyIndex, double> subMechanismBetaValues = new Dictionary<ThreeKeyIndex, double>();
        private readonly Dictionary<ThreeKeyIndex, IList<IllustrationPointResult>> subMechanismResults = new Dictionary<ThreeKeyIndex, IList<IllustrationPointResult>>();

        private IDictionary<int, WindDirection> windDirections;
        private IDictionary<int, string> closingSituations;
        private IDictionary<int, string> subMechanisms;
        private IDictionary<int, string> faultTrees;

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
            catch (Exception e) when (e is HydraRingDatabaseReaderException || e is SQLiteException)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Could_not_read_illustration_point_data, e);
            }
        }

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
            ParseFaultTree(reader);
            if (Output.IllustrationPoints == null)
            {
                SetSubMechanismAsRootIllustrationPoint();
            }
        }

        private static void ProceedOrThrow(HydraRingDatabaseReader reader)
        {
            if (!reader.NextResult())
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Could_not_read_illustration_point_data);
            }
        }

        private void SetSubMechanismAsRootIllustrationPoint()
        {
            var rootIllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
            foreach (Tuple<int, WindDirection, int, string> windDirectionClosingSituation in GetAllWindDirectionClosingSituationCombinations())
            {
                string submechanismIllustrationPointName = subMechanisms.First().Value;
                double subMechanismIllustrationPointBeta = subMechanismBetaValues.First().Value;
                var illustrationPoint = new SubMechanismIllustrationPoint(submechanismIllustrationPointName,
                                                                          subMechanismIllustrationPointBeta);

                AddRange(illustrationPoint.Results, subMechanismResults.First().Value);
                AddRange(illustrationPoint.Stochasts, subMechanismStochasts.First().Value);

                rootIllustrationPoints[CreateFaultTreeKey(windDirectionClosingSituation)] =
                    new IllustrationPointTreeNode(illustrationPoint);
            }
            Output.IllustrationPoints = rootIllustrationPoints;
        }

        private void ParseFaultTreeAlphaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readFaultTreeAlphaValue in GetIterator(reader))
            {
                int faultTreeId = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.FaultTreeId]);
                int windDirectionId = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string name = Convert.ToString(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.StochastName]);
                double duration = Convert.ToDouble(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.Duration]);
                double alpha = Convert.ToDouble(readFaultTreeAlphaValue[IllustrationPointsDatabaseConstants.AlphaValue]);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, faultTreeId);
                if (!faultTreeStochasts.ContainsKey(key))
                {
                    faultTreeStochasts[key] = new List<Stochast>();
                }

                faultTreeStochasts[key].Add(new Stochast
                {
                    Name = name,
                    Duration = duration,
                    Alpha = alpha
                });
            }
        }

        private void ParseFaultTreeBetaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readFaultTreeBetaValue in GetIterator(reader))
            {
                int faultTreeId = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.FaultTreeId]);
                int windDirectionId = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                double beta = Convert.ToDouble(readFaultTreeBetaValue[IllustrationPointsDatabaseConstants.BetaValue]);

                var threeKeyIndex = new ThreeKeyIndex(windDirectionId, closingSituationid, faultTreeId);
                if (faultTreeBetaValues.ContainsKey(threeKeyIndex))
                {
                    throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
                }
                faultTreeBetaValues[threeKeyIndex] = beta;
            }
        }

        private void ParseSubMechanismAlphaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismAlphaValue in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string name = Convert.ToString(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.StochastName]);
                double duration = Convert.ToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.Duration]);
                double alpha = Convert.ToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.AlphaValue]);
                double realization = Convert.ToDouble(readSubMechanismAlphaValue[IllustrationPointsDatabaseConstants.Realization]);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (!subMechanismStochasts.ContainsKey(key))
                {
                    subMechanismStochasts[key] = new List<RealizedStochast>();
                }

                subMechanismStochasts[key].Add(new RealizedStochast
                {
                    Name = name,
                    Duration = duration,
                    Alpha = alpha,
                    Realization = realization
                });
            }
        }

        private void ParseSubMechanismBetaValues(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismBetaValue in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                double beta = Convert.ToDouble(readSubMechanismBetaValue[IllustrationPointsDatabaseConstants.BetaValue]);

                var threeKeyIndex = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (subMechanismBetaValues.ContainsKey(threeKeyIndex))
                {
                    throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
                }
                subMechanismBetaValues[threeKeyIndex] = beta;
            }
        }

        private void ParseSubMechanismResults(HydraRingDatabaseReader reader)
        {
            foreach (Dictionary<string, object> readSubMechanismResult in GetIterator(reader))
            {
                int subMechanismId = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.SubMechanismId]);
                int windDirectionId = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.WindDirectionId]);
                int closingSituationid = Convert.ToInt32(readSubMechanismResult[IllustrationPointsDatabaseConstants.ClosingSituationId]);
                string description = Convert.ToString(readSubMechanismResult[IllustrationPointsDatabaseConstants.IllustrationPointResultDescription]);
                double value = Convert.ToDouble(readSubMechanismResult[IllustrationPointsDatabaseConstants.IllustrationPointResultValue]);

                var key = new ThreeKeyIndex(windDirectionId, closingSituationid, subMechanismId);
                if (!subMechanismResults.ContainsKey(key))
                {
                    subMechanismResults[key] = new List<IllustrationPointResult>();
                }

                subMechanismResults[key].Add(new IllustrationPointResult
                {
                    Description = description,
                    Value = value
                });
            }
        }

        private void ParseFaultTree(HydraRingDatabaseReader reader)
        {
            IEnumerable<Tuple<int, WindDirection, int, string>> windDirectionClosingSituations =
                GetAllWindDirectionClosingSituationCombinations();

            Dictionary<string, object>[] readFaultTrees = GetIterator(reader).ToArray();
            if (readFaultTrees.Length > 0)
            {
                List<Tuple<int?, int, Type, CombinationType>> results = CreateResultTuples(readFaultTrees);

                var rootIllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
                foreach (Tuple<int, WindDirection, int, string> windDirectionClosingSituation in windDirectionClosingSituations)
                {
                    Tuple<int?, int, Type, CombinationType> root = results.Single(r => !r.Item1.HasValue);

                    rootIllustrationPoints[CreateFaultTreeKey(windDirectionClosingSituation)] =
                        BuildFaultTree(windDirectionClosingSituation, root.Item2, root.Item4, results);
                }

                Output.IllustrationPoints = rootIllustrationPoints;
            }
        }

        private static List<Tuple<int?, int, Type, CombinationType>> CreateResultTuples(Dictionary<string, object>[] readFaultTrees)
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
                                                     closingSituation => Tuple.Create(windDirection.Key, windDirection.Value, closingSituation.Key, closingSituation.Value)));
        }

        private IllustrationPointTreeNode BuildFaultTree(
            Tuple<int, WindDirection, int, string> windDirectionClosingSituation,
            int faultTreeId,
            CombinationType combinationType,
            ICollection<Tuple<int?, int, Type, CombinationType>> results)
        {
            var dataKey = new ThreeKeyIndex(windDirectionClosingSituation.Item1, windDirectionClosingSituation.Item3, faultTreeId);
            var illustrationPoint = new FaultTreeIllustrationPoint(faultTrees[faultTreeId], faultTreeBetaValues[dataKey], combinationType);
            if (faultTreeStochasts.ContainsKey(dataKey))
            {
                AddRange(illustrationPoint.Stochasts, faultTreeStochasts[dataKey]);
            }

            var node = new IllustrationPointTreeNode(illustrationPoint);
            foreach (Tuple<int?, int, Type, CombinationType> child in results.Where(r => r.Item1 == faultTreeId))
            {
                node.Children.Add(child.Item3 == typeof(FaultTreeIllustrationPoint)
                                      ? BuildFaultTree(windDirectionClosingSituation, child.Item2, child.Item4, results)
                                      : BuildSubMechanism(windDirectionClosingSituation, child.Item2));
            }
            return node;
        }

        private IllustrationPointTreeNode BuildSubMechanism(Tuple<int, WindDirection, int, string> windDirectionClosingSituation, int subMechanismId)
        {
            var dataKey = new ThreeKeyIndex(windDirectionClosingSituation.Item1, windDirectionClosingSituation.Item3, subMechanismId);

            string submechanismIllustrationPointName = subMechanisms[subMechanismId];
            double subMechanismIllustrationPointBeta = subMechanismBetaValues[dataKey];
            var illustrationPoint = new SubMechanismIllustrationPoint(submechanismIllustrationPointName,
                                                                      subMechanismIllustrationPointBeta);

            if (subMechanismStochasts.ContainsKey(dataKey))
            {
                AddRange(illustrationPoint.Stochasts, subMechanismStochasts[dataKey]);
            }
            if (subMechanismResults.ContainsKey(dataKey))
            {
                AddRange(illustrationPoint.Results, subMechanismResults[dataKey]);
            }
            return new IllustrationPointTreeNode(illustrationPoint);
        }

        private static void AddRange<T>(ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            foreach (T item in itemsToAdd)
            {
                collection.Add(item);
            }
        }

        private void ParseGeneralBetaValue(HydraRingDatabaseReader reader)
        {
            Dictionary<string, object>[] betaValues = GetIterator(reader).ToArray();
            if (betaValues.Length != 1)
            {
                throw new HydraRingFileParserException(Resources.IllustrationPointsParser_Parse_Multiple_values_for_beta_of_illustration_point_found);
            }
            Output.Beta = Convert.ToDouble(betaValues[0][IllustrationPointsDatabaseConstants.BetaValue]);
        }

        private void ParseGeneralAlphaValues(HydraRingDatabaseReader reader)
        {
            Output.Stochasts = GetIterator(reader).Select(a => new Stochast
            {
                Name = Convert.ToString(a[IllustrationPointsDatabaseConstants.StochastName]),
                Duration = Convert.ToDouble(a[IllustrationPointsDatabaseConstants.Duration]),
                Alpha = Convert.ToDouble(a[IllustrationPointsDatabaseConstants.AlphaValue])
            }).ToArray();
        }

        private void ParseClosingSituations(HydraRingDatabaseReader reader)
        {
            closingSituations = GetIterator(reader).ToDictionary(
                r => Convert.ToInt32(r[IllustrationPointsDatabaseConstants.ClosingSituationId]),
                r => Convert.ToString(r[IllustrationPointsDatabaseConstants.ClosingSituationName]));
        }

        private void ParseWindDirections(HydraRingDatabaseReader reader)
        {
            windDirections = new Dictionary<int, WindDirection>();

            foreach (Dictionary<string, object> readWindDirection in GetIterator(reader))
            {
                int key = Convert.ToInt32(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionId]);
                string name = Convert.ToString(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionName]);
                double angle = Convert.ToDouble(readWindDirection[IllustrationPointsDatabaseConstants.WindDirectionAngle]);
                bool isGoverning = Convert.ToBoolean(readWindDirection[IllustrationPointsDatabaseConstants.IsGoverning]);

                var windDirection = new WindDirection(name, angle);
                windDirections[key] = windDirection;

                if (isGoverning)
                {
                    Output.GoverningWind = windDirection;
                }
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
                return obj.GetType() == GetType()
                       && Equals((ThreeKeyIndex) obj);
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