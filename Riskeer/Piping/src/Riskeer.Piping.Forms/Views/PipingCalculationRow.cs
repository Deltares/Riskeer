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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingCalculationsView"/>.
    /// </summary>
    internal class PipingCalculationRow
    {
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="pipingCalculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingCalculationRow(PipingCalculationScenario pipingCalculation,
                                    IObservablePropertyChangeHandler handler)
        {
            if (pipingCalculation == null)
            {
                throw new ArgumentNullException(nameof(pipingCalculation));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            PipingCalculation = pipingCalculation;
            propertyChangeHandler = handler;
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationScenario"/> this row contains.
        /// </summary>
        public PipingCalculationScenario PipingCalculation { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return PipingCalculation.Name;
            }
            set
            {
                PipingCalculation.Name = value;

                PipingCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel> StochasticSoilModel
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(PipingCalculation.InputParameters.StochasticSoilModel);
            }
            set
            {
                PipingStochasticSoilModel valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(PipingCalculation.InputParameters.StochasticSoilModel, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.StochasticSoilModel = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile> StochasticSoilProfile
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(PipingCalculation.InputParameters.StochasticSoilProfile);
            }
            set
            {
                PipingStochasticSoilProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(PipingCalculation.InputParameters.StochasticSoilProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.StochasticSoilProfile = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StochasticSoilProfileProbability
        {
            get
            {
                return PipingCalculation.InputParameters.StochasticSoilProfile != null
                           ? new RoundedDouble(2, PipingCalculation.InputParameters.StochasticSoilProfile.Probability * 100)
                           : new RoundedDouble(2);
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (PipingCalculation.InputParameters.HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(PipingCalculation.InputParameters.HydraulicBoundaryLocation,
                                                            PipingCalculation.InputParameters.SurfaceLine?.ReferenceLineIntersectionWorldPoint));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(PipingCalculation.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.HydraulicBoundaryLocation = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the damping factory exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble DampingFactorExitMean
        {
            get
            {
                return PipingCalculation.InputParameters.DampingFactorExit.Mean;
            }
            set
            {
                if (!PipingCalculation.InputParameters.DampingFactorExit.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.DampingFactorExit.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the phreatic level exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble PhreaticLevelExitMean
        {
            get
            {
                return PipingCalculation.InputParameters.PhreaticLevelExit.Mean;
            }
            set
            {
                if (!PipingCalculation.InputParameters.PhreaticLevelExit.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.PhreaticLevelExit.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the entry point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble EntryPointL
        {
            get
            {
                return PipingCalculation.InputParameters.EntryPointL;
            }
            set
            {
                if (!PipingCalculation.InputParameters.EntryPointL.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.EntryPointL = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the exit point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble ExitPointL
        {
            get
            {
                return PipingCalculation.InputParameters.ExitPointL;
            }
            set
            {
                if (!PipingCalculation.InputParameters.ExitPointL.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => PipingCalculation.InputParameters.ExitPointL = value, propertyChangeHandler);
                }
            }
        }
    }
}