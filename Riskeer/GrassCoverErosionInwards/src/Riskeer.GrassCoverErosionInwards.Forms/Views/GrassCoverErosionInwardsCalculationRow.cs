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
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.IO.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="GrassCoverErosionInwardsCalculationsView"/>.
    /// </summary>
    internal class GrassCoverErosionInwardsCalculationRow
    {
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="grassCoverErosionInwardsCalculationScenario">The <see cref="GrassCoverErosionInwardsCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationRow(GrassCoverErosionInwardsCalculationScenario grassCoverErosionInwardsCalculationScenario,
                                                      IObservablePropertyChangeHandler handler)
        {
            if (grassCoverErosionInwardsCalculationScenario == null)
            {
                throw new ArgumentNullException(nameof(grassCoverErosionInwardsCalculationScenario));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            GrassCoverErosionInwardsCalculationScenario = grassCoverErosionInwardsCalculationScenario;
            propertyChangeHandler = handler;
        }

        /// <summary>
        /// Gets the <see cref="GrassCoverErosionInwardsCalculationScenario"/> this row contains.
        /// </summary>
        public GrassCoverErosionInwardsCalculationScenario GrassCoverErosionInwardsCalculationScenario { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.Name;
            }
            set
            {
                GrassCoverErosionInwardsCalculationScenario.Name = value;

                GrassCoverErosionInwardsCalculationScenario.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (GrassCoverErosionInwardsCalculationScenario.InputParameters.HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(GrassCoverErosionInwardsCalculationScenario.InputParameters.HydraulicBoundaryLocation,null));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(GrassCoverErosionInwardsCalculationScenario.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.HydraulicBoundaryLocation = valueToSet, propertyChangeHandler);
                }
            }
        }

        public DataGridViewComboBoxItemWrapper<DikeProfile> DikeProfile { get; set; }

        public bool UseBreakWater { get; set; }

        public DataGridViewComboBoxItemWrapper<BreakWaterType> BreakWaterType { get; set; }

        public double BreakWaterHeight { get; set; }

        public bool UseForeShoreGeometry { get; set; }

        public double DikeHeight { get; set; }

        public double ExpectedCriticalOvertoppingRate { get; set; }

        public double StandardDeviationCriticalOvertoppingRate { get; set; }
    }
}