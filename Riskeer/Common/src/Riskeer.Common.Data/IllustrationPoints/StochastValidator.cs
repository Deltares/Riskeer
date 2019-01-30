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
using System.Collections.Generic;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// This class contains validations methods for <see cref="Stochast"/> collections.
    /// </summary>
    public static class StochastValidator
    {
        /// <summary>
        /// Validates a collection of <see cref="Stochast"/> objects by checking for duplicate names.
        /// </summary>
        /// <param name="stochasts">The collection of <see cref="Stochast"/> objects to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="stochasts"/> contains duplicate stochasts.</exception>
        public static void ValidateStochasts(IEnumerable<Stochast> stochasts)
        {
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            if (stochasts.HasDuplicates(s => s.Name))
            {
                throw new ArgumentException(string.Format(Resources.GeneralResult_Imported_non_unique_stochasts));
            }
        }
    }
}