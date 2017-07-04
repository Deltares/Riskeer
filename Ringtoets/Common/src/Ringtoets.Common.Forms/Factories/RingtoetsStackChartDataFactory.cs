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
using System.Drawing;
using System.Linq;
using Core.Components.Stack.Data;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="StackChartData"/> based on information used as input.
    /// </summary>
    public static class RingtoetsStackChartDataFactory
    {
        private const double minAlphaSquared = 0.01;

        /// <summary>
        /// Creates a new <see cref="StackChartData"/>.
        /// </summary>
        /// <returns>The created <see cref="StackChartData"/>.</returns>
        public static StackChartData Create()
        {
            return new StackChartData();
        }

        /// <summary>
        /// Creates the columns for the given <paramref name="stackChartData"/>.
        /// </summary>
        /// <param name="generalResultSubMechanismIllustrationPoint">The data to create the columns from.</param>
        /// <param name="stackChartData">The stack chart data to create the columns for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void CreateColumns(GeneralResultSubMechanismIllustrationPoint generalResultSubMechanismIllustrationPoint, StackChartData stackChartData)
        {
            if (generalResultSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(generalResultSubMechanismIllustrationPoint));
            }
            if (stackChartData == null)
            {
                throw new ArgumentNullException(nameof(stackChartData));
            }

            Tuple<string, string>[] labels = generalResultSubMechanismIllustrationPoint.TopLevelSubMechanismIllustrationPoints
                                                          .Select(illustrationPoint =>
                                                                      new Tuple<string, string>(illustrationPoint.WindDirection.Name,
                                                                                                illustrationPoint.ClosingSituation))
                                                          .ToArray();

            bool showClosingSituation = labels.Any(l => l.Item2 != labels[0].Item2);

            foreach (Tuple<string, string> label in labels)
            {
                string columnName = label.Item1;

                if (showClosingSituation)
                {
                    columnName = string.Format(Resources.RingtoetsStackChartDataFactory_CreateColumns_WindDirection_0_ClosingSituation_1,
                                               columnName,
                                               label.Item2);
                }

                stackChartData.AddColumn(columnName);
            }
        }

        /// <summary>
        /// Creates the rows for the given <paramref name="stackChartData"/>.
        /// </summary>
        /// <param name="generalResultSubMechanismIllustrationPoint">The data to create the rows from.</param>
        /// <param name="stackChartData">The stack chart data to create the rows for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void CreateRows(GeneralResultSubMechanismIllustrationPoint generalResultSubMechanismIllustrationPoint, StackChartData stackChartData)
        {
            if (generalResultSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(generalResultSubMechanismIllustrationPoint));
            }
            if (stackChartData == null)
            {
                throw new ArgumentNullException(nameof(stackChartData));
            }

            var stochastValues = new List<Tuple<string, double>>();

            foreach (TopLevelSubMechanismIllustrationPoint illustrationPoint in generalResultSubMechanismIllustrationPoint.TopLevelSubMechanismIllustrationPoints)
            {
                stochastValues.AddRange(illustrationPoint.SubMechanismIllustrationPoint.Stochasts
                                                         .Select(illustrationPointStochast =>
                                                                     new Tuple<string, double>(illustrationPointStochast.Name,
                                                                                               Math.Pow(illustrationPointStochast.Alpha, 2))));
            }

            IDictionary<string, List<double>> stochasts = CreateStochastsLookup(stochastValues);

            CreateRowsForStochasts(stackChartData, stochasts);
        }

        private static IDictionary<string, List<double>> CreateStochastsLookup(IEnumerable<Tuple<string, double>> stochastValues)
        {
            var lookup = new Dictionary<string, List<double>>();

            foreach (Tuple<string, double> stochastValue in stochastValues)
            {
                if (!lookup.ContainsKey(stochastValue.Item1))
                {
                    lookup.Add(stochastValue.Item1, new List<double>());
                }

                lookup[stochastValue.Item1].Add(stochastValue.Item2);
            }
            return lookup;
        }

        private static void CreateRowsForStochasts(StackChartData stackChartData, IDictionary<string, List<double>> stochasts)
        {
            IDictionary<string, List<double>> significantStochasts = new Dictionary<string, List<double>>();
            IDictionary<string, List<double>> remainingStochasts = new Dictionary<string, List<double>>();

            foreach (KeyValuePair<string, List<double>> stochast in stochasts)
            {
                if (StochastIsSignificant(stochast))
                {
                    significantStochasts.Add(stochast);
                }
                else
                {
                    remainingStochasts.Add(stochast);
                }
            }

            foreach (KeyValuePair<string, List<double>> significantStochast in significantStochasts)
            {
                stackChartData.AddRow(significantStochast.Key, significantStochast.Value.ToArray());
            }

            if (remainingStochasts.Any())
            {
                stackChartData.AddRow(Resources.RingtoetsStackChartDataFactory_RemainingRow_DisplayName,
                                      GetValuesForRemainingRow(remainingStochasts),
                                      Color.Gray);
            }
        }

        private static bool StochastIsSignificant(KeyValuePair<string, List<double>> stochast)
        {
            return stochast.Value.Any(v => v > minAlphaSquared);
        }

        private static double[] GetValuesForRemainingRow(IDictionary<string, List<double>> stochasts)
        {
            var values = new double[stochasts.First().Value.Count];
            var index = 0;

            foreach (KeyValuePair<string, List<double>> stochast in stochasts)
            {
                foreach (double value in stochast.Value)
                {
                    values[index] += value;
                    index++;
                }

                index = 0;
            }

            return values;
        }
    }
}