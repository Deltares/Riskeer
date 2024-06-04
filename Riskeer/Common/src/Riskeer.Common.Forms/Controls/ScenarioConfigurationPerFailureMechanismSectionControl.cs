// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Globalization;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Controls
{
    /// <summary>
    /// Control to display properties of the <see cref="ScenarioConfigurationPerFailureMechanismSection"/>.
    /// </summary>
    public partial class ScenarioConfigurationPerFailureMechanismSectionControl : UserControl
    {
        private const int roundedNSectionNrOfDecimals = 2;

        private readonly double b;
        private ScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection;

        private readonly Observer scenarioConfigurationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="ScenarioConfigurationPerFailureMechanismSectionControl"/>.
        /// </summary>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        public ScenarioConfigurationPerFailureMechanismSectionControl(double b)
        {
            this.b = b;
            
            InitializeComponent();
            InitializeToolTips();

            scenarioConfigurationObserver = new Observer(UpdateScenarioConfigurationPerSectionData);
        }

        /// <summary>
        /// Sets the data on the control.
        /// </summary>
        /// <param name="scenarioConfiguration">The scenario configuration to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="scenarioConfiguration"/>
        /// is <c>null</c>.</exception>
        public void SetData(ScenarioConfigurationPerFailureMechanismSection scenarioConfiguration)
        {
            if (scenarioConfiguration == null)
            {
                throw new ArgumentNullException(nameof(scenarioConfiguration));
            }
            
            parameterBTextBox.Text = b.ToString(CultureInfo.CurrentCulture);
            scenarioConfigurationPerFailureMechanismSection = scenarioConfiguration;
            scenarioConfigurationObserver.Observable = scenarioConfiguration;
            
            UpdateScenarioConfigurationPerSectionData();
        }

        /// <summary>
        /// Clears the data on the control.
        /// </summary>
        public void ClearData()
        {
            scenarioConfigurationPerFailureMechanismSection = null;
            scenarioConfigurationObserver.Observable = null;
            
            ClearControls();
        }

        private void InitializeToolTips()
        {
            parameterAToolTip.SetToolTip(parameterALabel, Resources.Parameter_A_Description);
            parameterBToolTip.SetToolTip(parameterBLabel, Resources.FailureMechanism_GeneralInput_B_Description);
            roundedNSectionToolTip.SetToolTip(roundedNSectionLabel, Resources.RoundedNSection_Description);
        }
        
        private void ClearControls()
        {
            parameterATextBox.Text = string.Empty;
            parameterBTextBox.Text = string.Empty;
            roundedNSectionTextBox.Text = string.Empty;
        }

        private void UpdateScenarioConfigurationPerSectionData()
        {
            parameterATextBox.Text = scenarioConfigurationPerFailureMechanismSection.A.ToString();

            double n = scenarioConfigurationPerFailureMechanismSection.GetN(b);
            roundedNSectionTextBox.Text = new RoundedDouble(roundedNSectionNrOfDecimals, n).ToString();
        }
    }
}