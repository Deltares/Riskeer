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
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismTypeTest : EnumValuesTestFixture<ExportableFailureMechanismType, int>
    {
        protected override IDictionary<ExportableFailureMechanismType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableFailureMechanismType, int>
                {
                    {
                        ExportableFailureMechanismType.STBI, 1
                    },
                    {
                        ExportableFailureMechanismType.STBU, 2
                    },
                    {
                        ExportableFailureMechanismType.STPH, 3
                    },
                    {
                        ExportableFailureMechanismType.STMI, 4
                    },
                    {
                        ExportableFailureMechanismType.AGK, 5
                    },
                    {
                        ExportableFailureMechanismType.AWO, 6
                    },
                    {
                        ExportableFailureMechanismType.GEBU, 7
                    },
                    {
                        ExportableFailureMechanismType.GABU, 8
                    },
                    {
                        ExportableFailureMechanismType.GEKB, 9
                    },
                    {
                        ExportableFailureMechanismType.GABI, 10
                    },
                    {
                        ExportableFailureMechanismType.ZST, 11
                    },
                    {
                        ExportableFailureMechanismType.DA, 12
                    },
                    {
                        ExportableFailureMechanismType.HTKW, 13
                    },
                    {
                        ExportableFailureMechanismType.BSKW, 14
                    },
                    {
                        ExportableFailureMechanismType.PKW, 15
                    },
                    {
                        ExportableFailureMechanismType.STKWp, 16
                    },
                    {
                        ExportableFailureMechanismType.STKWl, 17
                    },
                    {
                        ExportableFailureMechanismType.INN, 18
                    }
                };
            }
        }
    }
}