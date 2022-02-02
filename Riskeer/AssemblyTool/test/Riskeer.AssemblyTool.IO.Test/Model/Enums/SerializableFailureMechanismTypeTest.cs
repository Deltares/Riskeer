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
    public class SerializableFailureMechanismTypeTest : EnumWithXmlEnumNameTestFixture<SerializableFailureMechanismType>
    {
        protected override IDictionary<SerializableFailureMechanismType, int> ExpectedValueForEnumValues =>
            new Dictionary<SerializableFailureMechanismType, int>
            {
                {
                    SerializableFailureMechanismType.STBI, 1
                },
                {
                    SerializableFailureMechanismType.STPH, 2
                },
                {
                    SerializableFailureMechanismType.STMI, 3
                },
                {
                    SerializableFailureMechanismType.AGK, 4
                },
                {
                    SerializableFailureMechanismType.AWO, 5
                },
                {
                    SerializableFailureMechanismType.GEBU, 6
                },
                {
                    SerializableFailureMechanismType.GABU, 7
                },
                {
                    SerializableFailureMechanismType.GEKB, 8
                },
                {
                    SerializableFailureMechanismType.GABI, 9
                },
                {
                    SerializableFailureMechanismType.ZST, 10
                },
                {
                    SerializableFailureMechanismType.DA, 11
                },
                {
                    SerializableFailureMechanismType.HTKW, 12
                },
                {
                    SerializableFailureMechanismType.BSKW, 13
                },
                {
                    SerializableFailureMechanismType.PKW, 14
                },
                {
                    SerializableFailureMechanismType.STKWp, 15
                }
            };

        protected override IDictionary<SerializableFailureMechanismType, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<SerializableFailureMechanismType, string>
            {
                {
                    SerializableFailureMechanismType.STBI, "STBI"
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
                }
            };
    }
}