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
using System.Drawing;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingSoilLayer"/> based on the
    /// <see cref="SoilLayerEntity"/>.
    /// </summary>
    internal static class SoilLayerEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="SoilLayerEntity"/> and use the information to construct a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> to create <see cref="PipingSoilLayer"/> for.</param>
        /// <returns>A new <see cref="PipingSoilLayer"/>.</returns>
        internal static PipingSoilLayer Read(this SoilLayerEntity entity)
        {
            var pipingSoilLayer = new PipingSoilLayer(entity.Top.ToNullAsNaN())
            {
                IsAquifer = Convert.ToBoolean(entity.IsAquifer),
                Color = Color.FromArgb(Convert.ToInt32(entity.Color)),
                MaterialName = entity.MaterialName ?? string.Empty,
                BelowPhreaticLevelMean = entity.BelowPhreaticLevelMean.ToNullAsNaN(),
                BelowPhreaticLevelDeviation = entity.BelowPhreaticLevelDeviation.ToNullAsNaN(),
                BelowPhreaticLevelShift = entity.BelowPhreaticLevelShift.ToNullAsNaN(),
                DiameterD70Mean = entity.DiameterD70Mean.ToNullAsNaN(),
                DiameterD70CoefficientOfVariation = entity.DiameterD70CoefficientOfVariation.ToNullAsNaN(),
                PermeabilityMean = entity.PermeabilityMean.ToNullAsNaN(),
                PermeabilityCoefficientOfVariation = entity.PermeabilityCoefficientOfVariation.ToNullAsNaN()
            };
            return pipingSoilLayer;
        }
    }
}