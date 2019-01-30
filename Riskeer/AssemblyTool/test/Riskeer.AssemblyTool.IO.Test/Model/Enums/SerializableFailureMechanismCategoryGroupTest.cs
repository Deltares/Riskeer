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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableFailureMechanismCategoryGroupTest : EnumWithXmlEnumNameTestFixture<SerializableFailureMechanismCategoryGroup>
    {
        protected override IDictionary<SerializableFailureMechanismCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismCategoryGroup, int>
                {
                    {
                        SerializableFailureMechanismCategoryGroup.NotApplicable, 1
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.It, 2
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IIt, 3
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IIIt, 4
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IVt, 5
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.Vt, 6
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.VIt, 7
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.VIIt, 8
                    }
                };
            }
        }

        protected override IDictionary<SerializableFailureMechanismCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismCategoryGroup, string>
                {
                    {
                        SerializableFailureMechanismCategoryGroup.NotApplicable, "NVT"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.It, "I-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IIt, "II-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IIIt, "III-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.IVt, "IV-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.Vt, "V-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.VIt, "VI-traject"
                    },
                    {
                        SerializableFailureMechanismCategoryGroup.VIIt, "VII-traject"
                    }
                };
            }
        }
    }
}