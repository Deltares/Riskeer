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
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableFailureMechanismTypeTest : EnumWithXmlEnumNameTestFixture<SerializableFailureMechanismType>
    {
        protected override IDictionary<SerializableFailureMechanismType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismType, int>
                {
                    {
                        SerializableFailureMechanismType.STBI, 1
                    },
                    {
                        SerializableFailureMechanismType.STBU, 2
                    },
                    {
                        SerializableFailureMechanismType.STPH, 3
                    },
                    {
                        SerializableFailureMechanismType.STMI, 4
                    },
                    {
                        SerializableFailureMechanismType.AGK, 5
                    },
                    {
                        SerializableFailureMechanismType.AWO, 6
                    },
                    {
                        SerializableFailureMechanismType.GEBU, 7
                    },
                    {
                        SerializableFailureMechanismType.GABU, 8
                    },
                    {
                        SerializableFailureMechanismType.GEKB, 9
                    },
                    {
                        SerializableFailureMechanismType.GABI, 10
                    },
                    {
                        SerializableFailureMechanismType.ZST, 11
                    },
                    {
                        SerializableFailureMechanismType.DA, 12
                    },
                    {
                        SerializableFailureMechanismType.HTKW, 13
                    },
                    {
                        SerializableFailureMechanismType.BSKW, 14
                    },
                    {
                        SerializableFailureMechanismType.PKW, 15
                    },
                    {
                        SerializableFailureMechanismType.STKWp, 16
                    },
                    {
                        SerializableFailureMechanismType.STKWl, 17
                    },
                    {
                        SerializableFailureMechanismType.INN, 18
                    }
                };
            }
        }

        protected override IDictionary<SerializableFailureMechanismType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismType, string>
                {
                    {
                        SerializableFailureMechanismType.STBI, "STBI"
                    },
                    {
                        SerializableFailureMechanismType.STBU, "STBU"
                    },
                    {
                        SerializableFailureMechanismType.STPH, "STPH"
                    },
                    {
                        SerializableFailureMechanismType.STMI, "STMI"
                    },
                    {
                        SerializableFailureMechanismType.AGK, "AGK"
                    },
                    {
                        SerializableFailureMechanismType.AWO, "AWO"
                    },
                    {
                        SerializableFailureMechanismType.GEBU, "GEBU"
                    },
                    {
                        SerializableFailureMechanismType.GABU, "GABU"
                    },
                    {
                        SerializableFailureMechanismType.GEKB, "GEKB"
                    },
                    {
                        SerializableFailureMechanismType.GABI, "GABI"
                    },
                    {
                        SerializableFailureMechanismType.ZST, "ZST"
                    },
                    {
                        SerializableFailureMechanismType.DA, "DA"
                    },
                    {
                        SerializableFailureMechanismType.HTKW, "HTKW"
                    },
                    {
                        SerializableFailureMechanismType.BSKW, "BSKW"
                    },
                    {
                        SerializableFailureMechanismType.PKW, "PKW"
                    },
                    {
                        SerializableFailureMechanismType.STKWp, "STKWp"
                    },
                    {
                        SerializableFailureMechanismType.STKWl, "STKWl"
                    },
                    {
                        SerializableFailureMechanismType.INN, "INN"
                    }
                };
            }
        }
    }
}