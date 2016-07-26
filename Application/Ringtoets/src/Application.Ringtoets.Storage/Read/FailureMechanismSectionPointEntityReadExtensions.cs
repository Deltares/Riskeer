﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operationsn for a <see cref="Point2D"/> based on the
    /// <see cref="ReferenceLinePointEntity"/>.
    /// </summary>
    internal static class FailureMechanismSectionPointEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ReferenceLinePointEntity"/> and use the information to construct a <see cref="Point2D"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionPointEntity"/> to create <see cref="Point2D"/> for.</param>
        /// <returns>A new <see cref="Point2D"/>.</returns>
        internal static Point2D Read(this FailureMechanismSectionPointEntity entity)
        {
            return new Point2D(entity.X.ToNullAsNaN(), entity.Y.ToNullAsNaN());
        }
    }
}