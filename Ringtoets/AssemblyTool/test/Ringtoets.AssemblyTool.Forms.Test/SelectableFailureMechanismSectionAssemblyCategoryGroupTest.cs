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
using Riskeer.AssemblyTool.Forms;

namespace Ringtoets.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class SelectableFailureMechanismSectionAssemblyCategoryGroupTest : EnumWithResourcesDisplayNameTestFixture<SelectableFailureMechanismSectionAssemblyCategoryGroup>
    {
        protected override IDictionary<SelectableFailureMechanismSectionAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<SelectableFailureMechanismSectionAssemblyCategoryGroup, int>
                {
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.None, 1
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.Iv, 3
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IIv, 4
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IIIv, 5
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IVv, 6
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.Vv, 7
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.VIv, 8
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.VIIv, 9
                    }
                };
            }
        }

        protected override IDictionary<SelectableFailureMechanismSectionAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<SelectableFailureMechanismSectionAssemblyCategoryGroup, string>
                {
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.None, "<selecteer>"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, "NVT"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.Iv, "Iv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IIv, "IIv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IIIv, "IIIv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.IVv, "IVv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.Vv, "Vv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.VIv, "VIv"
                    },
                    {
                        SelectableFailureMechanismSectionAssemblyCategoryGroup.VIIv, "VIIv"
                    }
                };
            }
        }
    }
}