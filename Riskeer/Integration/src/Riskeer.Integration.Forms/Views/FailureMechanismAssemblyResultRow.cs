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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="IFailureMechanism"/>
    /// and its <see cref="FailureMechanismAssembly"/>.
    /// </summary>
    internal class FailureMechanismAssemblyResultRow : FailureMechanismAssemblyResultRowBase
    {
        private readonly Func<FailureMechanismAssembly> getFailureMechanismAssembly;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoryGroupResultRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to wrap so that it can be displayed as a row.</param>
        /// <param name="getFailureMechanismAssembly">Gets the <see cref="FailureMechanismAssembly"/>
        /// of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyResultRow(IFailureMechanism failureMechanism,
                                                 Func<FailureMechanismAssembly> getFailureMechanismAssembly)
            : base(failureMechanism)
        {
            if (getFailureMechanismAssembly == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismAssembly));
            }

            this.getFailureMechanismAssembly = getFailureMechanismAssembly;

            Update();
        }

        protected override void TryGetDerivedData()
        {
            try
            {
                FailureMechanismAssembly failureMechanismAssembly = getFailureMechanismAssembly();
                CategoryGroup = failureMechanismAssembly.Group;
                Probability = failureMechanismAssembly.Probability;
            }
            catch (AssemblyException e)
            {
                CategoryGroup = FailureMechanismAssemblyCategoryGroup.None;
                Probability = double.NaN;
                GetCategoryGroupColumnStateDefinition().ErrorText = e.Message;
            }
        }
    }
}