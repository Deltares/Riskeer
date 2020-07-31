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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingCalculationsView"/>.
    /// </summary>
    public class PipingCalculationRow : CalculationRow<PipingCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal PipingCalculationRow(PipingCalculationScenario calculationScenario,
                                      IObservablePropertyChangeHandler handler)
            : base(calculationScenario, handler) {}

        /// <summary>
        /// Gets or sets the stochastic soil model of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel> StochasticSoilModel
        {
            get => new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(Calculation.InputParameters.StochasticSoilModel);
            set
            {
                PipingStochasticSoilModel valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.StochasticSoilModel, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StochasticSoilModel = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile> StochasticSoilProfile
        {
            get => new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(Calculation.InputParameters.StochasticSoilProfile);
            set
            {
                PipingStochasticSoilProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.StochasticSoilProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StochasticSoilProfile = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StochasticSoilProfileProbability =>
            Calculation.InputParameters.StochasticSoilProfile != null
                ? new RoundedDouble(2, Calculation.InputParameters.StochasticSoilProfile.Probability * 100)
                : new RoundedDouble(2);

        /// <summary>
        /// Gets or sets the damping factory exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble DampingFactorExitMean
        {
            get => Calculation.InputParameters.DampingFactorExit.Mean;
            set
            {
                if (!Calculation.InputParameters.DampingFactorExit.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.DampingFactorExit.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the phreatic level exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble PhreaticLevelExitMean
        {
            get => Calculation.InputParameters.PhreaticLevelExit.Mean;
            set
            {
                if (!Calculation.InputParameters.PhreaticLevelExit.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.PhreaticLevelExit.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the entry point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble EntryPointL
        {
            get => Calculation.InputParameters.EntryPointL;
            set
            {
                if (!Calculation.InputParameters.EntryPointL.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.EntryPointL = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the exit point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble ExitPointL
        {
            get => Calculation.InputParameters.ExitPointL;
            set
            {
                if (!Calculation.InputParameters.ExitPointL.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.ExitPointL = value, PropertyChangeHandler);
                }
            }
        }

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