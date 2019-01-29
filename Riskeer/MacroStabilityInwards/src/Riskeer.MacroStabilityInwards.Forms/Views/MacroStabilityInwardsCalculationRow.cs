// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsCalculationScenario"/> in the <see cref="MacroStabilityInwardsCalculationsView"/>.
    /// </summary>
    internal class MacroStabilityInwardsCalculationRow
    {
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="macroStabilityInwardsCalculation">The <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculationRow(MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation,
                                                   IObservablePropertyChangeHandler handler)
        {
            if (macroStabilityInwardsCalculation == null)
            {
                throw new ArgumentNullException(nameof(macroStabilityInwardsCalculation));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            MacroStabilityInwardsCalculation = macroStabilityInwardsCalculation;
            propertyChangeHandler = handler;
        }

        /// <summary>
        /// Gets the <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.
        /// </summary>
        public MacroStabilityInwardsCalculationScenario MacroStabilityInwardsCalculation { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return MacroStabilityInwardsCalculation.Name;
            }
            set
            {
                MacroStabilityInwardsCalculation.Name = value;

                MacroStabilityInwardsCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel> StochasticSoilModel
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(MacroStabilityInwardsCalculation.InputParameters.StochasticSoilModel);
            }
            set
            {
                MacroStabilityInwardsStochasticSoilModel valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(MacroStabilityInwardsCalculation.InputParameters.StochasticSoilModel, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => MacroStabilityInwardsCalculation.InputParameters.StochasticSoilModel = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile> StochasticSoilProfile
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(MacroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile);
            }
            set
            {
                MacroStabilityInwardsStochasticSoilProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(MacroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => MacroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StochasticSoilProfileProbability
        {
            get
            {
                return MacroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile != null
                           ? new RoundedDouble(2, MacroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile.Probability * 100)
                           : new RoundedDouble(2);
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (MacroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(MacroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation,
                                                            MacroStabilityInwardsCalculation.InputParameters.SurfaceLine?.ReferenceLineIntersectionWorldPoint));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(MacroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => MacroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation = valueToSet, propertyChangeHandler);
                }
            }
        }
    }
}