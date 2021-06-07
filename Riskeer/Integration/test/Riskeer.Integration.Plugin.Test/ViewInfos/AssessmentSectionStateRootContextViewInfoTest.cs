// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.Views;
using RiskeerFormsResources = Riskeer.Integration.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class AssessmentSectionStateRootContextViewInfoTest
    {
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(AssessmentSectionReferenceLineView)
                                                      && tni.DataType == typeof(AssessmentSectionStateRootContext));
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
            Assert.AreEqual(typeof(AssessmentSectionStateRootContext), info.DataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Trajectkaart", viewName);
        }

        [Test]
        public void Image_Always_ReturnsExpectedIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.Map, image);
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsAssessmentSectionReferenceLineView()
        {
            // Setup
            var context = new AssessmentSectionStateRootContext(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionReferenceLineView>(view);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var view = new AssessmentSectionReferenceLineView(assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection1 = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            var assessmentSection2 = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var view = new AssessmentSectionReferenceLineView(assessmentSection1);

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection2);

            // Assert
            Assert.IsFalse(closeForData);
        }
    }
}