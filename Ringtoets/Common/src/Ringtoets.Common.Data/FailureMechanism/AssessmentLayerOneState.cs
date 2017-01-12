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

using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This enum defines the possible statuses for assessment layer one for each failure mechanism section
    /// </summary>
    public enum AssessmentLayerOneState
    {
        /// <summary>
        /// The failure mechanism section was not assessed.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentLayerOneState_NotAssessed))]
        NotAssessed = 1,

        /// <summary>
        /// The assessment of the failure mechanism section was sufficient or not relevant.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentLayerOneState_Sufficient))]
        Sufficient = 2,

        /// <summary>
        /// The assessment of the failure mechanism section has not reached a verdict yet.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentLayerOneState_NoVerdict))]
        NoVerdict = 3
    }
}