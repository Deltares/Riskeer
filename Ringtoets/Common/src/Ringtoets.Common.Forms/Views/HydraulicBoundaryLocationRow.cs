// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    public class HydraulicBoundaryLocationRow : CalculatableRow<HydraulicBoundaryLocation>
    {
        private readonly HydraulicBoundaryLocationCalculation calculation;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for this row.</param>
        /// <param name="hydraulicBoundaryLocationCalculation">The calculation of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public HydraulicBoundaryLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
            : base(hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocationCalculation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculation));
            }
            calculation = hydraulicBoundaryLocationCalculation;
        }

        /// <summary>
        /// Gets or sets if the illustration points need to be included.
        /// </summary>
        public bool IncludeIllustrationPoints
        {
            get
            {
                return calculation.InputParameters.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                calculation.InputParameters.ShouldIllustrationPointsBeCalculated = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return CalculatableObject.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return CalculatableObject.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return CalculatableObject.Location;
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
                return calculation.Output?.Result ?? RoundedDouble.NaN;
            }
        }
    }
}