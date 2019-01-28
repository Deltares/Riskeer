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

namespace Ringtoets.Common.Data.DikeProfiles
{
    /// <summary>
    /// Interface for objects which can have a foreshore profile.
    /// </summary>
    public interface IHasForeshoreProfile
    {
        /// <summary>
        /// Gets or sets the foreshore profile.
        /// </summary>
        ForeshoreProfile ForeshoreProfile { get; set; }

        /// <summary>
        /// Gets the value <c>true</c> if the parameters of the instance of 
        /// <see cref="IHasForeshoreProfile"/> that are derived from 
        /// <see cref="ForeshoreProfile"/> match the properties of
        /// <see cref="ForeshoreProfile"/>; or <c>false</c> if this 
        /// is not the case, or if there is no <see cref="ForeshoreProfile"/>
        /// assigned.
        /// </summary>
        bool IsForeshoreProfileInputSynchronized { get; }

        /// <summary>
        /// Applies the properties of the <see cref="ForeshoreProfile"/> to
        /// the parameters of the instance of <see cref="IHasForeshoreProfile"/>.
        /// </summary>
        /// <remarks>When no foreshore profile is present, the input parameters are set to default values.</remarks>
        void SynchronizeForeshoreProfileInput();
    }
}