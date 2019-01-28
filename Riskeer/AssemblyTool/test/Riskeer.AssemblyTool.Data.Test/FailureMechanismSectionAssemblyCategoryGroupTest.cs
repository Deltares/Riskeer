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

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCategoryGroupTest : EnumValuesTestFixture<FailureMechanismSectionAssemblyCategoryGroup, int>
    {
        protected override IDictionary<FailureMechanismSectionAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<FailureMechanismSectionAssemblyCategoryGroup, int>
                {
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.None, 1
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.Iv, 3
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.IIv, 4
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.IIIv, 5
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.IVv, 6
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.Vv, 7
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.VIv, 8
                    },
                    {
                        FailureMechanismSectionAssemblyCategoryGroup.VIIv, 9
                    }
                };
            }
        }
    }
}