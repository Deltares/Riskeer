// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Piping.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingSoilLayer"/> related to creating a <see cref="PipingSoilLayerEntity"/>.
    /// </summary>
    internal static class PipingSoilLayerCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingSoilLayerEntity"/> based on the information of the <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="layer">The layer to create a database entity for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="PipingSoilLayerEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layer"/> is <c>null</c>.</exception>
        public static PipingSoilLayerEntity Create(this PipingSoilLayer layer, int order)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            return new PipingSoilLayerEntity
            {
                IsAquifer = Convert.ToByte(layer.IsAquifer),
                Top = layer.Top.ToNaNAsNull(),
                BelowPhreaticLevelMean = layer.BelowPhreaticLevel.Mean.ToNaNAsNull(),
                BelowPhreaticLevelDeviation = layer.BelowPhreaticLevel.StandardDeviation.ToNaNAsNull(),
                BelowPhreaticLevelShift = layer.BelowPhreaticLevel.Shift.ToNaNAsNull(),
                DiameterD70Mean = layer.DiameterD70.Mean.ToNaNAsNull(),
                DiameterD70CoefficientOfVariation = layer.DiameterD70.CoefficientOfVariation.ToNaNAsNull(),
                PermeabilityMean = layer.Permeability.Mean.ToNaNAsNull(),
                PermeabilityCoefficientOfVariation = layer.Permeability.CoefficientOfVariation.ToNaNAsNull(),
                Color = layer.Color.ToInt64(),
                MaterialName = layer.MaterialName.DeepClone(),
                Order = order
            };
        }
    }
}