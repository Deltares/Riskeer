// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.TreeNodeInfos;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class to help when dealing with <see cref="CalculationType"/>.
    /// </summary>
    public static class CalculationTypeHelper
    {
        /// <summary>
        /// Gets an image based on the given <paramref name="calculationType"/>.
        /// </summary>
        /// <param name="calculationType">The <see cref="CalculationType"/> to get the image for.</param>
        /// <returns>An image.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationType"/>
        /// has an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="calculationType"/>
        /// has a valid but not supported value.</exception>
        public static Bitmap GetCalculationTypeImage(CalculationType calculationType)
        {
            if (!Enum.IsDefined(typeof(CalculationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationType),
                                                       (int) calculationType,
                                                       typeof(CalculationType));
            }

            switch (calculationType)
            {
                case CalculationType.SemiProbabilistic:
                    return Resources.SemiProbabilisticCalculationIcon;
                case CalculationType.Probabilistic:
                    return Resources.ProbabilisticCalculationIcon;
                case CalculationType.Hydraulic:
                    return Resources.HydraulicCalculationIcon;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}