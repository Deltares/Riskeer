// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Common.Base.Service
{
    /// <summary>
    /// Enumeration that defines the possible states of an <see cref="Activity"/>.
    /// </summary>
    public enum ActivityState
    {
        /// <summary>
        /// The state of an <see cref="Activity"/> that is about to be run.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        None,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is successfully ran.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        Executed,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is not successfully ran.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        Failed,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is successfully canceled.
        /// <seealso cref="Activity.Cancel"/>
        /// </summary>
        Canceled,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is skipped during run.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        Skipped,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is successfully finished.
        /// <seealso cref="Activity.Finish"/>
        /// </summary>
        Finished
    }
}