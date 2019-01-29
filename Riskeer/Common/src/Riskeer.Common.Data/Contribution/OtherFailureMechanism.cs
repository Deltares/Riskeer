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

using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Contribution
{
    /// <summary>
    /// This class represents a failure mechanism which has no representative within Ringtoets but 
    /// contributes to the overall verdict nonetheless.
    /// </summary>
    public class OtherFailureMechanism : FailureMechanismBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="OtherFailureMechanism"/>.
        /// </summary>
        public OtherFailureMechanism() : base(Resources.OtherFailureMechanism_DisplayName, Resources.OtherFailureMechanism_Code, int.MinValue) {}

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }
    }
}