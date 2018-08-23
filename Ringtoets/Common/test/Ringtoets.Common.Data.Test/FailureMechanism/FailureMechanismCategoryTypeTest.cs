// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismCategoryTypeTest : EnumWithResourcesDisplayNameTestFixture<FailureMechanismCategoryType>
    {
        protected override IDictionary<FailureMechanismCategoryType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<FailureMechanismCategoryType, string>
                {
                    {
                        FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm, "Iv->IIv"
                    },
                    {
                        FailureMechanismCategoryType.MechanismSpecificSignalingNorm, "IIv->IIIv"
                    },
                    {
                        FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, "IIIv->IVv"
                    },
                    {
                        FailureMechanismCategoryType.LowerLimitNorm, "IVv->Vv"
                    },
                    {
                        FailureMechanismCategoryType.FactorizedLowerLimitNorm, "Vv->VIv"
                    }
                };
            }
        }

        protected override IDictionary<FailureMechanismCategoryType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<FailureMechanismCategoryType, int>
                {
                    {
                        FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm, 1
                    },
                    {
                        FailureMechanismCategoryType.MechanismSpecificSignalingNorm, 2
                    },
                    {
                        FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, 3
                    },
                    {
                        FailureMechanismCategoryType.LowerLimitNorm, 4
                    },
                    {
                        FailureMechanismCategoryType.FactorizedLowerLimitNorm, 5
                    }
                };
            }
        }
    }
}