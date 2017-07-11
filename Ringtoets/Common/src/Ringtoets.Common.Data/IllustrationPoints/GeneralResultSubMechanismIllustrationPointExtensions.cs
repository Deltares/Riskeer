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
using System.Linq;

namespace Ringtoets.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="GeneralResultSubMechanismIllustrationPoint"/>.
    /// </summary>
    public static class GeneralResultSubMechanismIllustrationPointExtensions
    {
        /// <summary>
        /// Determines whether all closing situations are the same.
        /// </summary>
        /// <param name="illustrationPoint">The illustration point to check for.</param>
        /// <returns><c>true</c> when all closing situations are the same; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="illustrationPoint"/>
        /// is <c>null</c>.</exception>
        public static bool AreAllClosingSituationsSame(this GeneralResultSubMechanismIllustrationPoint illustrationPoint)
        {
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }

            return illustrationPoint.TopLevelSubMechanismIllustrationPoints
                                    .All(ip => ip.ClosingSituation == illustrationPoint.TopLevelSubMechanismIllustrationPoints
                                                                                       .First().ClosingSituation);
        }
    }
}