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

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Indicator of what properties to mark as read-only for
    /// <see cref="VariationCoefficientDistributionPropertiesBase{T}"/>.
    /// </summary>
    [Flags]
    public enum VariationCoefficientDistributionPropertiesReadOnly
    {
        /// <summary>
        /// Mark none of the properties of <see cref="VariationCoefficientDistributionPropertiesBase{T}"/>
        /// as read-only.
        /// </summary>
        None = 0,

        /// <summary>
        /// Mark <see cref="VariationCoefficientDistributionPropertiesBase{T}.Mean"/> as read-only.
        /// </summary>
        Mean = 1,

        /// <summary>
        /// Mark <see cref="VariationCoefficientDistributionPropertiesBase{T}.CoefficientOfVariation"/>
        /// as read-only.
        /// </summary>
        CoefficientOfVariation = 2,

        /// <summary>
        /// Marks both <see cref="VariationCoefficientDistributionPropertiesBase{T}.Mean"/> and
        /// <see cref="VariationCoefficientDistributionPropertiesBase{T}.CoefficientOfVariation"/>
        /// as read-only.
        /// </summary>
        All = Mean | CoefficientOfVariation
    }
}