// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocationCalculation"/>.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationRow : CalculatableRow<HydraulicBoundaryLocationCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The <see cref="HydraulicBoundaryLocationCalculation"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        internal HydraulicBoundaryLocationCalculationRow(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
            : base(hydraulicBoundaryLocationCalculation) {}

        /// <summary>
        /// Gets or sets the value indicating whether the illustration points need to be included.
        /// </summary>
        public bool IncludeIllustrationPoints
        {
            get
            {
                return CalculatableObject.InputParameters.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                CalculatableObject.InputParameters.ShouldIllustrationPointsBeCalculated = value;
                CalculatableObject.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return CalculatableObject.HydraulicBoundaryLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return CalculatableObject.HydraulicBoundaryLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return CalculatableObject.HydraulicBoundaryLocation.Location;
            }
        }

        /// <summary>
        /// Gets the result of the <see cref="HydraulicBoundaryLocationCalculation"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble Result
        {
            get
            {
                return CalculatableObject.Output?.Result ?? RoundedDouble.NaN;
            }
        }
    }
}