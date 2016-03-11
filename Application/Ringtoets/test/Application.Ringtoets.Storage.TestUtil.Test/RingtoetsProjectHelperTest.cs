﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.TestUtil.Test
{
    [TestFixture]
    public class RingtoetsProjectHelperTest
    {
        [Test]
        public void RingtoetsProjectHelper_Always_ReturnsFullProject()
        {
            // Setup
            string expectedProjectName = "tempProjectFile";
            string expectedDescription = "description";
            string expectedDikeAssessmentSectionName = "dikeAssessmentSection";
            string expectedDuneAssessmentSectionName = "duneAssessmentSection";

            // Call
            Project project = RingtoetsProjectHelper.GetFullTestProject();

            // Assert
            Assert.AreEqual(expectedProjectName, project.Name);
            Assert.AreEqual(expectedDescription, project.Description);

            DikeAssessmentSection dikeAssessmentSection = project.Items.OfType<DikeAssessmentSection>().FirstOrDefault();
            Assert.IsInstanceOf<DikeAssessmentSection>(dikeAssessmentSection);
            Assert.AreEqual(expectedDikeAssessmentSectionName, dikeAssessmentSection.Name);

            DuneAssessmentSection duneAssessmentSection = project.Items.OfType<DuneAssessmentSection>().FirstOrDefault();
            Assert.IsInstanceOf<DuneAssessmentSection>(duneAssessmentSection);
            Assert.AreEqual(expectedDuneAssessmentSectionName, duneAssessmentSection.Name);
        }
    }
}