﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// The <see cref="DateTimeService"/> class is responsible for providing general <see cref="DateTime"/> operations.
    /// </summary>
    public static class DateTimeService
    {
        /// <summary>
        /// Gets the current time in HH:mm:ss format.
        /// </summary>
        public static String CurrentTimeAsString
        {
            get
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }
    }
}