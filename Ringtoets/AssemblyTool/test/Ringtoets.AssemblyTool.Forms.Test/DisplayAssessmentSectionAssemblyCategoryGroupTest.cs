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

namespace Ringtoets.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class DisplayAssessmentSectionAssemblyCategoryGroupTest : EnumWithDisplayNameTestFixture<DisplayAssessmentSectionAssemblyCategoryGroup>
    {
        protected override IDictionary<DisplayAssessmentSectionAssemblyCategoryGroup, byte> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<DisplayAssessmentSectionAssemblyCategoryGroup, byte>
                {
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.APlus, 1
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.A, 2
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.B, 3
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.C, 4
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.D, 5
                    }
                };
            }
        }

        protected override IDictionary<DisplayAssessmentSectionAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<DisplayAssessmentSectionAssemblyCategoryGroup, string>
                {
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.APlus, "A+"
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.A, "A"
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.B, "B"
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.C, "C"
                    },
                    {
                        DisplayAssessmentSectionAssemblyCategoryGroup.D, "D"
                    }
                };
            }
        }
    }
}