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

using System;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="PipingFailureMechanismResult"/>.
    /// </summary>
    public class PipingFailureMechanismResultContext
    {
        public PipingFailureMechanism FailureMechanism { get; private set; }
        public PipingFailureMechanismResult FailureMechanismResult { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultContext"/>.
        /// </summary>
        /// <param name="failureMechanismResult">The <see cref="PipingFailureMechanismResult"/> that is wrapped.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> that is wrapped.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismResult"/> or <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public PipingFailureMechanismResultContext(PipingFailureMechanismResult failureMechanismResult, PipingFailureMechanism failureMechanism)
        {
            if (failureMechanismResult == null)
            {
                throw new ArgumentNullException("failureMechanismResult");
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            FailureMechanismResult = failureMechanismResult;
            FailureMechanism = failureMechanism;
        }
    }
}
