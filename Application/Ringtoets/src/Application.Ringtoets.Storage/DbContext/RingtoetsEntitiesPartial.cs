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

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Partial implementation of <see cref="RingtoetsEntities"/> that support a connection string and does not read the connection string from the configuration.
    /// </summary>
    public partial class RingtoetsEntities
    {
        /// <summary>
        /// A new instance of <see cref="RingtoetsEntities"/>.
        /// </summary>
        /// <param name="connString">A connection string.</param>
        public RingtoetsEntities(string connString)
            : base(connString) {}
    }
}