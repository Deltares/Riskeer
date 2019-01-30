﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its <see cref="FailureMechanismAssemblyCategoryGroup"/>.
    /// </summary>
    internal class FailureMechanismAssemblyCategoryGroupResultRow : FailureMechanismAssemblyResultRowBase
    {
        private readonly Func<FailureMechanismAssemblyCategoryGroup> getFailureMechanismAssemblyCategoryGroup;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoryGroupResultRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so that it can be displayed as a row.</param>
        /// <param name="getFailureMechanismAssemblyCategoryGroup">Gets the <see cref="FailureMechanismAssemblyCategoryGroup"/>
        /// of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCategoryGroupResultRow(IFailureMechanism failureMechanism,
                                                              Func<FailureMechanismAssemblyCategoryGroup> getFailureMechanismAssemblyCategoryGroup)
            : base(failureMechanism)
        {
            if (getFailureMechanismAssemblyCategoryGroup == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismAssemblyCategoryGroup));
            }

            this.getFailureMechanismAssemblyCategoryGroup = getFailureMechanismAssemblyCategoryGroup;
            Probability = double.NaN;

            Update();
        }

        protected override void TryGetDerivedData()
        {
            try
            {
                CategoryGroup = getFailureMechanismAssemblyCategoryGroup();
            }
            catch (AssemblyException e)
            {
                CategoryGroup = FailureMechanismAssemblyCategoryGroup.None;
                GetCategoryGroupColumnStateDefinition().ErrorText = e.Message;
            }
        }
    }
}