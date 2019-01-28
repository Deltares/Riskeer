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

using System.Linq;
using System.Threading;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class GeneralResultFaultTreeIllustrationPointViewInfoTest
    {
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
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
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralOutputIcon, info.Image);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structuresCalculation = mocks.Stub<IStructuresCalculation>();
            mocks.ReplayAll();

            // Call
            object viewData = info.GetViewData(new SimpleStructuresOutputContext(structuresCalculation, assessmentSection));

            // Assert
            Assert.AreSame(structuresCalculation, viewData);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_Always_CreatesGeneralResultFaultTreeIllustrationPointView()
        {
            // Call
            IView view = info.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<GeneralResultFaultTreeIllustrationPointView>(view);
        }

        private class SimpleStructuresOutputContext : StructuresOutputContext
        {
            public SimpleStructuresOutputContext(IStructuresCalculation wrappedData, IAssessmentSection assessmentSection)
                : base(wrappedData, assessmentSection) {}
        }

        public abstract class ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(GeneralResultFaultTreeIllustrationPointView))
                                 .CloseForData(view, o);
                }
            }

            protected override IView GetView(ICalculation data)
            {
                return new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint())
                {
                    Data = data
                };
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
                return new HeightStructuresCalculationContext(
                    new StructuresCalculation<HeightStructuresInput>(),
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

        [TestFixture]
        public class ShouldCloseClosingViewTester : ShouldCloseGeneralResultFaultTreeIllustrationPointViewForStructuresTester
        {
            protected override ICalculation GetCalculation()
            {
                return new StructuresCalculation<ClosingStructuresInput>();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new ClosingStructuresCalculationContext(
                    new StructuresCalculation<ClosingStructuresInput>(),
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

            protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
            {
                return new ClosingStructuresFailureMechanismContext(
                    new ClosingStructuresFailureMechanism
                    {
                        CalculationsGroup =
                        {
                            Children =
                            {
                                new StructuresCalculation<ClosingStructuresInput>()
                            }
                        }
                    },
                    new AssessmentSection(AssessmentSectionComposition.Dike));
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
                return new StabilityPointStructuresCalculationContext(
                    new StructuresCalculation<StabilityPointStructuresInput>(),
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

            protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
            {
                return new StabilityPointStructuresFailureMechanismContext(
                    new StabilityPointStructuresFailureMechanism
                    {
                        CalculationsGroup =
                        {
                            Children =
                            {
                                new StructuresCalculation<StabilityPointStructuresInput>()
                            }
                        }
                    },
                    new AssessmentSection(AssessmentSectionComposition.Dike));
            }
        }
    }
}