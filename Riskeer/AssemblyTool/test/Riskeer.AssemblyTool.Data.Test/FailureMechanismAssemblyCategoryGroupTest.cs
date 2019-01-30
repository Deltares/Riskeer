// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoryGroupTest : EnumWithResourcesDisplayNameTestFixture<FailureMechanismAssemblyCategoryGroup>
    {
        protected override IDictionary<FailureMechanismAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<FailureMechanismAssemblyCategoryGroup, int>
                {
                    {
                        FailureMechanismAssemblyCategoryGroup.None, 1
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.It, 3
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IIt, 4
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IIIt, 5
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IVt, 6
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.Vt, 7
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.VIt, 8
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.VIIt, 9
                    }
                };
            }
        }

        protected override IDictionary<FailureMechanismAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<FailureMechanismAssemblyCategoryGroup, string>
                {
                    {
                        FailureMechanismAssemblyCategoryGroup.None, ""
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.NotApplicable, "-"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.It, "It"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IIt, "IIt"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IIIt, "IIIt"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.IVt, "IVt"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.Vt, "Vt"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.VIt, "VIt"
                    },
                    {
                        FailureMechanismAssemblyCategoryGroup.VIIt, "VIIt"
                    }
                };
            }
        }
    }
}