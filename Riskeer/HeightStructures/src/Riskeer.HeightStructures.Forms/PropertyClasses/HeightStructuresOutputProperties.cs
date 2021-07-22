﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of height structures <see cref="StructuresOutput"/> for properties panel.
    /// </summary>
    public class HeightStructuresOutputProperties : StructuresOutputProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresOutputProperties"/>.
        /// </summary>
        /// <param name="structuresOutput">The structures output to create the object properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structuresOutput"/> is <c>null</c>.</exception>
        public HeightStructuresOutputProperties(StructuresOutput structuresOutput)
            : base(structuresOutput) {}
    }
}