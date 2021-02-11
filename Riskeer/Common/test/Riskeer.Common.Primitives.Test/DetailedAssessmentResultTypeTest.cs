﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Common.Primitives.Test
{
    [TestFixture]
    public class DetailedAssessmentResultTypeTest : EnumWithResourcesDisplayNameTestFixture<DetailedAssessmentResultType>
    {
        protected override IDictionary<DetailedAssessmentResultType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<DetailedAssessmentResultType, int>
                {
                    {
                        DetailedAssessmentResultType.None, 1
                    },
                    {
                        DetailedAssessmentResultType.Sufficient, 2
                    },
                    {
                        DetailedAssessmentResultType.Insufficient, 3
                    },
                    {
                        DetailedAssessmentResultType.NotAssessed, 4
                    }
                };
            }
        }

        protected override IDictionary<DetailedAssessmentResultType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<DetailedAssessmentResultType, string>
                {
                    {
                        DetailedAssessmentResultType.None, "<selecteer>"
                    },
                    {
                        DetailedAssessmentResultType.Sufficient, "V"
                    },
                    {
                        DetailedAssessmentResultType.Insufficient, "VN"
                    },
                    {
                        DetailedAssessmentResultType.NotAssessed, "NGO"
                    }
                };
            }
        }
    }
}