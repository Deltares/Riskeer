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
using Core.Common.Base.Data;
using Core.Common.Base.Exceptions;
using Core.Common.Base.Helpers;
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
        private const int lengthEffectNNrOfDecimals = 2;

        private readonly double b;
        private ScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection;

        private bool isParameterAUpdating;

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
            
            ClearParameterAErrorMessage();

            scenarioConfigurationPerFailureMechanismSection = scenarioConfiguration;

            UpdateScenarioConfigurationPerSectionData();

            EnableControl();
        }

        /// <summary>
        /// Clears the data on the control.
        /// </summary>
        public void ClearData()
        {
            isParameterAUpdating = true;
            scenarioConfigurationPerFailureMechanismSection = null;

            ClearParameterAErrorMessage();
            ClearScenarioConfigurationPerSectionData();

            DisableControl();

            isParameterAUpdating = false;
        }

        private void InitializeToolTips()
        {
            parameterAToolTip.SetToolTip(parameterALabel, Resources.Parameter_A_Description);
            parameterBToolTip.SetToolTip(parameterBLabel, Resources.FailureMechanism_GeneralInput_B_Description);
            lengthEffectNRoundedToolTip.SetToolTip(lengthEffectNRoundedLabel, Resources.LengthEffect_RoundedNSection_Description);
        }

        private void ParameterATextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                parameterALabel.Focus(); // Focus on different component to raise a leave event on the text box
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                ClearParameterAErrorMessage();
                UpdateScenarioConfigurationPerSectionData();
                e.Handled = true;
            }
        }

        private void ParameterATextBoxLeave(object sender, EventArgs e)
        {
            ClearParameterAErrorMessage();
            ProcessParameterATextBox();
        }

        private void ProcessParameterATextBox()
        {
            if (isParameterAUpdating)
            {
                return;
            }

            try
            {
                scenarioConfigurationPerFailureMechanismSection.A = (RoundedDouble) DoubleParsingHelper.Parse(parameterATextBox.Text);
                scenarioConfigurationPerFailureMechanismSection.NotifyObservers();

                UpdateScenarioConfigurationPerSectionData();
            }
            catch (Exception exception) when (exception is ArgumentOutOfRangeException
                                              || exception is DoubleParsingException)
            {
                ClearNRoundedData();
                SetParameterAErrorMessage(exception.Message);
                parameterATextBox.Focus();
            }
        }

        private void ClearScenarioConfigurationPerSectionData()
        {
            parameterATextBox.Text = string.Empty;
            parameterBTextBox.Text = string.Empty;
            ClearNRoundedData();
        }

        private void ClearNRoundedData()
        {
            lengthEffectNRoundedTextBox.Text = string.Empty;
        }

        private void UpdateScenarioConfigurationPerSectionData()
        {
            parameterATextBox.Text = scenarioConfigurationPerFailureMechanismSection.A.ToString();

            double n = scenarioConfigurationPerFailureMechanismSection.GetN(b);
            lengthEffectNRoundedTextBox.Text = new RoundedDouble(lengthEffectNNrOfDecimals, n).ToString();
        }

        private void SetParameterAErrorMessage(string errorMessage)
        {
            errorProvider.SetIconPadding(parameterATextBox, 5);
            errorProvider.SetError(parameterATextBox, errorMessage);
        }

        private void ClearParameterAErrorMessage()
        {
            errorProvider.SetError(parameterATextBox, string.Empty);
        }

        private void EnableControl()
        {
            parameterATextBox.Enabled = true;
            parameterATextBox.Refresh();
        }

        private void DisableControl()
        {
            parameterATextBox.Enabled = false;
            parameterATextBox.Refresh();
        }
    }
}