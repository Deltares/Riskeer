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
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Plugin.Test
{
    [TestFixture]
    public class RiskeerProjectFactoryTest
    {
        [Test]
        public void CreateNewProject_OnCreateNewProjectFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();

            // Call
            void Call() => projectFactory.CreateNewProject(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("onCreateNewProjectFunc", exception.ParamName);
        }

        [Test]
        public void CreateNewProject_WithOnCreateNewProjectFuncReturnAssessmentSection_ReturnsNewRiskeerProject()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            IProject project = projectFactory.CreateNewProject(() => assessmentSection);

            // Assert
            Assert.IsInstanceOf<RiskeerProject>(project);
            var riskeerProject = (RiskeerProject) project;
            CollectionAssert.AreEqual(new[]
            {
                assessmentSection
            }, riskeerProject.AssessmentSections);
        }

        [Test]
        public void CreateNewProject_WithOnCreateNewProjectFuncReturnNull_ReturnsNull()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();

            // Call
            IProject project = projectFactory.CreateNewProject(() => null);

            // Assert
            Assert.IsNull(project);
        }

        [Test]
        public void CreateNewProject_WithOnCreateNewProjectFuncThrowsException_ThrowsException()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();
            const string expectedMessage = "Exception message test";

            // Call
            void Call() => projectFactory.CreateNewProject(() => throw new Exception(expectedMessage));

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void CreateNewProject_WithOnCreateNewProjectFuncThrowsCriticalFileReadException_ThrowsProjectFactoryException()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();
            const string expectedMessage = "Exception message test";

            // Call
            void Call() => projectFactory.CreateNewProject(() => throw new CriticalFileReadException(expectedMessage));

            // Assert
            var exception = Assert.Throws<ProjectFactoryException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<Exception>(exception.InnerException);
        }

        [Test]
        public void CreateNewProject_WithOnCreateNewProjectFuncThrowsCriticalFileValidationException_ThrowsProjectFactoryException()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();
            const string expectedMessage = "Exception message test";

            // Call
            void Call() => projectFactory.CreateNewProject(() => throw new CriticalFileValidationException(expectedMessage));

            // Assert
            var exception = Assert.Throws<ProjectFactoryException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<Exception>(exception.InnerException);
        }
    }
}