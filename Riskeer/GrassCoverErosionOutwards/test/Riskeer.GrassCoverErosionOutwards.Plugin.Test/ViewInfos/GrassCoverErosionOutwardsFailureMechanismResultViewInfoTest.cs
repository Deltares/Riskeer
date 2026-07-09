// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using NSubstitute;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects.RegistrationState;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsResultViewInfoTest
    {
        private GrassCoverErosionOutwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>));
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsFailureMechanismSectionResultContext), info.DataType);
            Assert.AreEqual(typeof(IObservableEnumerable<NonAdoptableFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailureMechanismSectionResultContext(
                failureMechanism.SectionResults, failureMechanism, assessmentSection);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(Array.Empty<IFailureMechanism>());

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var otherFailureMechanism = Substitute.For<IFailureMechanism>();

            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                otherFailureMechanism
            });

            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.GetFailureMechanisms().Returns(new IFailureMechanism[]
            {
                failureMechanism
            });

            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, new GrassCoverErosionOutwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var context = new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            using (var view = new NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual)))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailureMechanismSectionResultContext(
                failureMechanism.SectionResults, failureMechanism, assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<NonAdoptableFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>>(view);
        }
    }
}