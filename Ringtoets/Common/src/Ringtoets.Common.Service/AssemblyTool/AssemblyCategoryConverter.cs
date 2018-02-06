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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Exceptions;

namespace Ringtoets.Common.Service.AssemblyTool
{
    /// <summary>
    /// Converter to convert <see cref="AssemblyCategoryResult"/> into <see cref="AssemblyCategory"/>.
    /// </summary>
    public static class AssemblyCategoryConverter
    {
        /// <summary>
        /// Converts an <see cref="IEnumerable{T}" /> of <see cref="AssemblyCategoryResult"/> into an
        /// <see cref="IEnumerable{T}"/> of <see cref="AssemblyCategory"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>The converted categories.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any element in <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyCategoryConversionException">Thrown when <paramref name="result"/>
        /// cannot be successfully converted into an <see cref="AssessmentSectionAssemblyCategory"/>.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> ConvertToAssessmentSectionAssemblyCategories(
            IEnumerable<AssessmentSectionAssemblyCategoryResult> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.Contains(null))
            {
                throw new ArgumentException(@"Result cannot contain null.", nameof(result));
            }

            return result.Select(ConvertAssessmentSectionAssemblyCategory).ToArray();
        }

        /// <summary>
        /// Converts <see cref="AssemblyCategoryResult"/> into <see cref="AssemblyCategory"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>The converted category.</returns>
        /// <exception cref="AssemblyCategoryConversionException">Thrown when <paramref name="result"/>
        /// cannot be successfully converted into an <see cref="AssessmentSectionAssemblyCategory"/>.</exception>
        private static AssessmentSectionAssemblyCategory ConvertAssessmentSectionAssemblyCategory(AssessmentSectionAssemblyCategoryResult result)
        {
            try
            {
                return new AssessmentSectionAssemblyCategory(result.LowerBoundary, result.UpperBoundary,
                                                             ConvertToAssessmentSectionAssemblyCategoryGroup(result.Category));
            }
            catch (Exception e) when (e is InvalidEnumArgumentException || e is NotSupportedException)
            {
                throw new AssemblyCategoryConversionException(e.Message, e);
            }
        }

        /// <summary>
        /// Converts <see cref="AssessmentSectionAssemblyCategoryResultType"/> into
        /// <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryType">The category type to convert.</param>
        /// <returns>The converted category group.</returns>
        /// /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="AssessmentSectionAssemblyCategoryResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="AssessmentSectionAssemblyCategoryResultType"/>
        /// is a valid value, but unsupported.</exception>
        private static AssessmentSectionAssemblyCategoryGroup ConvertToAssessmentSectionAssemblyCategoryGroup(
            AssessmentSectionAssemblyCategoryResultType categoryType)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategoryResultType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(categoryType),
                                                       (int) categoryType,
                                                       typeof(AssessmentSectionAssemblyCategoryResultType));
            }

            switch (categoryType)
            {
                case AssessmentSectionAssemblyCategoryResultType.APlus:
                    return AssessmentSectionAssemblyCategoryGroup.APlus;
                case AssessmentSectionAssemblyCategoryResultType.A:
                    return AssessmentSectionAssemblyCategoryGroup.A;
                case AssessmentSectionAssemblyCategoryResultType.B:
                    return AssessmentSectionAssemblyCategoryGroup.B;
                case AssessmentSectionAssemblyCategoryResultType.C:
                    return AssessmentSectionAssemblyCategoryGroup.C;
                case AssessmentSectionAssemblyCategoryResultType.D:
                    return AssessmentSectionAssemblyCategoryGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}