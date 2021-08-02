// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using log4net;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability.TargetProbability"/>
    /// value of a <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> and clearing all dependent data.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler : IObservablePropertyChangeHandler
    {
        private readonly HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability;
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler));

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The calculations to change the target probability for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationsForTargetProbability"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            this.calculationsForTargetProbability = calculationsForTargetProbability;
        }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue)
        {
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            var affectedObjects = new List<IObservable>();

            if (ConfirmPropertyChange())
            {
                setValue();

                affectedObjects.AddRange(ClearHydraulicBoundaryLocationCalculationOutput());
            }

            return affectedObjects;
        }

        private static bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(Resources.HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler_Confirm_change_target_probability_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        private IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput()
        {
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculationsForTargetProbability);

            if (affectedObjects.Any())
            {
                log.Info(Resources.HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler_Hydraulic_load_results_cleared);

                return affectedObjects;
            }

            return Enumerable.Empty<IObservable>();
        }
    }
}