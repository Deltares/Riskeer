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

using System;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for an instance of <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructuresFailureMechanismContext : FailureMechanismContext<HeightStructuresFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeightStructuresFailureMechanismContext"/> class.
        /// </summary>
        /// <param name="wrappedFailureMechanism">The <see cref="HeightStructuresFailureMechanism"/> instance wrapped by this context object.</param>
        /// <param name="parent">The assessment section which the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedFailureMechanism"/> or <paramref name="parent"/> is <c>null</c>.</exception>
        public HeightStructuresFailureMechanismContext(HeightStructuresFailureMechanism wrappedFailureMechanism, IAssessmentSection parent)
            : base(wrappedFailureMechanism, parent) {}
    }
}
