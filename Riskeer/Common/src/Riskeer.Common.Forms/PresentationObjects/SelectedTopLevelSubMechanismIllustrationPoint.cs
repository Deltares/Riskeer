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

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Class that represents a top level sub mechanism illustration point together 
    /// with all the calculated closing situations that are present.
    /// </summary>
    public class SelectedTopLevelSubMechanismIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectedTopLevelSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="topLevelSubMechanismIllustrationPoint">The top level sub mechanism illustration point.</param>
        /// <param name="closingSituations">The closing situations that are present.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SelectedTopLevelSubMechanismIllustrationPoint(TopLevelSubMechanismIllustrationPoint topLevelSubMechanismIllustrationPoint,
                                                             IEnumerable<string> closingSituations)
        {
            if (topLevelSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(topLevelSubMechanismIllustrationPoint));
            }

            if (closingSituations == null)
            {
                throw new ArgumentNullException(nameof(closingSituations));
            }

            TopLevelSubMechanismIllustrationPoint = topLevelSubMechanismIllustrationPoint;
            ClosingSituations = closingSituations;
        }

        /// <summary>
        /// Gets the top level sub mechanism illustration point.
        /// </summary>
        public TopLevelSubMechanismIllustrationPoint TopLevelSubMechanismIllustrationPoint { get; }

        /// <summary>
        /// Gets the calculated closing situations.
        /// </summary>
        public IEnumerable<string> ClosingSituations { get; }
    }
}