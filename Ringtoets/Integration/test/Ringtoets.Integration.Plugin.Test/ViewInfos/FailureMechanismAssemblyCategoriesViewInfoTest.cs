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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoriesViewInfoTest
    {
        private static ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            using (var plugin = new RingtoetsPlugin())
            {
                info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismAssemblyCategoriesView));
            }
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismAssemblyCategoriesContextBase), info.DataType);
            Assert.AreEqual(typeof(IFailureMechanism), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Categoriegrenzen", viewName);
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismAssemblyCategoriesContext = new TestFailureMechanismAssemblyCategoriesContext(failureMechanism,
                                                                                                              assessmentSection,
                                                                                                              () => new Random(39).NextDouble());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var view = (FailureMechanismAssemblyCategoriesView) info.CreateInstance(failureMechanismAssemblyCategoriesContext);

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void Image_Always_ReturnsNormIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.NormsIcon, image);
        }

        [TestFixture]
        public class ShouldCloseFailureMechanismAssemblyCategoriesViewForDataTester : ShouldCloseViewWithFailureMechanismTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return info.CloseForData(view, o);
            }

            protected override IView GetView(IFailureMechanism failureMechanism)
            {
                return new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                  new AssessmentSectionStub(),
                                                                  Enumerable.Empty<FailureMechanismAssemblyCategory>,
                                                                  Enumerable.Empty<FailureMechanismSectionAssemblyCategory>);
            }

            protected override IFailureMechanism GetFailureMechanism()
            {
                return new TestFailureMechanism();
            }
        }

        private class TestFailureMechanismAssemblyCategoriesContext : FailureMechanismAssemblyCategoriesContextBase
        {
            public TestFailureMechanismAssemblyCategoriesContext(IFailureMechanism wrappedData, IAssessmentSection assessmentSection, Func<double> getNFunc)
                : base(wrappedData, assessmentSection, getNFunc) {}

            public override Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> GetFailureMechanismSectionAssemblyCategoriesFunc
            {
                get
                {
                    return Enumerable.Empty<FailureMechanismSectionAssemblyCategory>;
                }
            }
        }
    }
}