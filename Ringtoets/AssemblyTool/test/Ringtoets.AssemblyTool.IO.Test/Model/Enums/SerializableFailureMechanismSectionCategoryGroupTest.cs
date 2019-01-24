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
    public class SerializableFailureMechanismSectionCategoryGroupTest : EnumWithXmlEnumNameTestFixture<SerializableFailureMechanismSectionCategoryGroup>
    {
        protected override IDictionary<SerializableFailureMechanismSectionCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismSectionCategoryGroup, int>
                {
                    {
                        SerializableFailureMechanismSectionCategoryGroup.NotApplicable, 1
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.Iv, 2
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IIv, 3
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IIIv, 4
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IVv, 5
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.Vv, 6
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.VIv, 7
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.VIIv, 8
                    }
                };
            }
        }

        protected override IDictionary<SerializableFailureMechanismSectionCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismSectionCategoryGroup, string>
                {
                    {
                        SerializableFailureMechanismSectionCategoryGroup.NotApplicable, "NVT"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.Iv, "I-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IIv, "II-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IIIv, "III-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.IVv, "IV-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.Vv, "V-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.VIv, "VI-vak"
                    },
                    {
                        SerializableFailureMechanismSectionCategoryGroup.VIIv, "VII-vak"
                    }
                };
            }
        }
    }
}