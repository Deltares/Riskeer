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

namespace Ringtoets.Common.Primitives.Test
{
    [TestFixture]
    public class TailorMadeAssessmentCategoryGroupResultTypeTest : EnumWithResourcesDisplayNameTestFixture<TailorMadeAssessmentCategoryGroupResultType>
    {
        protected override IDictionary<TailorMadeAssessmentCategoryGroupResultType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<TailorMadeAssessmentCategoryGroupResultType, int>
                {
                    {
                        TailorMadeAssessmentCategoryGroupResultType.None, 1
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.Iv, 2
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IIv, 3
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IIIv, 4
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IVv, 5
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.Vv, 6
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.VIv, 7
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.VIIv, 8
                    }
                };
            }
        }

        protected override IDictionary<TailorMadeAssessmentCategoryGroupResultType, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<TailorMadeAssessmentCategoryGroupResultType, string>
                {
                    {
                        TailorMadeAssessmentCategoryGroupResultType.None, "<selecteer>"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.Iv, "Iv (FV)"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IIv, "IIv"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IIIv, "IIIv"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.IVv, "IVv"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.Vv, "Vv"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.VIv, "VIv"
                    },
                    {
                        TailorMadeAssessmentCategoryGroupResultType.VIIv, "VIIv (NGO)"
                    }
                };
            }
        }
    }
}