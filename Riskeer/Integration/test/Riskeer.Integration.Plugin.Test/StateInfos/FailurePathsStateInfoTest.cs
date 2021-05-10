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

using System.Linq;
using Core.Common.Base.Data;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Plugin.Test.StateInfos
{
    [TestFixture]
    public class FailurePathsStateInfoTest
    {
        private RiskeerPlugin plugin;
        private StateInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetStateInfos().First(si => si.Name == "Faalpaden");
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
            Assert.AreEqual("\uE953", info.Symbol);
        }

        [Test]
        public void GetRootData_RiskeerProject_ReturnsExpectedRootData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var project = new RiskeerProject
            {
                AssessmentSections =
                {
                    assessmentSection
                }
            };

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            var rootDataCollection = rootData as object[];
            Assert.IsNotNull(rootDataCollection);
            Assert.AreEqual(8, rootDataCollection.Length);
        }

        [Test]
        public void GetRootData_ProjectMock_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var project = mockRepository.StrictMock<IProject>();

            mockRepository.ReplayAll();

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            Assert.IsNull(rootData);

            mockRepository.VerifyAll();
        }
    }
}