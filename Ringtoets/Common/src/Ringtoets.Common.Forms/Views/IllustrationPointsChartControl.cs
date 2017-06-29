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
using System.Windows.Forms;
using Core.Components.Stack.Data;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Control to show illustration points in a chart view.
    /// </summary>
    public partial class IllustrationPointsChartControl : UserControl
    {
        private GeneralResult data;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsChartControl"/>.
        /// </summary>
        public IllustrationPointsChartControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public GeneralResult Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;

                if (data != null)
                {
                    CreateColumns();
                }
            }
        }

        private void CreateColumns()
        {
            var chartData = new StackChartData();

            var stochastValues = new List<TempIllustrationPoint>();

            foreach (WindDirectionClosingSituationIllustrationPoint illustrationPoint in data.WindDirectionClosingSituationIllustrationPoints)
            {
                chartData.AddColumn($"{illustrationPoint.WindDirection.Name}");

                stochastValues.AddRange(illustrationPoint.IllustrationPoint.Stochasts
                                                         .Select(illustrationPointStochast =>
                                                                     new TempIllustrationPoint
                                                                     {
                                                                         Name = illustrationPointStochast.Name,
                                                                         AlphaSquared = Math.Pow(illustrationPointStochast.Alpha, 2)
                                                                     }));
            }

            IDictionary<string, List<double>> lookup = new Dictionary<string, List<double>>();

            foreach (TempIllustrationPoint stochastValue in stochastValues)
            {
                if (!lookup.ContainsKey(stochastValue.Name))
                {
                    lookup.Add(stochastValue.Name, new List<double>());
                }

                lookup[stochastValue.Name].Add(stochastValue.AlphaSquared);
            }

            IDictionary<string, List<double>> plotDirectly= new Dictionary<string, List<double>>();

            foreach (KeyValuePair<string, List<double>> lookupValue in lookup)
            {
                if(!lookupValue.Value.Any(v => v < 0.01))
                {
                    plotDirectly.Add(lookupValue);
                }
            }

            foreach (KeyValuePair<string, List<double>> pair in plotDirectly)
            {
                chartData.AddRow(pair.Key, pair.Value.ToArray());
            }

            Dictionary<string, List<double>> otherLookup = lookup.Except(plotDirectly).ToDictionary(l => l.Key, l => l.Value);

            if (otherLookup.Any())
            {
                var values = new double[chartData.Columns.Count()];
                int index = 0;

                foreach (KeyValuePair<string, List<double>> keyValuePair in otherLookup)
                { 
                    foreach (double value in keyValuePair.Value)
                    {
                        values[index] += value;
                        index++;
                    }

                    index = 0;
                }

                chartData.AddRow("Overig", values);
            }

            stackChartControl.Data = chartData;
        }

        private class TempIllustrationPoint
        {
            public string Name { get; set; }

            public double AlphaSquared { get; set; }
        }
    }
}