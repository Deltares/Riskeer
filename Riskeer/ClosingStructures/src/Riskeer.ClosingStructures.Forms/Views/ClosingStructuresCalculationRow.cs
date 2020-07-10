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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.ClosingStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="ClosingStructuresCalculationsView"/>.
    /// </summary>
    internal class ClosingStructuresCalculationRow : IHasColumnStateDefinitions
    {
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="ClosingStructuresCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresCalculationRow(StructuresCalculationScenario<ClosingStructuresInput> calculationScenario,
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

            ClosingStructuresCalculationScenario = calculationScenario;
            propertyChangeHandler = handler;
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            UpdateUseBreakWaterColumnStateDefinitions();
        }

        /// <summary>
        /// Gets the <see cref="ClosingStructuresCalculationScenario"/> this row contains.
        /// </summary>
        public StructuresCalculationScenario<ClosingStructuresInput> ClosingStructuresCalculationScenario { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get => ClosingStructuresCalculationScenario.Name;
            set
            {
                ClosingStructuresCalculationScenario.Name = value;

                ClosingStructuresCalculationScenario.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (ClosingStructuresCalculationScenario.InputParameters.HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(ClosingStructuresCalculationScenario.InputParameters.HydraulicBoundaryLocation, null));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(ClosingStructuresCalculationScenario.InputParameters.HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.HydraulicBoundaryLocation = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreshore profile of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<ForeshoreProfile> ForeshoreProfile
        {
            get => new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(ClosingStructuresCalculationScenario.InputParameters.ForeshoreProfile);
            set
            {
                ForeshoreProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(ClosingStructuresCalculationScenario.InputParameters.ForeshoreProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.ForeshoreProfile = valueToSet, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether break water of the <see cref="ClosingStructuresCalculationScenario"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get => ClosingStructuresCalculationScenario.InputParameters.UseBreakWater;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.UseBreakWater = value, propertyChangeHandler);
                UpdateUseBreakWaterColumnStateDefinitions();
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public BreakWaterType BreakWaterType
        {
            get => ClosingStructuresCalculationScenario.InputParameters.BreakWater.Type;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.BreakWater.Type = value, propertyChangeHandler);
            }
        }

        /// <summary>
        /// Gets or sets the break water height of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public RoundedDouble BreakWaterHeight
        {
            get => ClosingStructuresCalculationScenario.InputParameters.BreakWater.Height;
            set
            {
                if (!ClosingStructuresCalculationScenario.InputParameters.BreakWater.Height.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.BreakWater.Height = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether foreshore profile of the <see cref="ClosingStructuresCalculationScenario"/> should be used.
        /// </summary>
        public bool UseForeShoreGeometry
        {
            get => ClosingStructuresCalculationScenario.InputParameters.UseForeshore;
            set
            {
                if (!ClosingStructuresCalculationScenario.InputParameters.UseForeshore.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.UseForeshore = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public ClosingStructureInflowModelType InflowModelType
        {
            get => ClosingStructuresCalculationScenario.InputParameters.InflowModelType;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.InflowModelType = value, propertyChangeHandler);
            }
        }

        /// <summary>
        /// Gets or sets the mean inside water level of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public RoundedDouble MeanInsideWaterLevel
        {
            get => ClosingStructuresCalculationScenario.InputParameters.InsideWaterLevel.Mean;
            set
            {
                if (!ClosingStructuresCalculationScenario.InputParameters.InsideWaterLevel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.InsideWaterLevel.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the critial overtopping discharge of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public RoundedDouble CriticalOvertoppingDischarge
        {
            get => ClosingStructuresCalculationScenario.InputParameters.CriticalOvertoppingDischarge.Mean;
            set
            {
                if (!ClosingStructuresCalculationScenario.InputParameters.CriticalOvertoppingDischarge.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.CriticalOvertoppingDischarge.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the allowed level of storage increase of the <see cref="ClosingStructuresCalculationScenario"/>.
        /// </summary>
        public RoundedDouble AllowedLevelIncreaseStorage
        {
            get => ClosingStructuresCalculationScenario.InputParameters.AllowedLevelIncreaseStorage.Mean;
            set
            {
                if (!ClosingStructuresCalculationScenario.InputParameters.AllowedLevelIncreaseStorage.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => ClosingStructuresCalculationScenario.InputParameters.AllowedLevelIncreaseStorage.Mean = value, propertyChangeHandler);
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
    }
}