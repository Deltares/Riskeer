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
        public void Constructor_CreateAssessmentSectionFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new RiskeerProjectFactory(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createAssessmentSectionFunc", exception.ParamName);
        }
        [Test]
        public void CreateNewProject_WithCreateAssessmentSectionFuncReturnAssessmentSection_ReturnsNewRiskeerProject()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var projectFactory = new RiskeerProjectFactory(() => assessmentSection);

            // Call
            IProject project = projectFactory.CreateNewProject();

            // Assert
            Assert.IsInstanceOf<RiskeerProject>(project);
            var riskeerProject = (RiskeerProject) project;
            CollectionAssert.AreEqual(new[]
            {
                assessmentSection
            }, riskeerProject.AssessmentSections);
        }

        [Test]
        public void CreateNewProject_WithCreateAssessmentSectionFuncReturnNull_ReturnsNull()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory(() => null);

            // Call
            IProject project = projectFactory.CreateNewProject();

            // Assert
            Assert.IsNull(project);
        }

        [Test]
        public void CreateNewProject_WithCreateAssessmentSectionFuncThrowsException_ThrowsException()
        {
            // Setup
            const string expectedMessage = "Exception message test";
            var projectFactory = new RiskeerProjectFactory(() => throw new Exception(expectedMessage));

            // Call
            void Call() => projectFactory.CreateNewProject();

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void CreateNewProject_WithCreateAssessmentSectionFuncThrowsCriticalFileReadException_ThrowsProjectFactoryException()
        {
            // Setup
            const string expectedMessage = "Exception message test";
            var projectFactory = new RiskeerProjectFactory(() => throw new CriticalFileReadException(expectedMessage));

            // Call
            void Call() => projectFactory.CreateNewProject();

            // Assert
            var exception = Assert.Throws<ProjectFactoryException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<Exception>(exception.InnerException);
        }

        [Test]
        public void CreateNewProject_WithCreateAssessmentSectionFuncThrowsCriticalFileValidationException_ThrowsProjectFactoryException()
        {
            // Setup
            const string expectedMessage = "Exception message test";
            var projectFactory = new RiskeerProjectFactory(() => throw new CriticalFileValidationException(expectedMessage));

            // Call
            void Call() => projectFactory.CreateNewProject();

            // Assert
            var exception = Assert.Throws<ProjectFactoryException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<Exception>(exception.InnerException);
        }
    }
}