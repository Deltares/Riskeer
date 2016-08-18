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

using Ringtoets.Common.Forms.Properties;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// This class helps in consistently writing the probability as '1 over the yearly return period'.
    /// </summary>
    public static class ProbabilityFormattingHelper
    {
        /// <summary>
        /// Formats the specified probability.
        /// </summary>
        /// <param name="probability">The probability.</param>
        /// <returns>The formatted text.</returns>
        public static string Format(double probability)
        {
            if (probability.Equals(0.0))
            {
                return string.Format(Resources.ProbabilityPerYearFormat,
                                     CommonBaseResources.RoundedDouble_ToString_PositiveInfinity);
            }
            return string.Format(Resources.ProbabilityPerYearFormat,
                                 1.0/probability);
        }
    }
}