// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Forms.Properties;
using CommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Controls
{
    /// <summary>
    /// Control to display an assembly result with probability.
    /// </summary>
    public partial class AssessmentSectionAssemblyResultControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyResultControl"/>.
        /// </summary>
        public AssessmentSectionAssemblyResultControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the error message of the control.
        /// </summary>
        /// <param name="errorMessage">The error message to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessage"/>
        /// is <c>null</c>.</exception>
        public void SetError(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            SetErrorProviderMessage(errorMessage);
            SetErrorProviderIconPadding();
        }

        /// <summary>
        /// Clears the errors of the control.
        /// </summary>
        public void ClearErrors()
        {
            errorProvider.SetError(groupLabel, string.Empty);
            errorProvider.SetError(probabilityLabel, string.Empty);
        }

        /// <summary>
        /// Sets the value of <paramref name="result"/> on the control.
        /// </summary>
        /// <param name="result">The <see cref="AssessmentSectionAssemblyResult"/> to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="result"/>
        /// has an invalid value for <see cref="AssessmentSectionAssemblyGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="result"/>
        /// is not supported.</exception>
        public void SetAssemblyResult(AssessmentSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            groupLabel.Text = EnumDisplayNameHelper.GetDisplayName(result.AssemblyGroup);
            groupLabel.BackColor = AssessmentSectionAssemblyGroupColorHelper.GetAssessmentSectionAssemblyGroupColor(result.AssemblyGroup);

            probabilityLabel.Text = ProbabilityFormattingHelper.FormatWithDiscreteNumbers(result.Probability);
        }

        /// <summary>
        /// Clears the assembly result of the control.
        /// </summary>
        public void ClearAssemblyResult()
        {
            groupLabel.Text = Resources.AssessmentSectionAssemblyResultControl_AssemblyGroup_No_result_dash;
            groupLabel.BackColor = Color.White;
            probabilityLabel.Text = CommonFormsResources.RoundedDouble_No_result_dash;
        }

        private void SetErrorProviderMessage(string errorMessage)
        {
            errorProvider.SetError(groupLabel, errorMessage);
            errorProvider.SetError(probabilityLabel, errorMessage);
        }

        private void SetErrorProviderIconPadding()
        {
            errorProvider.SetIconPadding(groupLabel, 3);
            errorProvider.SetIconPadding(probabilityLabel, 3);
        }
    }
}