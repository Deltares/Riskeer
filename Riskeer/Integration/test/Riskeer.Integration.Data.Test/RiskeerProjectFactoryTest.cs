﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.Integration.Data.Test
{
    [TestFixture]
    public class RiskeerProjectFactoryTest
    {
        [Test]
        public void CreateNewProject_ReturnsNewRiskeerProject()
        {
            // Setup
            var projectFactory = new RiskeerProjectFactory();

            // Call
            IProject result = projectFactory.CreateNewProject();

            // Assert
            var riskeerProject = result as RiskeerProject;
            Assert.IsNotNull(riskeerProject);
            Assert.AreEqual(1, riskeerProject.AssessmentSections.Count);
        }
    }
}