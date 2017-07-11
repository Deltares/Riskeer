﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Service.ValidationRules
{
    /// <summary>
    /// Interface to register structure validation rules.
    /// </summary>
    /// <typeparam name="TStructuresInput">The type of the structure input.</typeparam>
    /// <typeparam name="TStructure">The type of the structure.</typeparam>
    public interface IStructuresValidationRulesRegistry<in TStructuresInput, TStructure>
        where TStructuresInput : StructuresInputBase<TStructure>
        where TStructure : StructureBase
    {
        /// <summary>
        /// Gets the validation rules for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to get the validation rules for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationRule"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        IEnumerable<ValidationRule> GetValidationRules(TStructuresInput input);
    }
}