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

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This class represents the equally named table in the database storage.
    /// </summary>
    public partial class DuneAssessmentSectionEntity : IComparable<DuneAssessmentSectionEntity>
    {
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object. </param>
        /// <returns>A value that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.</returns>
        public int CompareTo(DuneAssessmentSectionEntity other)
        {
            if (Order > other.Order)
            {
                return 1;
            }
            if (Order < other.Order)
            {
                return -1;
            }
            return String.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}