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
    public class SerializableFailureMechanismSectionTypeTest : EnumWithXmlEnumNameTestFixture<SerializableFailureMechanismSectionType>
    {
        protected override IDictionary<SerializableFailureMechanismSectionType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismSectionType, int>
                {
                    {
                        SerializableFailureMechanismSectionType.FailureMechanism, 1
                    },
                    {
                        SerializableFailureMechanismSectionType.Combined, 2
                    }
                };
            }
        }

        protected override IDictionary<SerializableFailureMechanismSectionType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SerializableFailureMechanismSectionType, string>
                {
                    {
                        SerializableFailureMechanismSectionType.FailureMechanism, "TOETSSSTE"
                    },
                    {
                        SerializableFailureMechanismSectionType.Combined, "GECBNETSSTE"
                    }
                };
            }
        }
    }
}