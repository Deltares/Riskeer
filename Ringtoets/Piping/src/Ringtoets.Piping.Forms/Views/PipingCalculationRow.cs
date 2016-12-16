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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingCalculationsView"/>.
    /// </summary>
    internal class PipingCalculationRow
    {
        private readonly PipingCalculationScenario pipingCalculation;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="pipingCalculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingCalculation"/> is <c>null</c>.</exception>
        public PipingCalculationRow(PipingCalculationScenario pipingCalculation)
        {
            if (pipingCalculation == null)
            {
                throw new ArgumentNullException("pipingCalculation");
            }

            this.pipingCalculation = pipingCalculation;
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationScenario"/> this row contains.
        /// </summary>
        public PipingCalculationScenario PipingCalculation
        {
            get
            {
                return pipingCalculation;
            }
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return pipingCalculation.Name;
            }
            set
            {
                pipingCalculation.Name = value;

                pipingCalculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<StochasticSoilModel> StochasticSoilModel
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(pipingCalculation.InputParameters.StochasticSoilModel);
            }
            set
            {
                pipingCalculation.InputParameters.StochasticSoilModel = value != null
                                                                            ? value.WrappedObject
                                                                            : null;
                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil profile of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<StochasticSoilProfile> StochasticSoilProfile
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(pipingCalculation.InputParameters.StochasticSoilProfile);
            }
            set
            {
                pipingCalculation.InputParameters.StochasticSoilProfile = value != null
                                                                              ? value.WrappedObject
                                                                              : null;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the stochastic soil profile probability of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string StochasticSoilProfileProbability
        {
            get
            {
                return pipingCalculation.InputParameters.StochasticSoilProfile != null
                           ? new RoundedDouble(3, pipingCalculation.InputParameters.StochasticSoilProfile.Probability*100).Value.ToString(CultureInfo.CurrentCulture)
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
                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(pipingCalculation.InputParameters.HydraulicBoundaryLocation,
                                                            pipingCalculation.InputParameters.SurfaceLine != null
                                                                ? pipingCalculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint
                                                                : null));
            }
            set
            {
                pipingCalculation.InputParameters.HydraulicBoundaryLocation = value == null || value.WrappedObject == null
                                                                                  ? null
                                                                                  : value.WrappedObject.HydraulicBoundaryLocation;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the damping factory exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble DampingFactorExitMean
        {
            get
            {
                return pipingCalculation.InputParameters.DampingFactorExit.Mean;
            }
            set
            {
                pipingCalculation.InputParameters.DampingFactorExit.Mean = value;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the phreatic level exit mean of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble PhreaticLevelExitMean
        {
            get
            {
                return pipingCalculation.InputParameters.PhreaticLevelExit.Mean;
            }
            set
            {
                pipingCalculation.InputParameters.PhreaticLevelExit.Mean = value;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the entry point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble EntryPointL
        {
            get
            {
                return pipingCalculation.InputParameters.EntryPointL;
            }
            set
            {
                pipingCalculation.InputParameters.EntryPointL = value;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the exit point l of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble ExitPointL
        {
            get
            {
                return pipingCalculation.InputParameters.ExitPointL;
            }
            set
            {
                pipingCalculation.InputParameters.ExitPointL = value;

                pipingCalculation.InputParameters.NotifyObservers();
            }
        }
    }
}