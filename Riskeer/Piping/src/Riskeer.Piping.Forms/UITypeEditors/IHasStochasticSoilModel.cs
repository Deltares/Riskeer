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

using System.Collections.Generic;
using Core.Common.Gui.PropertyBag;
using Riskeer.Piping.Data.SoilProfile;

namespace Riskeer.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// Interface for objects which can have a stochastic soil model.
    /// </summary>
    public interface IHasStochasticSoilModel : IObjectProperties
    {
        /// <summary>
        /// Gets the stochastic soil model.
        /// </summary>
        PipingStochasticSoilModel StochasticSoilModel { get; }

        /// <summary>
        /// Returns the collection of stochastic soil models.
        /// </summary>
        /// <returns>A collection of stochastic soil models.</returns>
        IEnumerable<PipingStochasticSoilModel> GetAvailableStochasticSoilModels();
    }
}