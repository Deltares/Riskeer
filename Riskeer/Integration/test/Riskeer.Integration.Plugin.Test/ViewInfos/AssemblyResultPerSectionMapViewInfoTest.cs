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
using System.Drawing;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.Plugin.Properties;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class AssemblyResultPerSectionMapViewInfoTest
    {
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(AssemblyResultPerSectionMapView));
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
            Assert.AreEqual(typeof(AssemblyResultPerSectionMapContext), info.DataType);
            Assert.AreEqual(typeof(AssessmentSection), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Assemblagekaart", viewName);
        }

        [Test]
        public void Image_Always_ReturnsExpectedIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.AssemblyResultPerSectionMap, image);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var view = new AssemblyResultPerSectionMapView(assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            AssessmentSection assessmentSection1 = CreateAssessmentSection();
            AssessmentSection assessmentSection2 = CreateAssessmentSection();

            using (var view = new AssemblyResultPerSectionMapView(assessmentSection1))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection2);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var context = new AssemblyResultPerSectionMapContext(assessmentSection);

            // Call
            var view = (AssemblyResultPerSectionMapView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(assessmentSection, view.AssessmentSection);
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            return new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
        }
    }
}