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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingFaultTreeIllustrationPoint"/> data into 
    /// <see cref="FaultTreeIllustrationPoint"/> data.
    /// </summary>
    public static class FaultTreeIllustrationPointConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/> based on the 
        /// information of <paramref name="hydraRingFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraRingFaultTreeIllustrationPoint">The <see cref="HydraRingFaultTreeIllustrationPoint"/> 
        /// to base the <see cref="FaultTreeIllustrationPoint"/> to create on.</param>
        /// <returns>The newly created <see cref="FaultTreeIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingFaultTreeIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="IllustrationPointConversionException">Thrown when the <paramref name="hydraRingFaultTreeIllustrationPoint"/>
        /// cannot be converted to a <see cref="FaultTreeIllustrationPoint"/>.</exception>
        public static FaultTreeIllustrationPoint Convert(HydraRingFaultTreeIllustrationPoint hydraRingFaultTreeIllustrationPoint)
        {
            if (hydraRingFaultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(hydraRingFaultTreeIllustrationPoint));
            }

            try
            {
                CombinationType combinationType = CombinationTypeConverter.Convert(hydraRingFaultTreeIllustrationPoint.CombinationType);
                IEnumerable<Stochast> stochasts = hydraRingFaultTreeIllustrationPoint.Stochasts
                                                                                     .Select(StochastConverter.Convert)
                                                                                     .ToArray();

                return new FaultTreeIllustrationPoint(hydraRingFaultTreeIllustrationPoint.Name,
                                                      hydraRingFaultTreeIllustrationPoint.Beta,
                                                      stochasts,
                                                      combinationType);
            }
            catch (Exception e) when (e is NotSupportedException || e is InvalidEnumArgumentException)
            {
                string errorMessage = $"Could not convert the {typeof(HydraRingCombinationType)} into a {typeof(CombinationType)}.";
                throw new IllustrationPointConversionException(errorMessage, e);
            }
        }
    }
}