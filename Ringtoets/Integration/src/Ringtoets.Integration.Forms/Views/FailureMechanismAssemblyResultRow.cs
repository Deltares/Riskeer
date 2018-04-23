﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its assembly result.
    /// </summary>
    public class FailureMechanismAssemblyResultRow
    {
        private readonly IFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so 
        /// that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name
        {
            get
            {
                return failureMechanism.Name;
            }
        }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public string Code
        {
            get
            {
                return failureMechanism.Code;
            }
        }

        /// <summary>
        /// Gets the group of the failure mechanism.
        /// </summary>
        public int Group
        {
            get
            {
                return failureMechanism.Group;
            }
        }
    }
}