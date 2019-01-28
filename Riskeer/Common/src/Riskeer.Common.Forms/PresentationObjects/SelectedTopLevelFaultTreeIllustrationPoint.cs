// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Class that represents a selected top level fault tree illustration point.
    /// </summary>
    public class SelectedTopLevelFaultTreeIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectedTopLevelFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="topLevelFaultTreeIllustrationPoint">The top level fault tree illustration point.</param>
        /// <param name="closingSituations">The closing situations that are present.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SelectedTopLevelFaultTreeIllustrationPoint(TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint,
                                                          IEnumerable<string> closingSituations)
        {
            if (topLevelFaultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(topLevelFaultTreeIllustrationPoint));
            }

            if (closingSituations == null)
            {
                throw new ArgumentNullException(nameof(closingSituations));
            }

            TopLevelFaultTreeIllustrationPoint = topLevelFaultTreeIllustrationPoint;
            ClosingSituations = closingSituations;
        }

        /// <summary>
        /// Gets the top level fault tree illustration point.
        /// </summary>
        public TopLevelFaultTreeIllustrationPoint TopLevelFaultTreeIllustrationPoint { get; }

        /// <summary>
        /// Gets the calculated closing situations.
        /// </summary>
        public IEnumerable<string> ClosingSituations { get; }
    }
}