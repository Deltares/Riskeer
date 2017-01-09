// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingCalculationsView"/>.
    /// </summary>
    internal class PipingCalculationRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="pipingCalculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingCalculation"/> is <c>null</c>.</exception>
        public PipingCalculationRow(PipingCalculationScenario pipingCalculation)
        {
            if (pipingCalculation == null)
            {
                throw new ArgumentNullException(nameof(pipingCalculation));
            }

            PipingCalculation = pipingCalculation;
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
        public DataGridViewComboBoxItemWrapper<StochasticSoilModel> StochasticSoilModel
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(PipingCalculation.InputParameters.StochasticSoilModel);
            }
            set
            {
                var valueToSet = value?.WrappedObject;
                if (PipingCalculation.InputParameters.StochasticSoilModel != valueToSet)
                {
                    PipingCalculation.InputParameters.StochasticSoilModel = valueToSet;
                    ClearOutputAndNotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<StochasticSoilProfile> StochasticSoilProfile
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(PipingCalculation.InputParameters.StochasticSoilProfile);
            }
            set
            {
                var valueToSet = value?.WrappedObject;
                if (PipingCalculation.InputParameters.StochasticSoilProfile != valueToSet)
                {
                    PipingCalculation.InputParameters.StochasticSoilProfile = valueToSet;
                    ClearOutputAndNotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string StochasticSoilProfileProbability
        {
            get
            {
                return PipingCalculation.InputParameters.StochasticSoilProfile != null
                           ? new RoundedDouble(3, PipingCalculation.InputParameters.StochasticSoilProfile.Probability*100).Value.ToString(CultureInfo.CurrentCulture)
                           : new RoundedDouble(3).Value.ToString(CultureInfo.CurrentCulture);
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
                var valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (PipingCalculation.InputParameters.HydraulicBoundaryLocation != valueToSet)
                {
                    PipingCalculation.InputParameters.HydraulicBoundaryLocation = valueToSet;
                    ClearOutputAndNotifyPropertyChanged();
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
                    PipingCalculation.InputParameters.DampingFactorExit.Mean = value;
                    ClearOutputAndNotifyPropertyChanged();
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
                    PipingCalculation.InputParameters.PhreaticLevelExit.Mean = value;
                    ClearOutputAndNotifyPropertyChanged();
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
                    PipingCalculation.InputParameters.EntryPointL = value;
                    ClearOutputAndNotifyPropertyChanged();
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
                    PipingCalculation.InputParameters.ExitPointL = value;
                    ClearOutputAndNotifyPropertyChanged();
                }
            }
        }

        private void ClearOutputAndNotifyPropertyChanged()
        {
            IEnumerable<IObservable> affectedCalculation = RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(PipingCalculation);
            foreach (var calculation in affectedCalculation)
            {
                calculation.NotifyObservers();
            }
            PipingCalculation.InputParameters.NotifyObservers();
        }
    }
}