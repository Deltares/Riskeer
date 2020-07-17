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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="StabilityPointStructuresCalculationsView"/>.
    /// </summary>
    internal class StabilityPointStructuresCalculationRow : IHasColumnStateDefinitions
    {
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;
        private const int constructiveStrengthLinearLoadModelColumnIndex = 8;
        private const int constructiveStrengthQuadraticLoadModelColumnIndex = 9;
        private const int stabilityLinearLoadModelColumnIndex = 10;
        private const int stabilityQuadraticLoadModelColumnIndex = 11;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="CalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationRow(StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario,
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
            UpdateLoadSchematizationColumnStateDefinitions();
        }

        /// <summary>
        /// Gets the <see cref="CalculationScenario"/> this row contains.
        /// </summary>
        public StructuresCalculationScenario<StabilityPointStructuresInput> CalculationScenario { get; }

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
        /// Gets or sets the foreshore profile of the <see cref="CalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<ForeshoreProfile> ForeshoreProfile
        {
            get => new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(CalculationScenario.InputParameters.ForeshoreProfile);
            set
            {
                ForeshoreProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(CalculationScenario.InputParameters.ForeshoreProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.ForeshoreProfile = valueToSet, propertyChangeHandler);
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
                if (!CalculationScenario.InputParameters.UseBreakWater.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.UseBreakWater = value, propertyChangeHandler);
                    UpdateUseBreakWaterColumnStateDefinitions();
                }
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
                if (!CalculationScenario.InputParameters.BreakWater.Type.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.BreakWater.Type = value, propertyChangeHandler);
                }
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
        public bool UseForeshoreGeometry
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
        /// Gets or sets the load schematization type of the <see cref="CalculationScenario"/>.
        /// </summary>
        public LoadSchematizationType LoadSchematizationType
        {
            get => CalculationScenario.InputParameters.LoadSchematizationType;
            set
            {
                if (!CalculationScenario.InputParameters.LoadSchematizationType.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.LoadSchematizationType = value, propertyChangeHandler);
                    UpdateLoadSchematizationColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength linear load model of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble ConstructiveStrengthLinearLoadModel
        {
            get => CalculationScenario.InputParameters.ConstructiveStrengthLinearLoadModel.Mean;
            set
            {
                if (!CalculationScenario.InputParameters.ConstructiveStrengthLinearLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.ConstructiveStrengthLinearLoadModel.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength quadratic load model of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble ConstructiveStrengthQuadraticLoadModel
        {
            get => CalculationScenario.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean;
            set
            {
                if (!CalculationScenario.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stability of the linear load model of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble StabilityLinearLoadModel
        {
            get => CalculationScenario.InputParameters.StabilityLinearLoadModel.Mean;
            set
            {
                if (!CalculationScenario.InputParameters.StabilityLinearLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.StabilityLinearLoadModel.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stability of the quadratic load model of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble StabilityQuadraticLoadModel
        {
            get => CalculationScenario.InputParameters.StabilityQuadraticLoadModel.Mean;
            set
            {
                if (!CalculationScenario.InputParameters.StabilityQuadraticLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.StabilityQuadraticLoadModel.Mean = value, propertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the evaluation level of the <see cref="CalculationScenario"/>.
        /// </summary>
        public RoundedDouble EvaluationLevel
        {
            get => CalculationScenario.InputParameters.EvaluationLevel;
            set
            {
                if (!CalculationScenario.InputParameters.EvaluationLevel.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => CalculationScenario.InputParameters.EvaluationLevel = value, propertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(breakWaterTypeColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(breakWaterHeightColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(useForeshoreColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(constructiveStrengthLinearLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(constructiveStrengthQuadraticLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityLinearLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityQuadraticLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
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
            ForeshoreProfile foreShoreProfileGeometry = CalculationScenario.InputParameters.ForeshoreProfile;
            if (foreShoreProfileGeometry == null || !foreShoreProfileGeometry.Geometry.Any())
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[useForeshoreColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useForeshoreColumnIndex]);
            }
        }

        private void UpdateLoadSchematizationColumnStateDefinitions()
        {
            if (LoadSchematizationType == LoadSchematizationType.Linear)
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[constructiveStrengthLinearLoadModelColumnIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[stabilityLinearLoadModelColumnIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[constructiveStrengthQuadraticLoadModelColumnIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[stabilityQuadraticLoadModelColumnIndex]);
            }
            else
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[constructiveStrengthLinearLoadModelColumnIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[stabilityLinearLoadModelColumnIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[constructiveStrengthQuadraticLoadModelColumnIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[stabilityQuadraticLoadModelColumnIndex]);
            }
        }
    }
}