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
using System.Linq;
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
        private const int useForeshoreColumnIndex = 6;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="CalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationRow(GrassCoverErosionInwardsCalculationScenario calculationScenario,
                                                      IObservablePropertyChangeHandler handler)
        {
            if (calculationScenario == null)
            {
                throw new ArgumentNullException(nameof(calculationScenario));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            CalculationScenario = calculationScenario;
            propertyChangeHandler = handler;
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            UpdateUseBreakWaterColumnStateDefinitions();
            UpdateUseForeshoreColumnStateDefinitions();
        }

        /// <summary>
        /// Gets the <see cref="CalculationScenario"/> this row contains.
        /// </summary>
        public GrassCoverErosionInwardsCalculationScenario CalculationScenario { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="CalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get => CalculationScenario.Name;
            set
            {
                CalculationScenario.Name = value;

                CalculationScenario.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="CalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (CalculationScenario.InputParameters.HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(CalculationScenario.InputParameters.HydraulicBoundaryLocation, null));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(CalculationScenario.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.HydraulicBoundaryLocation = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dike profile of the <see cref="CalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<DikeProfile> DikeProfile
        {
            get => new DataGridViewComboBoxItemWrapper<DikeProfile>(CalculationScenario.InputParameters.DikeProfile);
            set
            {
                DikeProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(CalculationScenario.InputParameters.DikeProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.DikeProfile = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether break water of the <see cref="CalculationScenario"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get => CalculationScenario.InputParameters.UseBreakWater;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.UseBreakWater = value, propertyChangeHandler);
                UpdateUseBreakWaterColumnStateDefinitions();
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="CalculationScenario"/>.
        /// </summary>
        public BreakWaterType BreakWaterType
        {
            get => CalculationScenario.InputParameters.BreakWater.Type;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.BreakWater.Type = value, propertyChangeHandler);
            }
        }

        /// <summary>
        /// Gets or sets the break water height of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble BreakWaterHeight
        {
            get => CalculationScenario.InputParameters.BreakWater.Height;
            set
            {
                if (!CalculationScenario.InputParameters.BreakWater.Height.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.BreakWater.Height = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether foreshore profile of the <see cref="CalculationScenario"/> should be used.
        /// </summary>
        public bool UseForeShoreGeometry
        {
            get => CalculationScenario.InputParameters.UseForeshore;
            set
            {
                if (!CalculationScenario.InputParameters.UseForeshore.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.UseForeshore = value, propertyChangeHandler);
                    UpdateUseForeshoreColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dike height of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble DikeHeight
        {
            get => CalculationScenario.InputParameters.DikeHeight;
            set
            {
                if (!CalculationScenario.InputParameters.DikeHeight.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.DikeHeight = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mean critical flow rate of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble MeanCriticalFlowRate
        {
            get => CalculationScenario.InputParameters.CriticalFlowRate.Mean;
            set
            {
                if (!CalculationScenario.InputParameters.CriticalFlowRate.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.CriticalFlowRate.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard deviation critical flow rate of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble StandardDeviationCriticalFlowRate
        {
            get => CalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation;
            set
            {
                if (!CalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.CriticalFlowRate.StandardDeviation = value, propertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(breakWaterTypeColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(breakWaterHeightColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(useForeshoreColumnIndex, new DataGridViewColumnStateDefinition());
        }

        private void UpdateUseBreakWaterColumnStateDefinitions()
        {
            if (!UseBreakWater)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[breakWaterTypeColumnIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[breakWaterHeightColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[breakWaterTypeColumnIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[breakWaterHeightColumnIndex]);
            }
        }

        private void UpdateUseForeshoreColumnStateDefinitions()
        {
            DikeProfile dikeProfileForeshoreGeometry = CalculationScenario.InputParameters.DikeProfile;
            if (dikeProfileForeshoreGeometry == null || !dikeProfileForeshoreGeometry.ForeshoreGeometry.Any())
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[useForeshoreColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useForeshoreColumnIndex]);
            }
        }
    }
}