﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Adapter class for a wave height calculation.
    /// </summary>
    public class WaveHeightCalculation : IHydraulicBoundaryCalculationWrapper
    {
        private readonly HydraulicBoundaryLocation hydraulicBoundaryLocation;
        private readonly HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location the
        /// <paramref name="hydraulicBoundaryLocationCalculation"/> belongs to.</param>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location
        /// calculation to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/>
        /// or <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        public WaveHeightCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            if (hydraulicBoundaryLocationCalculation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculation));
            }

            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;
            this.hydraulicBoundaryLocationCalculation = hydraulicBoundaryLocationCalculation;
        }

        /// <summary>
        /// Gets the observable object to notify upon an internal state change.
        /// </summary>
        public IObservable ObservableObject
        {
            get
            {
                return hydraulicBoundaryLocation;
            }
        }

        public long Id
        {
            get
            {
                return hydraulicBoundaryLocation.Id;
            }
        }

        public string Name
        {
            get
            {
                return hydraulicBoundaryLocation.Name;
            }
        }

        public bool CalculateIllustrationPoints
        {
            get
            {
                return hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
            }
        }

        public HydraulicBoundaryLocationOutput Output
        {
            get
            {
                return hydraulicBoundaryLocationCalculation.Output;
            }
            set
            {
                hydraulicBoundaryLocationCalculation.Output = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped calculation has already been calculated.
        /// </summary>
        /// <returns><c>true</c> if the calculation is fully calculated, <c>false</c> otherwise.</returns>
        /// <remarks>A calculation is fully calculated, depending on if the illustration points 
        /// are set to be calculated.</remarks>
        public bool IsCalculated()
        {
            return hydraulicBoundaryLocationCalculation.HasOutput
                   && hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated == hydraulicBoundaryLocationCalculation.Output.HasGeneralResult;
        }
    }
}