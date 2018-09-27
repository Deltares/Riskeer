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
using Ringtoets.Common.Data;
using Ringtoets.Common.IO.Exceptions;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Interface for transforming generic stochastic soil models into mechanism specific stochastic soil models.
    /// </summary>
    /// <typeparam name="T">The type of the mechanism specific stochastic soil model.</typeparam>
    public interface IStochasticSoilModelTransformer<out T> where T : IMechanismStochasticSoilModel
    {
        /// <summary>
        /// Transforms the generic <paramref name="stochasticSoilModel"/> into a mechanism specific 
        /// stochastic soil model of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="stochasticSoilModel">The stochastic soil model to use in the transformation.</param>
        /// <returns>A new <typeparamref name="T"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilModel"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        T Transform(StochasticSoilModel stochasticSoilModel);
    }
}