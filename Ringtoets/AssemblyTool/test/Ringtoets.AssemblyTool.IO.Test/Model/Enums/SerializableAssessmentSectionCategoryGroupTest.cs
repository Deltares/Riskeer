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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableAssessmentSectionCategoryGroupTest : EnumWithXmlEnumNameTestFixture<SerializableAssessmentSectionCategoryGroup>
    {
        protected override IDictionary<SerializableAssessmentSectionCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentSectionCategoryGroup, int>
                {
                    {
                        SerializableAssessmentSectionCategoryGroup.APlus, 1
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.A, 2
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.B, 3
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.C, 4
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.D, 5
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.NotAssessed, 6
                    }
                };
            }
        }

        protected override IDictionary<SerializableAssessmentSectionCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentSectionCategoryGroup, string>
                {
                    {
                        SerializableAssessmentSectionCategoryGroup.APlus, "A+"
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.A, "A"
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.B, "B"
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.C, "C"
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.D, "D"
                    },
                    {
                        SerializableAssessmentSectionCategoryGroup.NotAssessed, "NGO"
                    }
                };
            }
        }
    }
}