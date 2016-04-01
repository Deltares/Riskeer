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
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for an instance of <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingFailureMechanismContext : FailureMechanismContext<PipingFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanismContext"/> class.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The parent of <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> or <paramref name="assessmentSection"/> are <c>null</c>.</exception>
        public PipingFailureMechanismContext(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection) :
            base(failureMechanism, assessmentSection)
        {
            
        }
    }
}