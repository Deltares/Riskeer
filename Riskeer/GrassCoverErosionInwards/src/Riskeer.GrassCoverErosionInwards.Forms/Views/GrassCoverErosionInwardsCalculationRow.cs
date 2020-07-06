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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="GrassCoverErosionInwardsCalculationsView"/>.
    /// </summary>
    internal class GrassCoverErosionInwardsCalculationRow : IHasColumnStateDefinitions
    {
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
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
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            UpdateColumnStateDefinitions();
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
                    new SelectableHydraulicBoundaryLocation(GrassCoverErosionInwardsCalculationScenario.InputParameters.HydraulicBoundaryLocation, null));
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

        /// <summary>
        /// Gets or sets the dike profile of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<DikeProfile> DikeProfile
        {
            get
            {
                return new DataGridViewComboBoxItemWrapper<DikeProfile>(GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeProfile);
            }
            set
            {
                DikeProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeProfile = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether break water of the <see cref="GrassCoverErosionInwardsCalculationScenario"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.UseBreakWater;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.UseBreakWater.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.UseBreakWater = value, propertyChangeHandler);
                    UpdateColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public BreakWaterType BreakWaterType
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Type;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Type.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Type = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water height of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble BreakWaterHeight
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Height;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Height.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.BreakWater.Height = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether foreshore profile of the <see cref="GrassCoverErosionInwardsCalculationScenario"/> should be used.
        /// </summary>
        public bool UseForeShoreGeometry
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.UseForeshore;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.UseForeshore.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.UseForeshore = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dike height of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble DikeHeight
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeHeight;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeHeight.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.DikeHeight = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mean critical flow rate of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble MeanCriticalFlowRate
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.Mean;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard deviation critical flow rate of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StandardDeviationCriticalFlowRate
        {
            get
            {
                return GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation;
            }
            set
            {
                if (!GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => GrassCoverErosionInwardsCalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation = value, propertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(breakWaterTypeColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(breakWaterHeightColumnIndex, new DataGridViewColumnStateDefinition());
        }

        private void UpdateColumnStateDefinitions()
        {
            if (!UseBreakWater)
            {
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[breakWaterTypeColumnIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[breakWaterHeightColumnIndex]);
            }
            else
            {
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[breakWaterTypeColumnIndex]);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[breakWaterHeightColumnIndex]);
            }
        }
    }
}