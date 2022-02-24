// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class AssessmentSectionAssemblyCategoryGroupTest : EnumWithResourcesDisplayNameTestFixture<AssessmentSectionAssemblyGroup>
    {
        protected override IDictionary<AssessmentSectionAssemblyGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<AssessmentSectionAssemblyGroup, int>
                {
                    {
                        AssessmentSectionAssemblyGroup.None, 1
                    },
                    {
                        AssessmentSectionAssemblyGroup.NotApplicable, 2
                    },
                    {
                        AssessmentSectionAssemblyGroup.NotAssessed, 3
                    },
                    {
                        AssessmentSectionAssemblyGroup.APlus, 4
                    },
                    {
                        AssessmentSectionAssemblyGroup.A, 5
                    },
                    {
                        AssessmentSectionAssemblyGroup.B, 6
                    },
                    {
                        AssessmentSectionAssemblyGroup.C, 7
                    },
                    {
                        AssessmentSectionAssemblyGroup.D, 8
                    }
                };
            }
        }

        protected override IDictionary<AssessmentSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<AssessmentSectionAssemblyGroup, string>
                {
                    {
                        AssessmentSectionAssemblyGroup.None, ""
                    },
                    {
                        AssessmentSectionAssemblyGroup.APlus, "A+"
                    },
                    {
                        AssessmentSectionAssemblyGroup.A, "A"
                    },
                    {
                        AssessmentSectionAssemblyGroup.B, "B"
                    },
                    {
                        AssessmentSectionAssemblyGroup.C, "C"
                    },
                    {
                        AssessmentSectionAssemblyGroup.D, "D"
                    },
                    {
                        AssessmentSectionAssemblyGroup.NotApplicable, "-"
                    },
                    {
                        AssessmentSectionAssemblyGroup.NotAssessed, "NGO"
                    }
                };
            }
        }
    }
}