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

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Interface for an illustration point entity.
    /// </summary>
    public interface ITopLevelIllustrationPointEntity
    {
        /// <summary>
        /// Gets or sets the closing situation.
        /// </summary>
        string ClosingSituation { get; set; }

        /// <summary>
        /// Gets or sets the name of the wind direction.
        /// </summary>
        string WindDirectionName { get; set; }

        /// <summary>
        /// Gets or sets the angle of the wind direction.
        /// </summary>
        double WindDirectionAngle { get; set; }

        /// <summary>
        /// Gets or sets the order in which the value resides in its parent.
        /// </summary>
        int Order { get; set; }
    }
}