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
using Core.Common.Base;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Adapter class for a design water level calculation.
    /// </summary>
    public class DesignWaterLevelCalculation : IDesignWaterLevelCalculation
    {
        private readonly HydraulicBoundaryLocation hydraulicBoundaryLocation;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> 
        /// is <c>null</c>.</exception>
        public DesignWaterLevelCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }
            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;
        }

        /// <summary>
        /// Gets a value indicating whether this calculation has already been calculated.
        /// </summary>
        /// <returns></returns>
        public bool IsCalculated()
        {
            HydraulicBoundaryLocationCalculation calculation = hydraulicBoundaryLocation.DesignWaterLevelCalculation;
            return calculation.HasOutput
                   && calculation.InputParameters.ShouldIllustrationPointsBeCalculated == calculation.Output.HasIllustrationPoints;
        }

        /// <summary>
        /// Gets the observable object to notify upon an internal state change.
        /// </summary>
        /// <returns></returns>
        public IObservable GetObservableObject()
        {
            return hydraulicBoundaryLocation;
        }

        public long GetId()
        {
            return hydraulicBoundaryLocation.Id;
        }

        public string GetName()
        {
            return hydraulicBoundaryLocation.Name;
        }

        public bool GetCalculateIllustrationPoints()
        {
            return hydraulicBoundaryLocation.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
        }

        public void SetOutput(HydraulicBoundaryLocationOutput output)
        {
            hydraulicBoundaryLocation.DesignWaterLevelCalculation.Output = output;
        }
    }
}