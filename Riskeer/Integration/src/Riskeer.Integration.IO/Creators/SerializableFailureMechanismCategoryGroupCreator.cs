// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.ComponentModel;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableFailureMechanismCategoryGroup"/>.
    /// </summary>
    public static class SerializableFailureMechanismCategoryGroupCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableFailureMechanismCategoryGroup"/> based on <paramref name="categoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The <see cref="FailureMechanismAssemblyCategoryGroup"/> to
        /// create a <see cref="SerializableFailureMechanismCategoryGroup"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismCategoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableFailureMechanismCategoryGroup Create(FailureMechanismAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(FailureMechanismAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case FailureMechanismAssemblyCategoryGroup.NotApplicable:
                    return SerializableFailureMechanismCategoryGroup.NotApplicable;
                case FailureMechanismAssemblyCategoryGroup.It:
                    return SerializableFailureMechanismCategoryGroup.It;
                case FailureMechanismAssemblyCategoryGroup.IIt:
                    return SerializableFailureMechanismCategoryGroup.IIt;
                case FailureMechanismAssemblyCategoryGroup.IIIt:
                    return SerializableFailureMechanismCategoryGroup.IIIt;
                case FailureMechanismAssemblyCategoryGroup.IVt:
                    return SerializableFailureMechanismCategoryGroup.IVt;
                case FailureMechanismAssemblyCategoryGroup.Vt:
                    return SerializableFailureMechanismCategoryGroup.Vt;
                case FailureMechanismAssemblyCategoryGroup.VIt:
                    return SerializableFailureMechanismCategoryGroup.VIt;
                case FailureMechanismAssemblyCategoryGroup.VIIt:
                    return SerializableFailureMechanismCategoryGroup.VIIt;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}