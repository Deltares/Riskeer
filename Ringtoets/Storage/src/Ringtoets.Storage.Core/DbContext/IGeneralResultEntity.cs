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

using System.Collections.Generic;

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Interface for a general result entity.
    /// </summary>
    public interface IGeneralResultEntity
    {
        /// <summary>
        /// Gets or sets the name of the governing wind direction.
        /// </summary>
        string GoverningWindDirectionName { get; set; }

        /// <summary>
        /// Gets or sets the angle of the governing wind direction.
        /// </summary>
        double GoverningWindDirectionAngle { get; set; }

        /// <summary>
        /// Gets the general alpha values.
        /// </summary>
        ICollection<StochastEntity> StochastEntities { get; }
    }
}