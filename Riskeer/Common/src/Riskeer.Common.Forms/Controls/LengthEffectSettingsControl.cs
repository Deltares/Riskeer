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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Exceptions;
using Core.Common.Base.Helpers;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;

namespace Riskeer.Common.Forms.Controls
{
    /// <summary>
    /// Control to display length effect properties.
    /// </summary>
    public partial class LengthEffectSettingsControl : UserControl
    {
        private const int lengthEffectNNrOfDecimals = 2;
        private readonly double b;

        private readonly Observer scenarioConfigurationObserver;
        private ScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection;

        private bool isParameterAUpdating;

        /// <summary>
        /// Creates a new instance of <see cref="LengthEffectSettingsControl"/>
        /// </summary>
        public LengthEffectSettingsControl(double b)
        {
            this.b = b;

            InitializeComponent();
            scenarioConfigurationObserver = new Observer(UpdateLengthEffectData);
        }

        public void SetScenarioConfiguration(ScenarioConfigurationPerFailureMechanismSection scenarioConfiguration)
        {
            if (scenarioConfiguration == null)
            {
                throw new ArgumentNullException(nameof(scenarioConfiguration));
            }

            ClearLengthEffectErrorMessage();

            scenarioConfigurationPerFailureMechanismSection = scenarioConfiguration;
            scenarioConfigurationObserver.Observable = scenarioConfiguration;

            UpdateLengthEffectData();

            EnableControl();
        }

        /// <summary>
        /// Clears the data on the control.
        /// </summary>
        public void ClearData()
        {
            isParameterAUpdating = true;
            scenarioConfigurationPerFailureMechanismSection = null;

            scenarioConfigurationObserver.Observable = scenarioConfigurationPerFailureMechanismSection;
            ClearLengthEffectErrorMessage();
            ClearLengthEffectData();

            DisableControl();

            isParameterAUpdating = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                scenarioConfigurationObserver.Dispose();
            }

            base.Dispose(disposing);
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
                ClearLengthEffectErrorMessage();
                UpdateLengthEffectData();
                e.Handled = true;
            }
        }

        private void ParameterATextBoxLeave(object sender, EventArgs e)
        {
            ClearLengthEffectErrorMessage();
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

                UpdateLengthEffectData();
            }
            catch (Exception exception) when (exception is ArgumentOutOfRangeException
                                              || exception is DoubleParsingException)
            {
                ClearNRoundedData();
                SetLengthEffectErrorMessage(exception.Message);
                parameterATextBox.Focus();
            }
        }

        private void ClearLengthEffectData()
        {
            parameterATextBox.Text = string.Empty;
            ClearNRoundedData();
        }

        private void ClearNRoundedData()
        {
            lengthEffectNRoundedTextBox.Text = string.Empty;
        }

        private void UpdateLengthEffectData()
        {
            parameterATextBox.Text = scenarioConfigurationPerFailureMechanismSection.A.ToString();

            double n = scenarioConfigurationPerFailureMechanismSection.GetN(b);
            lengthEffectNRoundedTextBox.Text = new RoundedDouble(lengthEffectNNrOfDecimals, n).ToString();
        }

        private void SetLengthEffectErrorMessage(string errorMessage)
        {
            lengthEffectErrorProvider.SetIconPadding(parameterATextBox, 5);
            lengthEffectErrorProvider.SetError(parameterATextBox, errorMessage);
        }

        private void ClearLengthEffectErrorMessage()
        {
            lengthEffectErrorProvider.SetError(parameterATextBox, string.Empty);
        }

        private void EnableControl()
        {
            parameterATextBox.Enabled = true;
            parameterATextBox.Refresh();

            lengthEffectNRoundedTextBox.Enabled = true;
            lengthEffectNRoundedTextBox.Refresh();
        }

        private void DisableControl()
        {
            parameterATextBox.Enabled = false;
            parameterATextBox.Refresh();

            lengthEffectNRoundedTextBox.Enabled = false;
            lengthEffectNRoundedTextBox.Refresh();
        }
    }
}