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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsCalculationScenario"/> in the <see cref="MacroStabilityInwardsCalculationsView"/>.
    /// </summary>
    public class MacroStabilityInwardsCalculationRow : CalculationRow<MacroStabilityInwardsCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal MacroStabilityInwardsCalculationRow(MacroStabilityInwardsCalculationScenario calculationScenario,
                                                   IObservablePropertyChangeHandler handler)
            : base(calculationScenario, handler) {}

        /// <summary>
        /// Gets or sets the stochastic soil model of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel> StochasticSoilModel
        {
            get => new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(Calculation.InputParameters.StochasticSoilModel);
            set
            {
                MacroStabilityInwardsStochasticSoilModel valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.StochasticSoilModel, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StochasticSoilModel = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile> StochasticSoilProfile
        {
            get => new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(Calculation.InputParameters.StochasticSoilProfile);
            set
            {
                MacroStabilityInwardsStochasticSoilProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.StochasticSoilProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StochasticSoilProfile = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StochasticSoilProfileProbability =>
            Calculation.InputParameters.StochasticSoilProfile != null
                ? new RoundedDouble(2, Calculation.InputParameters.StochasticSoilProfile.Probability * 100)
                : new RoundedDouble(2);

        public override Point2D GetCalculationLocation()
        {
            return Calculation.InputParameters.SurfaceLine?.ReferenceLineIntersectionWorldPoint;
        }

        protected override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get => Calculation.InputParameters.HydraulicBoundaryLocation;
            set => Calculation.InputParameters.HydraulicBoundaryLocation = value;
        }
    }
}