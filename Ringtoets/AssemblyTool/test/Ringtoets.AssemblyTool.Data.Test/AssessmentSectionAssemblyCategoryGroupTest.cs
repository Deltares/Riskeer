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

namespace Ringtoets.AssemblyTool.Data.Test
{
    [TestFixture]
    public class AssessmentSectionAssemblyCategoryGroupTest : EnumWithDisplayNameTestFixture<AssessmentSectionAssemblyCategoryGroup>
    {
        protected override IDictionary<AssessmentSectionAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<AssessmentSectionAssemblyCategoryGroup, int>
                {
                    {
                        AssessmentSectionAssemblyCategoryGroup.None, 1
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.NotAssessed, 3
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.APlus, 4
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.A, 5
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.B, 6
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.C, 7
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.D, 8
                    }
                };
            }
        }

        protected override IDictionary<AssessmentSectionAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<AssessmentSectionAssemblyCategoryGroup, string>
                {
                    {
                        AssessmentSectionAssemblyCategoryGroup.None, ""
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.APlus, "A+"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.A, "A"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.B, "B"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.C, "C"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.D, "D"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.NotApplicable, "-"
                    },
                    {
                        AssessmentSectionAssemblyCategoryGroup.NotAssessed, "NGO"
                    }
                };
            }
        }
    }
}