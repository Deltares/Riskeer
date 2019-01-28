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

using Ringtoets.Common.Data;
using Ringtoets.Common.IO.Exceptions;

namespace Ringtoets.Common.IO.SurfaceLines
{
    /// <summary>
    /// Interface for transforming generic surface lines into mechanism specific surface lines.
    /// </summary>
    /// <typeparam name="T">The type of the mechanism specific surface line.</typeparam>
    public interface ISurfaceLineTransformer<out T> where T : IMechanismSurfaceLine
    {
        /// <summary>
        /// Transforms the generic <paramref name="surfaceLine"/> into a mechanism specific surface line
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to use in the transformation.</param>
        /// <param name="characteristicPoints">The characteristic points to use in the transformation.</param>
        /// <returns>A new <typeparamref name="T"/> based on the given data.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        T Transform(SurfaceLine surfaceLine, CharacteristicPoints characteristicPoints);
    }
}