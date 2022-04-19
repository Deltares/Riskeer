﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.PresentationObjects.CalculationsState
{
    /// <summary>
    /// Presentation object for <see cref="HeightStructuresFailureMechanism"/> in the calculations state.
    /// </summary>
    public class HeightStructuresFailureMechanismContext : FailureMechanismContext<HeightStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailureMechanismContext"/>.
        /// </summary>
        /// <param name="wrappedFailureMechanism">The <see cref="HeightStructuresFailureMechanism"/>
        /// instance wrapped by this context object.</param>
        /// <param name="parent">The assessment section which the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresFailureMechanismContext(HeightStructuresFailureMechanism wrappedFailureMechanism, IAssessmentSection parent)
            : base(wrappedFailureMechanism, parent) {}
    }
}