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

using System.Linq;
using System.Threading;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    /// <summary>
    /// Base class for grass cover erosion inwards output view into tests.
    /// </summary>
    /// <typeparam name="TView">The type of view.</typeparam>
    /// <typeparam name="TOutputContext">The type of output context.</typeparam>
    [Apartment(ApartmentState.STA)]
    public abstract class GrassCoverErosionInwardsOutputViewInfoTestBase<TView, TOutputContext>
        where TView : IView
    {
        private GrassCoverErosionInwardsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            Info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(TView));
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
            Assert.AreEqual(typeof(TOutputContext), Info.DataType);
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculation), Info.ViewDataType);
            TestHelper.AssertImagesAreEqual(Resources.GeneralOutputIcon, Info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = Info.GetViewName(null, null);

            // Assert
            Assert.AreEqual(ViewName, viewName);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            TOutputContext context = GetContext(calculation);

            // Call
            object viewData = Info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithoutOutput_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            TOutputContext context = GetContext(calculation);

            // Call
            bool additionalDataCheck = Info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithOutputWithoutIllustrationPoints_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            TOutputContext context = GetContext(calculation);

            // Call
            bool additionalDataCheck = Info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithOutputAndIllustrationPoints_ReturnsTrue()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            TOutputContext context = GetContext(calculation);

            // Call
            bool additionalDataCheck = Info.AdditionalDataCheck(context);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void CreateInstance_WithContext_CreatesInstanceOfViewType()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            TOutputContext context = GetContext(calculation);

            // Call
            IView view = Info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<TView>(view);
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        protected abstract string ViewName { get; }

        /// <summary>
        /// Gets the <see cref="ViewInfo"/>.
        /// </summary>
        protected ViewInfo Info { get; private set; }

        /// <summary>
        /// Returns an output context object.
        /// </summary>
        /// <param name="calculation">The calculation that must be set as wrapped data of the context.</param>
        /// <returns>An output context object.</returns>
        protected abstract TOutputContext GetContext(GrassCoverErosionInwardsCalculation calculation);

        [TestFixture]
        public abstract class ShouldCloseGrassCoverErosionInwardsOutputViewTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new GrassCoverErosionInwardsPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(TView))
                                 .CloseForData(view, o);
                }
            }

            protected override ICalculation GetCalculation()
            {
                return new GrassCoverErosionInwardsCalculation();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new GrassCoverErosionInwardsCalculationScenarioContext(new GrassCoverErosionInwardsCalculationScenario(),
                                                                              new CalculationGroup(),
                                                                              new GrassCoverErosionInwardsFailureMechanism(),
                                                                              new AssessmentSectionStub());
            }

            protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
            {
                return new GrassCoverErosionInwardsCalculationGroupContext(
                    new CalculationGroup
                    {
                        Children =
                        {
                            new GrassCoverErosionInwardsCalculation()
                        }
                    },
                    null,
                    new GrassCoverErosionInwardsFailureMechanism(),
                    new AssessmentSectionStub());
            }

            protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
            {
                return new GrassCoverErosionInwardsFailureMechanismContext(
                    new GrassCoverErosionInwardsFailureMechanism
                    {
                        CalculationsGroup =
                        {
                            Children =
                            {
                                new GrassCoverErosionInwardsCalculation()
                            }
                        }
                    },
                    new AssessmentSectionStub());
            }
        }
    }
}