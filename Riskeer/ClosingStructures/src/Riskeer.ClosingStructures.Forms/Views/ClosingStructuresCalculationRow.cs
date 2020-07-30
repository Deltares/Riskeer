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
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;

namespace Riskeer.ClosingStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="ClosingStructuresCalculationsView"/>.
    /// </summary>
    public class ClosingStructuresCalculationRow : CalculationRow<StructuresCalculationScenario<ClosingStructuresInput>>, IHasColumnStateDefinitions
    {
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal ClosingStructuresCalculationRow(StructuresCalculationScenario<ClosingStructuresInput> calculationScenario,
                                                 IObservablePropertyChangeHandler handler)
            : base(calculationScenario, handler)
        {
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            UpdateUseBreakWaterColumnStateDefinitions();
            UpdateUseForeshoreColumnStateDefinitions();
        }

        /// <summary>
        /// Gets or sets the foreshore profile of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<ForeshoreProfile> ForeshoreProfile
        {
            get => new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(Calculation.InputParameters.ForeshoreProfile);
            set
            {
                ForeshoreProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.ForeshoreProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.ForeshoreProfile = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether break water of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get => Calculation.InputParameters.UseBreakWater;
            set
            {
                if (!Calculation.InputParameters.UseBreakWater.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.UseBreakWater = value, PropertyChangeHandler);
                    UpdateUseBreakWaterColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public BreakWaterType BreakWaterType
        {
            get => Calculation.InputParameters.BreakWater.Type;
            set
            {
                if (!Calculation.InputParameters.BreakWater.Type.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.BreakWater.Type = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water height of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public RoundedDouble BreakWaterHeight
        {
            get => Calculation.InputParameters.BreakWater.Height;
            set
            {
                if (!Calculation.InputParameters.BreakWater.Height.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.BreakWater.Height = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether foreshore profile of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/> should be used.
        /// </summary>
        public bool UseForeshoreGeometry
        {
            get => Calculation.InputParameters.UseForeshore;
            set
            {
                if (!Calculation.InputParameters.UseForeshore.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.UseForeshore = value, PropertyChangeHandler);
                    UpdateUseForeshoreColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public ClosingStructureInflowModelType InflowModelType
        {
            get => Calculation.InputParameters.InflowModelType;
            set
            {
                if (!Calculation.InputParameters.InflowModelType.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.InflowModelType = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mean inside water level of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public RoundedDouble MeanInsideWaterLevel
        {
            get => Calculation.InputParameters.InsideWaterLevel.Mean;
            set
            {
                if (!Calculation.InputParameters.InsideWaterLevel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.InsideWaterLevel.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the critial overtopping discharge of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public RoundedDouble CriticalOvertoppingDischarge
        {
            get => Calculation.InputParameters.CriticalOvertoppingDischarge.Mean;
            set
            {
                if (!Calculation.InputParameters.CriticalOvertoppingDischarge.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.CriticalOvertoppingDischarge.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the allowed level of storage increase of the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>.
        /// </summary>
        public RoundedDouble AllowedLevelIncreaseStorage
        {
            get => Calculation.InputParameters.AllowedLevelIncreaseStorage.Mean;
            set
            {
                if (!Calculation.InputParameters.AllowedLevelIncreaseStorage.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.AllowedLevelIncreaseStorage.Mean = value, PropertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        public override Point2D GetCalculationLocation()
        {
            return Calculation.InputParameters.Structure.Location;
        }

        protected override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get => Calculation.InputParameters.HydraulicBoundaryLocation;
            set => Calculation.InputParameters.HydraulicBoundaryLocation = value;
        }

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
            ForeshoreProfile foreShoreProfileGeometry = Calculation.InputParameters.ForeshoreProfile;
            if (foreShoreProfileGeometry == null || !foreShoreProfileGeometry.Geometry.Any())
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