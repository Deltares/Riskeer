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

using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Container for all layer related data that is required for performing calculations.
    /// </summary>
    internal class LayerWithSoil
    {
        /// <summary>
        /// Creates a new instance of <see cref="LayerWithSoil"/>.
        /// </summary>
        /// <param name="outerRing">The outer ring of the layer.</param>
        /// <param name="innerRings">The inner rings of the layer.</param>
        /// <param name="soil">The soil of the layer.</param>
        /// <param name="isAquifer">A value indicating whether the layer is an aquifer.</param>
        /// <param name="waterPressureInterpolationModel">The water pressure interpolation model of the layer.</param>
        internal LayerWithSoil(IEnumerable<Point2D> outerRing,
                               IEnumerable<IEnumerable<Point2D>> innerRings,
                               Soil soil,
                               bool isAquifer,
                               WaterpressureInterpolationModel waterPressureInterpolationModel)
        {
            OuterRing = outerRing;
            InnerRings = innerRings;
            Soil = soil;
            IsAquifer = isAquifer;
            WaterPressureInterpolationModel = waterPressureInterpolationModel;
        }

        /// <summary>
        /// Gets the outer ring of the layer.
        /// </summary>
        public IEnumerable<Point2D> OuterRing { get; }

        /// <summary>
        /// Gets the inner rings of the layer.
        /// </summary>
        public IEnumerable<IEnumerable<Point2D>> InnerRings { get; }

        /// <summary>
        /// Gets the soil of the layer.
        /// </summary>
        public Soil Soil { get; }

        /// <summary>
        /// Gets a value indicating whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; }

        /// <summary>
        /// Gets the water pressure interpolation model of the layer.
        /// </summary>
        public WaterpressureInterpolationModel WaterPressureInterpolationModel { get; }
    }
}