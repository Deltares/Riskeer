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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="StabilityPointStructuresCalculationsView"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationRow : CalculationRow<StructuresCalculationScenario<StabilityPointStructuresInput>>, IHasColumnStateDefinitions
    {
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;
        private const int constructiveStrengthLinearLoadModelColumnIndex = 8;
        private const int constructiveStrengthQuadraticLoadModelColumnIndex = 9;
        private const int stabilityLinearLoadModelColumnIndex = 10;
        private const int stabilityQuadraticLoadModelColumnIndex = 11;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal StabilityPointStructuresCalculationRow(StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario,
                                                        IObservablePropertyChangeHandler handler)
            : base(calculationScenario, handler)
        {
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            UpdateUseBreakWaterColumnStateDefinitions();
            UpdateBreakWaterTypeAndHeightColumnStateDefinitions();
            UpdateUseForeshoreColumnStateDefinitions();
            UpdateLoadSchematizationColumnStateDefinitions();
        }

        /// <summary>
        /// Gets or sets the foreshore profile of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
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
                    UpdateUseForeshoreColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether break water of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get => Calculation.InputParameters.UseBreakWater;
            set
            {
                if (!Calculation.InputParameters.UseBreakWater.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.UseBreakWater = value, PropertyChangeHandler);
                    UpdateBreakWaterTypeAndHeightColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
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
        /// Gets or sets the break water height of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
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
        /// Gets or sets whether foreshore profile of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/> should be used.
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
        /// Gets or sets the load schematization type of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public LoadSchematizationType LoadSchematizationType
        {
            get => Calculation.InputParameters.LoadSchematizationType;
            set
            {
                if (!Calculation.InputParameters.LoadSchematizationType.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.LoadSchematizationType = value, PropertyChangeHandler);
                    UpdateLoadSchematizationColumnStateDefinitions();
                }
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength linear load model of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public RoundedDouble ConstructiveStrengthLinearLoadModel
        {
            get => Calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean;
            set
            {
                if (!Calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the constructive strength quadratic load model of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public RoundedDouble ConstructiveStrengthQuadraticLoadModel
        {
            get => Calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean;
            set
            {
                if (!Calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stability of the linear load model of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public RoundedDouble StabilityLinearLoadModel
        {
            get => Calculation.InputParameters.StabilityLinearLoadModel.Mean;
            set
            {
                if (!Calculation.InputParameters.StabilityLinearLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StabilityLinearLoadModel.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stability of the quadratic load model of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public RoundedDouble StabilityQuadraticLoadModel
        {
            get => Calculation.InputParameters.StabilityQuadraticLoadModel.Mean;
            set
            {
                if (!Calculation.InputParameters.StabilityQuadraticLoadModel.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.StabilityQuadraticLoadModel.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the evaluation level of the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>.
        /// </summary>
        public RoundedDouble EvaluationLevel
        {
            get => Calculation.InputParameters.EvaluationLevel;
            set
            {
                if (!Calculation.InputParameters.EvaluationLevel.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.EvaluationLevel = value, PropertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        public override Point2D GetCalculationLocation()
        {
            return Calculation.InputParameters.Structure?.Location;
        }

        protected override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get => Calculation.InputParameters.HydraulicBoundaryLocation;
            set => Calculation.InputParameters.HydraulicBoundaryLocation = value;
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(useBreakWaterColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(breakWaterTypeColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(breakWaterHeightColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(useForeshoreColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(constructiveStrengthLinearLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(constructiveStrengthQuadraticLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityLinearLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityQuadraticLoadModelColumnIndex, new DataGridViewColumnStateDefinition());
        }

        private void UpdateBreakWaterTypeAndHeightColumnStateDefinitions()
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
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useForeshoreColumnIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
            }
        }

        private void UpdateUseBreakWaterColumnStateDefinitions()
        {
            ForeshoreProfile foreShoreProfileGeometry = Calculation.InputParameters.ForeshoreProfile;
            if (foreShoreProfileGeometry == null)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
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