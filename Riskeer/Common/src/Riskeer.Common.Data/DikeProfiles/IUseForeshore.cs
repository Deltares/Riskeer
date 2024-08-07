﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Common.Base;
using Core.Common.Base.Data;

namespace Riskeer.Common.Data.DikeProfiles
{
    /// <summary>
    /// This interface describes properties for the usage of <see cref="ForeshoreProfile"/>.
    /// </summary>
    public interface IUseForeshore : IObservable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ForeshoreProfile"/> needs to be taken into account.
        /// </summary>
        bool UseForeshore { get; set; }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        RoundedPoint2DCollection ForeshoreGeometry { get; }
    }
}