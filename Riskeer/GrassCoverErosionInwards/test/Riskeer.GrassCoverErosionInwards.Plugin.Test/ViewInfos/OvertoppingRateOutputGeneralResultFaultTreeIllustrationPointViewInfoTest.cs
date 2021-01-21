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

using Core.Common.Controls.Views;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointViewInfoTest :
        GrassCoverErosionInwardsOutputViewInfoTestBase<OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView, OvertoppingRateOutputContext>
    {
        protected override string ViewName => "Overslagdebiet";

        [Test]
        public void AdditionalDataCheck_CalculationWithoutOvertoppingRateOutput_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.1),
                                                            new TestDikeHeightOutput(0.2),
                                                            null)
            };
            OvertoppingRateOutputContext context = GetContext(calculation);

            // Call
            bool additionalDataCheck = Info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        protected override OvertoppingRateOutputContext GetContext(GrassCoverErosionInwardsCalculation calculation)
        {
            return new OvertoppingRateOutputContext(calculation);
        }

        [TestFixture]
        public class ShouldCloseOvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView : ShouldCloseGrassCoverErosionInwardsOutputViewTester
        {
            protected override IView GetView(ICalculation data)
            {
                return new OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView(
                    (GrassCoverErosionInwardsCalculation) data,
                    () => new TestGeneralResultFaultTreeIllustrationPoint());
            }
        }
    }
}