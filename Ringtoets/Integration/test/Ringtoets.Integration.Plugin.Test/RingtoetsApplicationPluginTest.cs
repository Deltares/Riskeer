// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var ringtoetsApplicationPlugin = new RingtoetsApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(ringtoetsApplicationPlugin);
        }

        [Test]
        public void GetDataItemInfos_ReturnsExpectedDataItemDefinitions()
        {
            // setup
            var plugin = new RingtoetsApplicationPlugin();

            // call
            var dataItemDefinitions = plugin.GetDataItemInfos().ToArray();

            // assert
            Assert.AreEqual(1, dataItemDefinitions.Length);

            DataItemInfo assessmentSectionDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(AssessmentSection));
            Assert.AreEqual("Traject", assessmentSectionDataItemDefinition.Name);
            Assert.AreEqual("Algemeen", assessmentSectionDataItemDefinition.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AssessmentSectionFolderIcon, assessmentSectionDataItemDefinition.Image);
            Assert.IsNull(assessmentSectionDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<AssessmentSection>(assessmentSectionDataItemDefinition.CreateData(new Project()));
        }

        [Test]
        public void WhenAddingAssessmentSection_GivenProjectHasAssessmentSection_ThenAddedAssessmentSectionHasUniqueName()
        {
            // Setup
            var project = new Project();

            var plugin = new RingtoetsApplicationPlugin();
            AddAssessmentSectionToProject(project, plugin);

            // Call
            AddAssessmentSectionToProject(project, plugin);

            // Assert
            CollectionAssert.AllItemsAreUnique(project.Items.Cast<IAssessmentSection>().Select(section => section.Name));
        }

        private void AddAssessmentSectionToProject(Project project, RingtoetsApplicationPlugin plugin)
        {
            var itemToAdd = plugin.GetDataItemInfos()
                                  .First(di => di.ValueType == typeof(AssessmentSection))
                                  .CreateData(project);

            project.Items.Add(itemToAdd);
        }
    }
}