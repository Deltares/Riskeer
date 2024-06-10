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
    /// Control to display properties of the <see cref="FailureMechanismSectionConfiguration"/>.
    /// </summary>
    public partial class FailureMechanismSectionConfigurationControl : UserControl
    {
        private const int lengthEffectNRoundedNrOfDecimals = 2;

        private readonly double b;
        private FailureMechanismSectionConfiguration failureMechanismSectionConfiguration;

        private readonly Observer sectionConfigurationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionConfigurationControl"/>.
        /// </summary>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        public FailureMechanismSectionConfigurationControl(double b)
        {
            this.b = b;
            
            InitializeComponent();
            InitializeToolTips();

            sectionConfigurationObserver = new Observer(UpdateFailureMechanismSectionConfigurationData);
        }

        /// <summary>
        /// Sets the data on the control.
        /// </summary>
        /// <param name="sectionConfiguration">The configuration to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/>
        /// is <c>null</c>.</exception>
        public void SetData(FailureMechanismSectionConfiguration sectionConfiguration)
        {
            if (sectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sectionConfiguration));
            }
            
            parameterBTextBox.Text = b.ToString(CultureInfo.CurrentCulture);
            failureMechanismSectionConfiguration = sectionConfiguration;
            sectionConfigurationObserver.Observable = sectionConfiguration;
            
            UpdateFailureMechanismSectionConfigurationData();
        }

        /// <summary>
        /// Clears the data on the control.
        /// </summary>
        public void ClearData()
        {
            failureMechanismSectionConfiguration = null;
            sectionConfigurationObserver.Observable = null;
            
            ClearControls();
        }

        private void InitializeToolTips()
        {
            parameterAToolTip.SetToolTip(parameterALabel, Resources.Parameter_A_Description);
            parameterBToolTip.SetToolTip(parameterBLabel, Resources.FailureMechanism_GeneralInput_B_Description);
            lengthEffectNRoundedToolTip.SetToolTip(lengthEffectNRoundedLabel, Resources.LengthEffectNRounded_Description);
        }
        
        private void ClearControls()
        {
            parameterATextBox.Text = string.Empty;
            parameterBTextBox.Text = string.Empty;
            lengthEffectNRoundedTextBox.Text = string.Empty;
        }

        private void UpdateFailureMechanismSectionConfigurationData()
        {
            parameterATextBox.Text = failureMechanismSectionConfiguration.A.ToString();

            double n = failureMechanismSectionConfiguration.GetN(b);
            lengthEffectNRoundedTextBox.Text = new RoundedDouble(lengthEffectNRoundedNrOfDecimals, n).ToString();
        }
    }
}