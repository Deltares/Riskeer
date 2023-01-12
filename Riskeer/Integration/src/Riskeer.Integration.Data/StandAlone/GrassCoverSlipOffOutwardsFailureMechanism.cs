// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Properties;

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Grass Cover Slip Off Outwards failure mechanism.
    /// </summary>
    public class GrassCoverSlipOffOutwardsFailureMechanism : FailureMechanismBase<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>,
                                                             IHasGeneralInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism()
            : base(Resources.GrassCoverSlipOffOutwardsFailureMechanism_DisplayName, Resources.GrassCoverSlipOffOutwardsFailureMechanism_Code)
        {
            GeneralInput = new GeneralInput();
        }

        public GeneralInput GeneralInput { get; }
    }
}