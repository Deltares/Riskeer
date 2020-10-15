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
using Core.Common.Gui.PropertyBag;
using Riskeer.Piping.Data.SoilProfile;

namespace Riskeer.Piping.Forms.UITypeEditors
{
    public interface IHasStochasticSoilProfile : IObjectProperties
    {
        /// <summary>
        /// Returns the collection of surface lines.
        /// </summary>
        /// <returns>A collection of surface lines.</returns>
        IEnumerable<PipingStochasticSoilProfile> GetAvailableStochasticSoilProfiles();

        /// <summary>
        /// Gets the piping surface line.
        /// </summary>
        PipingStochasticSoilProfile StochasticSoilProfile { get; }
    }
}
