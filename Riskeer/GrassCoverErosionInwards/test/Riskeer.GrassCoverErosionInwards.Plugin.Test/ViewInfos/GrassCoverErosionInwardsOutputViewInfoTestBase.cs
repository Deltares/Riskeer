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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    /// <summary>
    /// Base class for grass cover erosion inwards output view into tests.
    /// </summary>
    /// <typeparam name="TView">The type of view involved.</typeparam>
    /// <typeparam name="TOutputContext">The type of output context involved.</typeparam>
    [Apartment(ApartmentState.STA)]
    public abstract class GrassCoverErosionInwardsOutputViewInfoTestBase<TView, TOutputContext> : ShouldCloseViewWithCalculationDataTester
        where TView : IView
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        protected abstract string ViewName { get; }

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(TView));
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
            Assert.AreEqual(typeof(TOutputContext), info.DataType);
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculation), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(Resources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual(ViewName, viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            TOutputContext context = GetContext(calculation);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
        }

        [Test]
        public void CreateInstance_Always_CreatesInstanceOfViewType()
        {
            // Call
            IView view = info.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<TView>(view);
        }

        /// <summary>
        /// Returns an output context object.
        /// </summary>
        /// <param name="calculation">The calculation that must be set as wrapped data of the context.</param>
        /// <returns>An output context object.</returns>
        protected abstract TOutputContext GetContext(GrassCoverErosionInwardsCalculation calculation);

        protected override bool ShouldCloseMethod(IView view, object o)
        {
            return info.CloseForData(view, o);
        }

        protected override ICalculation GetCalculation()
        {
            return new GrassCoverErosionInwardsCalculation();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new GrassCoverErosionInwardsCalculationContext(new GrassCoverErosionInwardsCalculation(),
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