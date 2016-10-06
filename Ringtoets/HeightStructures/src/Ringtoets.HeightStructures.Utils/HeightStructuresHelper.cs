// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Utils
{
    /// <summary>
    /// Class holds helper methods to match <see cref="FailureMechanismSection"/> objects 
    /// with <see cref="HeightStructuresCalculation"/> objects. 
    /// </summary>
    public static class HeightStructuresHelper
    {
        /// <summary>
        /// Determine which <see cref="FailureMechanismSection"/> geometrically contains the <see cref="HeightStructuresCalculation"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static FailureMechanismSection FailureMechanismSectionForCalculation(IEnumerable<FailureMechanismSection> sections,
                                                                                    HeightStructuresCalculation calculation)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

            return FindSectionForCalculation(sectionSegments, calculation);
        }

        private static FailureMechanismSection FindSectionForCalculation(SectionSegments[] sectionSegmentsCollection,
                                                                         HeightStructuresCalculation calculation)
        {
            var structure = calculation.InputParameters.HeightStructure;
            return structure != null
                       ? SectionSegmentsHelper.GetSectionForPoint(sectionSegmentsCollection, structure.Location)
                       : null;
        }
    }
}