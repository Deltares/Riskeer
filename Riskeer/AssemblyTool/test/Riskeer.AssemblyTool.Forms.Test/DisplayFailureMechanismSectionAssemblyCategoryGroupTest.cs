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

namespace Riskeer.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class DisplayFailureMechanismSectionAssemblyCategoryGroupTest : EnumWithResourcesDisplayNameTestFixture<DisplayFailureMechanismSectionAssemblyCategoryGroup>
    {
        protected override IDictionary<DisplayFailureMechanismSectionAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<DisplayFailureMechanismSectionAssemblyCategoryGroup, int>
                {
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.None, 1
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv, 3
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv, 4
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv, 5
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv, 6
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv, 7
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv, 8
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv, 9
                    }
                };
            }
        }

        protected override IDictionary<DisplayFailureMechanismSectionAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<DisplayFailureMechanismSectionAssemblyCategoryGroup, string>
                {
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.None, ""
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, "-"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv, "Iv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv, "IIv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv, "IIIv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv, "IVv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv, "Vv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv, "VIv"
                    },
                    {
                        DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv, "VIIv"
                    }
                };
            }
        }
    }
}