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
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableAssessmentLevelTest : EnumWithXmlEnumNameTestFixture<SerializableAssessmentLevel>
    {
        protected override IDictionary<SerializableAssessmentLevel, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentLevel, int>
                {
                    {
                        SerializableAssessmentLevel.SimpleAssessment, 1
                    },
                    {
                        SerializableAssessmentLevel.DetailedAssessment, 2
                    },
                    {
                        SerializableAssessmentLevel.TailorMadeAssessment, 3
                    },
                    {
                        SerializableAssessmentLevel.CombinedAssessment, 4
                    },
                    {
                        SerializableAssessmentLevel.CombinedSectionAssessment, 5
                    },
                    {
                        SerializableAssessmentLevel.CombinedSectionFailureMechanismAssessment, 6
                    }
                };
            }
        }

        protected override IDictionary<SerializableAssessmentLevel, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableAssessmentLevel, string>
                {
                    {
                        SerializableAssessmentLevel.SimpleAssessment, "EENVDGETS"
                    },
                    {
                        SerializableAssessmentLevel.DetailedAssessment, "GEDTETS"
                    },
                    {
                        SerializableAssessmentLevel.TailorMadeAssessment, "TOETSOPMT"
                    },
                    {
                        SerializableAssessmentLevel.CombinedAssessment, "GECBNTR"
                    },
                    {
                        SerializableAssessmentLevel.CombinedSectionAssessment, "GECBNTRDV"
                    },
                    {
                        SerializableAssessmentLevel.CombinedSectionFailureMechanismAssessment, "GECBNTRDVTS"
                    }
                };
            }
        }
    }
}