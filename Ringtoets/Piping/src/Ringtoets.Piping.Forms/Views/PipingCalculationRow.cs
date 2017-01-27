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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingCalculationsView"/>.
    /// </summary>
    internal class PipingCalculationRow
    {
        private readonly ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="pipingCalculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingCalculationRow(PipingCalculationScenario pipingCalculation,
            ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> handler)
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
        public DataGridViewComboBoxItemWrapper<StochasticSoilModel> StochasticSoilModel
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(PipingCalculation.InputParameters.StochasticSoilModel);
            }
            set
            {
                StochasticSoilModel valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(PipingCalculation.InputParameters.StochasticSoilModel, valueToSet))
                {
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.StochasticSoilModel = v, valueToSet);
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
                StochasticSoilProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(PipingCalculation.InputParameters.StochasticSoilProfile, valueToSet))
                {
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.StochasticSoilProfile = v, valueToSet);
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
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(PipingCalculation.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.HydraulicBoundaryLocation = v, valueToSet);
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.DampingFactorExit.Mean = v, value);
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.PhreaticLevelExit.Mean = v, value);
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.EntryPointL = v, value);
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.ExitPointL = v, value);
                }
            }
        }

        private void ChangePropertyValueAndNotifyAffectedObjects<TValue>(
           SetCalculationInputPropertyValueDelegate<PipingInput, TValue> setPropertyValue,
           TValue value)
        {
            IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                PipingCalculation.InputParameters,
                PipingCalculation,
                value,
                setPropertyValue);

            NotifyAffectedObjects(affectedObjects);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (var affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}