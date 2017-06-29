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
using System.Linq;
using Core.Components.Stack.Data;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="StackChartData"/> based on information used as input.
    /// </summary>
    public static class RingtoetsStackChartDataFactory
    {
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
        /// <param name="generalResult">The data to create the columns from.</param>
        /// <param name="stackChartData">The stack chart data to create the columns for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void CreateColumns(GeneralResult generalResult, StackChartData stackChartData)
        {
            if (generalResult == null)
            {
                throw new ArgumentNullException(nameof(generalResult));
            }
            if (stackChartData == null)
            {
                throw new ArgumentNullException(nameof(stackChartData));
            }

            Tuple<string, string>[] labels = generalResult.WindDirectionClosingSituationIllustrationPoints
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
                    columnName = $"{columnName} ({label.Item2})";
                }

                stackChartData.AddColumn(columnName);
            }
        }
    }
}