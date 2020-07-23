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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for configuring calculations.
    /// </summary>
    public abstract partial class CalculationsView : UserControl, ISelectionProvider, IView
    {
        private CalculationGroup calculationGroup;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationsView"/>.
        /// </summary>
        /// <param name="calculationGroup">All the calculations of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CalculationsView(CalculationGroup calculationGroup)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            this.calculationGroup = calculationGroup;

            InitializeComponent();
        }

        public object Selection { get; }

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = value as CalculationGroup;
        }
    }
}