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

using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class defines the possible outcomes of a detailed assessment of safety per failure mechanism section.
    /// </summary>
    public enum AssessmentLayerTwoAResult
    {
        /// <summary>
        /// No assessment for the failure mechanism section has been performed.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), "AssessmentLayerTwoAResult_NotCalculated")]
        NotCalculated = 1,

        /// <summary>
        /// An assessment for the failure mechanism section was performed and the outcome is 
        /// that it failed the assessment.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), "AssessmentLayerTwoAResult_Failed")]
        Failed,

        /// <summary>
        /// An assessment for the failure mechanism section was performed and the outcome is 
        /// that it passed the assessment.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), "AssessmentLayerTwoAResult_Successful")]
        Successful
    }
}