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

using System.Threading;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ProbabilisticFaultTreePipingSectionSpecificOutputViewInfoTest : ProbabilisticPipingOutputViewInfoTestBase<
        ProbabilisticFaultTreePipingSectionSpecificOutputView, ProbabilisticPipingSectionSpecificOutputContext, TopLevelFaultTreeIllustrationPoint>
    {
        protected override string ViewName => "Sterkte berekening vak";

        protected override IView GetView(ICalculation data)
        {
            return new ProbabilisticFaultTreePipingSectionSpecificOutputView(
                (ProbabilisticPipingCalculationScenario) data,
                () => new TestGeneralResultFaultTreeIllustrationPoint());
        }

        protected override ProbabilisticPipingSectionSpecificOutputContext GetContext(
            ProbabilisticPipingCalculationScenario calculationScenario, IAssessmentSection assessmentSection)
        {
            return new ProbabilisticPipingSectionSpecificOutputContext(calculationScenario);
        }

        protected override ProbabilisticPipingOutput GetOutputWithCorrectIllustrationPoints(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            return new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(generalResult),
                                                 PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(generalResult));
        }

        protected override ProbabilisticPipingOutput GetOutputWithIncorrectIllustrationPoints()
        {
            return new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput(),
                                                 PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput());
        }
    }
}