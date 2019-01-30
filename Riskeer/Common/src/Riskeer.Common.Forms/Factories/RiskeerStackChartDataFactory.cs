// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Core.Common.Util.Extensions;
using Core.Components.Stack.Data;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="StackChartData"/> based on information used as input.
    /// </summary>
    public static class RiskeerStackChartDataFactory
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
        /// <param name="illustrationPointControlItems">The data to create the columns from.</param>
        /// <param name="stackChartData">The stack chart data to create the columns for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void CreateColumns(IEnumerable<IllustrationPointControlItem> illustrationPointControlItems,
                                         StackChartData stackChartData)
        {
            if (illustrationPointControlItems == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointControlItems));
            }

            if (stackChartData == null)
            {
                throw new ArgumentNullException(nameof(stackChartData));
            }

            IEnumerable<Tuple<string, string>> labels =
                illustrationPointControlItems.Select(controlItem => Tuple.Create(controlItem.WindDirectionName,
                                                                                 controlItem.ClosingSituation)).ToArray();

            bool showClosingSituation = illustrationPointControlItems.HasMultipleUniqueValues(item => item.ClosingSituation);

            foreach (Tuple<string, string> label in labels)
            {
                string columnName = label.Item1;

                if (showClosingSituation)
                {
                    columnName = string.Format(Resources.RiskeerStackChartDataFactory_CreateColumns_WindDirection_0_ClosingSituation_1,
                                               columnName,
                                               label.Item2);
                }

                stackChartData.AddColumn(columnName);
            }
        }

        /// <summary>
        /// Creates the rows for the given <paramref name="stackChartData"/>.
        /// </summary>
        /// <param name="illustrationPointControlItems">The data to create the rows from.</param>
        /// <param name="stackChartData">The stack chart data to create the rows for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void CreateRows(IEnumerable<IllustrationPointControlItem> illustrationPointControlItems, StackChartData stackChartData)
        {
            if (illustrationPointControlItems == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointControlItems));
            }

            if (stackChartData == null)
            {
                throw new ArgumentNullException(nameof(stackChartData));
            }

            var stochastValues = new List<Tuple<string, RoundedDouble>>();

            foreach (IllustrationPointControlItem illustrationPointControlItem in illustrationPointControlItems)
            {
                stochastValues.AddRange(GetStochastValues(illustrationPointControlItem));
            }

            IDictionary<string, List<RoundedDouble>> stochasts = CreateStochastsLookup(stochastValues);

            CreateRowsForStochasts(stackChartData, stochasts);
        }

        private static IEnumerable<Tuple<string, RoundedDouble>> GetStochastValues(IllustrationPointControlItem illustrationPointControlItem)
        {
            return illustrationPointControlItem.Stochasts
                                               .Select(stochast => Tuple.Create(stochast.Name,
                                                                                new RoundedDouble(5, Math.Pow(stochast.Alpha, 2))))
                                               .ToArray();
        }

        private static IDictionary<string, List<RoundedDouble>> CreateStochastsLookup(IEnumerable<Tuple<string, RoundedDouble>> stochastValues)
        {
            var lookup = new Dictionary<string, List<RoundedDouble>>();

            foreach (Tuple<string, RoundedDouble> stochastValue in stochastValues)
            {
                if (!lookup.ContainsKey(stochastValue.Item1))
                {
                    lookup.Add(stochastValue.Item1, new List<RoundedDouble>());
                }

                lookup[stochastValue.Item1].Add(stochastValue.Item2);
            }

            return lookup;
        }

        private static void CreateRowsForStochasts(StackChartData stackChartData, IDictionary<string, List<RoundedDouble>> stochasts)
        {
            IDictionary<string, List<RoundedDouble>> significantStochasts = new Dictionary<string, List<RoundedDouble>>();
            IDictionary<string, List<RoundedDouble>> remainingStochasts = new Dictionary<string, List<RoundedDouble>>();

            foreach (KeyValuePair<string, List<RoundedDouble>> stochast in stochasts)
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

            foreach (KeyValuePair<string, List<RoundedDouble>> significantStochast in significantStochasts)
            {
                stackChartData.AddRow(significantStochast.Key, significantStochast.Value.Select(v => v.Value).ToArray());
            }

            if (remainingStochasts.Any())
            {
                stackChartData.AddRow(Resources.RiskeerStackChartDataFactory_RemainingRow_DisplayName,
                                      GetValuesForRemainingRow(remainingStochasts),
                                      Color.Gray);
            }
        }

        private static bool StochastIsSignificant(KeyValuePair<string, List<RoundedDouble>> stochast)
        {
            return stochast.Value.Any(v => v > minAlphaSquared);
        }

        private static double[] GetValuesForRemainingRow(IDictionary<string, List<RoundedDouble>> stochasts)
        {
            var values = new double[stochasts.First().Value.Count];
            var index = 0;

            foreach (KeyValuePair<string, List<RoundedDouble>> stochast in stochasts)
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