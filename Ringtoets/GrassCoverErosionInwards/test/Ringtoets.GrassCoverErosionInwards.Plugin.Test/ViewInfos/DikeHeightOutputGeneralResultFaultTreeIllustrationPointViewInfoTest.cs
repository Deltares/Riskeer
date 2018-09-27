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

using System.Threading;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DikeHeightOutputGeneralResultFaultTreeIllustrationPointViewInfoTest :
        GrassCoverErosionInwardsOutputViewInfoTestBase<DikeHeightOutputGeneralResultFaultTreeIllustrationPointView, DikeHeightOutputContext>
    {
        protected override string ViewName
        {
            get
            {
                return "HBN";
            }
        }

        protected override IView GetView(ICalculation data)
        {
            return new DikeHeightOutputGeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
            {
                Data = data
            };
        }

        protected override DikeHeightOutputContext GetContext(GrassCoverErosionInwardsCalculation calculation)
        {
            return new DikeHeightOutputContext(calculation);
        }
    }
}