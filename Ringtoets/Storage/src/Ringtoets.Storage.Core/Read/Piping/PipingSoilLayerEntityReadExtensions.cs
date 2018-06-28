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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingSoilLayer"/> based on the
    /// <see cref="PipingSoilLayerEntity"/>.
    /// </summary>
    internal static class PipingSoilLayerEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingSoilLayerEntity"/> and use the information to construct a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilLayerEntity"/> to create <see cref="PipingSoilLayer"/> for.</param>
        /// <returns>A new <see cref="PipingSoilLayer"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public static PipingSoilLayer Read(this PipingSoilLayerEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new PipingSoilLayer(entity.Top.ToNullAsNaN())
            {
                IsAquifer = Convert.ToBoolean(entity.IsAquifer),
                Color = entity.Color.ToColor(),
                MaterialName = entity.MaterialName ?? string.Empty,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) entity.BelowPhreaticLevelMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.BelowPhreaticLevelDeviation.ToNullAsNaN(),
                    Shift = (RoundedDouble) entity.BelowPhreaticLevelShift.ToNullAsNaN()
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) entity.DiameterD70Mean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.DiameterD70CoefficientOfVariation.ToNullAsNaN()
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) entity.PermeabilityMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.PermeabilityCoefficientOfVariation.ToNullAsNaN()
                }
            };
        }
    }
}