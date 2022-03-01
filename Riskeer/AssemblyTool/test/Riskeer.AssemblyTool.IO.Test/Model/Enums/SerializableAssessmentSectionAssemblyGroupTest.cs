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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableAssessmentSectionAssemblyGroupTest : EnumWithXmlEnumNameTestFixture<SerializableAssessmentSectionAssemblyGroup>
    {
        protected override IDictionary<SerializableAssessmentSectionAssemblyGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentSectionAssemblyGroup, int>
                {
                    {
                        SerializableAssessmentSectionAssemblyGroup.APlus, 1
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.A, 2
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.B, 3
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.C, 4
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.D, 5
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.NotAssessed, 6
                    }
                };
            }
        }

        protected override IDictionary<SerializableAssessmentSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentSectionAssemblyGroup, string>
                {
                    {
                        SerializableAssessmentSectionAssemblyGroup.APlus, "A+"
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.A, "A"
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.B, "B"
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.C, "C"
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.D, "D"
                    },
                    {
                        SerializableAssessmentSectionAssemblyGroup.NotAssessed, "NGO"
                    }
                };
            }
        }
    }
}