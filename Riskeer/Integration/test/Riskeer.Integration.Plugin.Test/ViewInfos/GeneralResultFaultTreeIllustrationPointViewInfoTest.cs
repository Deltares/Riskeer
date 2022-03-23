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

using System.Linq;
using System.Threading;
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class GeneralResultFaultTreeIllustrationPointViewInfoTest
    {
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GeneralResultFaultTreeIllustrationPointView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StructuresOutputContext), info.DataType);
            Assert.AreEqual(typeof(IStructuresCalculation), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsCalculationOutputDisplayName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedStructuresCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var structuresCalculation = mocks.Stub<IStructuresCalculation>();
            mocks.ReplayAll();

            // Call
            object viewData = info.GetViewData(new StructuresOutputContext(structuresCalculation));

            // Assert
            Assert.AreSame(structuresCalculation, viewData);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_CreatesGeneralResultFaultTreeIllustrationPointView()
        {
            // Setup
            var mocks = new MockRepository();
            var structuresCalculation = mocks.Stub<IStructuresCalculation>();
            mocks.ReplayAll();

            // Call
            IView view = info.CreateInstance(new StructuresOutputContext(structuresCalculation));

            // Assert
            Assert.IsInstanceOf<GeneralResultFaultTreeIllustrationPointView>(view);
            mocks.VerifyAll();
        }

        public abstract class ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new RiskeerPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(GeneralResultFaultTreeIllustrationPointView))
                                 .CloseForData(view, o);
                }
            }

            protected override IView GetView(ICalculation data)
            {
                return new GeneralResultFaultTreeIllustrationPointView(data, () => new TestGeneralResultFaultTreeIllustrationPoint());
            }
        }

        [TestFixture]
        public class ShouldCloseHeightViewTester : ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester
        {
            protected override ICalculation GetCalculation()
            {
                return new StructuresCalculation<HeightStructuresInput>();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new HeightStructuresCalculationScenarioContext(
                    new StructuresCalculationScenario<HeightStructuresInput>(),
                    new CalculationGroup(),
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
                    null,
                    new HeightStructuresFailureMechanism(),
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }

            protected override ICalculatableFailureMechanism GetFailureMechanismWithCalculation()
            {
                return new HeightStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculation<HeightStructuresInput>()
                        }
                    }
                };
            }
        }

        [TestFixture]
        public class ShouldCloseClosingViewTester : ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester
        {
            protected override ICalculation GetCalculation()
            {
                return new StructuresCalculation<ClosingStructuresInput>();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new ClosingStructuresCalculationScenarioContext(
                    new StructuresCalculationScenario<ClosingStructuresInput>(),
                    new CalculationGroup(),
                    new ClosingStructuresFailureMechanism(),
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }

            protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
            {
                return new ClosingStructuresCalculationGroupContext(
                    new CalculationGroup
                    {
                        Children =
                        {
                            new StructuresCalculation<ClosingStructuresInput>()
                        }
                    },
                    null,
                    new ClosingStructuresFailureMechanism(),
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }

            protected override ICalculatableFailureMechanism GetFailureMechanismWithCalculation()
            {
                return
                    new ClosingStructuresFailureMechanism
                    {
                        CalculationsGroup =
                        {
                            Children =
                            {
                                new StructuresCalculation<ClosingStructuresInput>()
                            }
                        }
                    };
            }
        }

        [TestFixture]
        public class ShouldCloseStabilityPointViewTester : ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester
        {
            protected override ICalculation GetCalculation()
            {
                return new StructuresCalculation<StabilityPointStructuresInput>();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new StabilityPointStructuresCalculationScenarioContext(
                    new StructuresCalculationScenario<StabilityPointStructuresInput>(),
                    new CalculationGroup(),
                    new StabilityPointStructuresFailureMechanism(),
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }

            protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
            {
                return new StabilityPointStructuresCalculationGroupContext(
                    new CalculationGroup
                    {
                        Children =
                        {
                            new StructuresCalculation<StabilityPointStructuresInput>()
                        }
                    },
                    null,
                    new StabilityPointStructuresFailureMechanism(),
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }

            protected override ICalculatableFailureMechanism GetFailureMechanismWithCalculation()
            {
                return new StabilityPointStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculation<StabilityPointStructuresInput>()
                        }
                    }
                };
            }
        }
    }
}