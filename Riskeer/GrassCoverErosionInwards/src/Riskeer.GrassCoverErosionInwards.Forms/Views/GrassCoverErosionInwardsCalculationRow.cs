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
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row in the <see cref="GrassCoverErosionInwardsCalculationsView"/>.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationRow : CalculationRow<GrassCoverErosionInwardsCalculationScenario>, IHasColumnStateDefinitions
    {
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;

        /// <summary>
        /// Fired when <see cref="DikeProfile"/> has changed.
        /// </summary>
        public event EventHandler DikeProfileChanged;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="GrassCoverErosionInwardsCalculationScenario"/> this row contains.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal GrassCoverErosionInwardsCalculationRow(GrassCoverErosionInwardsCalculationScenario calculationScenario,
                                                        IObservablePropertyChangeHandler handler) :
            base(calculationScenario, handler)
        {
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>
            {
                {
                    useBreakWaterColumnIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    breakWaterTypeColumnIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    breakWaterHeightColumnIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    useForeshoreColumnIndex, new DataGridViewColumnStateDefinition()
                }
            };

            UpdateUseBreakWaterColumnStateDefinitions();
            UpdateBreakWaterTypeAndHeightColumnStateDefinitions();
            UpdateUseForeshoreColumnStateDefinitions();
        }

        /// <summary>
        /// Gets or sets the dike profile of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<DikeProfile> DikeProfile
        {
            get => new DataGridViewComboBoxItemWrapper<DikeProfile>(Calculation.InputParameters.DikeProfile);
            set
            {
                DikeProfile valueToSet = value?.WrappedObject;
                if (!ReferenceEquals(Calculation.InputParameters.DikeProfile, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() =>
                    {
                        Calculation.InputParameters.DikeProfile = valueToSet;
                        UpdateUseBreakWaterColumnStateDefinitions();
                        UpdateBreakWaterTypeAndHeightColumnStateDefinitions();
                        UpdateUseForeshoreColumnStateDefinitions();
                    }, PropertyChangeHandler);
                    DikeProfileChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the break water of the <see cref="GrassCoverErosionInwardsCalculationScenario"/> should be used.
        /// </summary>
        public bool UseBreakWater
        {
            get => Calculation.InputParameters.UseBreakWater;
            set
            {
                if (!Calculation.InputParameters.UseBreakWater.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() =>
                    {
                        Calculation.InputParameters.UseBreakWater = value;
                        UpdateBreakWaterTypeAndHeightColumnStateDefinitions();
                    }, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the break water type of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
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
        /// Gets or sets the break water height of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
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
        /// Gets or sets whether the foreshore profile of the <see cref="GrassCoverErosionInwardsCalculationScenario"/> should be used.
        /// </summary>
        public bool UseForeshoreGeometry
        {
            get => Calculation.InputParameters.UseForeshore;
            set
            {
                if (!Calculation.InputParameters.UseForeshore.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.UseForeshore = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dike height of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble DikeHeight
        {
            get => Calculation.InputParameters.DikeHeight;
            set
            {
                if (!Calculation.InputParameters.DikeHeight.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.DikeHeight = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mean critical flow rate of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble MeanCriticalFlowRate
        {
            get => Calculation.InputParameters.CriticalFlowRate.Mean;
            set
            {
                if (!Calculation.InputParameters.CriticalFlowRate.Mean.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.CriticalFlowRate.Mean = value, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard deviation critical flow rate of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        public RoundedDouble StandardDeviationCriticalFlowRate
        {
            get => Calculation.InputParameters.CriticalFlowRate.StandardDeviation;
            set
            {
                if (!Calculation.InputParameters.CriticalFlowRate.StandardDeviation.Equals(value))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => Calculation.InputParameters.CriticalFlowRate.StandardDeviation = value, PropertyChangeHandler);
                }
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        public override Point2D GetCalculationLocation()
        {
            return Calculation.InputParameters.DikeProfile?.WorldReferencePoint;
        }

        protected override HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get => Calculation.InputParameters.HydraulicBoundaryLocation;
            set => Calculation.InputParameters.HydraulicBoundaryLocation = value;
        }

        private void UpdateUseBreakWaterColumnStateDefinitions()
        {
            DikeProfile dikeProfile = Calculation.InputParameters.DikeProfile;

            if (dikeProfile == null)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[useBreakWaterColumnIndex]);
            }
        }

        private void UpdateBreakWaterTypeAndHeightColumnStateDefinitions()
        {
            DikeProfile dikeProfile = Calculation.InputParameters.DikeProfile;

            if (!UseBreakWater || dikeProfile == null)
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
            DikeProfile dikeProfile = Calculation.InputParameters.DikeProfile;

            if (dikeProfile == null || !dikeProfile.ForeshoreGeometry.Any())
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