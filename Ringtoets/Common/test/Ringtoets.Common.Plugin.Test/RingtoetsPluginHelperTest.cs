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

using System.Linq;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsPluginHelperTest : ShouldCloseViewWithCalculationDataTester
    {
        protected override bool PerformShouldCloseViewWithCalculationDataMethod(IView view, object o)
        {
            using (var plugin = new RingtoetsPlugin())
            {
                return plugin.GetViewInfos()
                             .First(tni => tni.ViewType == typeof(GeneralResultFaultTreeIllustrationPointView))
                             .CloseForData(view, o);
            }
        }

        protected override IView GetView()
        {
            return new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint());
        }

        protected override ICalculation GetCalculation()
        {
            return new StructuresCalculation<HeightStructuresInput>();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new HeightStructuresCalculationContext(
                new StructuresCalculation<HeightStructuresInput>(),
                new HeightStructuresFailureMechanism(),
                new AssessmentSection(AssessmentSectionComposition.Dike));
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new HeightStructuresCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        new StructuresCalculation<HeightStructuresInput>()
                    }
                },
                new HeightStructuresFailureMechanism(),
                new AssessmentSection(AssessmentSectionComposition.Dike));
        }

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new HeightStructuresFailureMechanismContext(
                new HeightStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculation<HeightStructuresInput>()
                        }
                    }
                },
                new AssessmentSection(AssessmentSectionComposition.Dike));
        }
    }
}