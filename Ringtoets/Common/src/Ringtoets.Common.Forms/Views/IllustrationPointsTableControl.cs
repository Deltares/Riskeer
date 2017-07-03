﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Control to show illustration points in a table view.
    /// </summary>
    public partial class IllustrationPointsTableControl : UserControl
    {
        private const int closingSituationColumnIndex = 1;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsTableControl"/>.
        /// </summary>
        public IllustrationPointsTableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public GeneralResult Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            illustrationPointsDataGridViewControl.AddTextBoxColumn("WindDirection",
                                                                   Resources.IllustrationPoint_WindDirection_DisplayName,
                                                                   true);
            illustrationPointsDataGridViewControl.AddTextBoxColumn("ClosingScenario",
                                                                   Resources.IllustrationPoint_ClosingSituation_DisplayName,
                                                                   true);
            illustrationPointsDataGridViewControl.AddTextBoxColumn("CalculatedProbability",
                                                                   Resources.IllustrationPoint_CalculatedProbability_DisplayName,
                                                                   true);
            illustrationPointsDataGridViewControl.AddTextBoxColumn("CalculatedReliability",
                                                                   Resources.IllustrationPoint_CalculatedReliability_DisplayName,
                                                                   true);

            illustrationPointsDataGridViewControl.SetColumnVisibility(closingSituationColumnIndex, false);
        }
    }
}