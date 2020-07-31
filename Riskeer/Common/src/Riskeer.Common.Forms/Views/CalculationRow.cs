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
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="TCalculation"/> in the <see cref="CalculationsView{TCalculation, TCalculationInput, TCalculationRow, TFailureMechanism}"/>.
    /// </summary>
    /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
    public abstract class CalculationRow<TCalculation>
        where TCalculation : class, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculationRow{TCalculation}"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ICalculation"/> this row contains.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CalculationRow(TCalculation calculation, IObservablePropertyChangeHandler propertyChangeHandler)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (propertyChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(propertyChangeHandler));
            }

            Calculation = calculation;
            PropertyChangeHandler = propertyChangeHandler;
        }

        /// <summary>
        /// Gets the <see cref="ICalculation"/> this row contains.
        /// </summary>
        public TCalculation Calculation { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="Calculation"/>.
        /// </summary>
        public string Name
        {
            get => Calculation.Name;
            set
            {
                Calculation.Name = value;
                Calculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the hydraulic boundary location of the <see cref="Calculation"/>.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> SelectableHydraulicBoundaryLocation
        {
            get
            {
                if (HydraulicBoundaryLocation == null)
                {
                    return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null);
                }

                return new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(
                    new SelectableHydraulicBoundaryLocation(HydraulicBoundaryLocation,
                                                            GetCalculationLocation()));
            }
            set
            {
                HydraulicBoundaryLocation valueToSet = value?.WrappedObject?.HydraulicBoundaryLocation;
                if (!ReferenceEquals(HydraulicBoundaryLocation, valueToSet))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => HydraulicBoundaryLocation = valueToSet, PropertyChangeHandler);
                }
            }
        }

        /// <summary>
        /// Gets the location of the <see cref="Calculation"/>.
        /// </summary>
        /// <returns>The location of the <see cref="Calculation"/>.</returns>
        public abstract Point2D GetCalculationLocation();

        /// <summary>
        /// Gets the <see cref="IObservablePropertyChangeHandler"/>.
        /// </summary>
        protected IObservablePropertyChangeHandler PropertyChangeHandler { get; }

        /// <summary>
        /// Gets or sets the <see cref="HydraulicBoundaryLocation"/> of the <see cref="Calculation"/>.
        /// </summary>
        protected abstract HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }
    }
}