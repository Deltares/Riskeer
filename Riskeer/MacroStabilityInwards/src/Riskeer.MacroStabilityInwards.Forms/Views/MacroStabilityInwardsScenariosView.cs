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
using Core.Common.Controls.Views;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// View for configuring macrostability inwards calculation scenarios.
    /// </summary>
    public partial class MacroStabilityInwardsScenariosView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// to get the sections from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            InitializeComponent();
        }

        public object Data { get; set; }
    }
}