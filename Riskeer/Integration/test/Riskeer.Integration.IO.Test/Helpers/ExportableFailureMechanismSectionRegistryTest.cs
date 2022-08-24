// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class ExportableFailureMechanismSectionRegistryTest
    {
        [Test]
        public void Register_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            void Call() => registry.Register(null, ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void Register_ExportableSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            void Call() => registry.Register(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("exportableSection", paramName);
        }
        
        [Test]
        public void Contains_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            void Call() => registry.Contains(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void Contains_WithRegisteredSection_ReturnsTrue()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            registry.Register(section, ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection());

            // Call
            bool result = registry.Contains(section);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_WithUnregisteredSection_ReturnsFalse()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();
            registry.Register(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection());

            // Call
            bool result = registry.Contains(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_ExportableModelRegistryEmpty_ReturnsFalse()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            bool result = registry.Contains(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            void Call() => registry.Get(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void GivenRegistryWithSectionAdded_WhenGetWithSameSection_ThenSameExportableFailureMechanismSectionReturned()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            ExportableFailureMechanismSection exportableSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            registry.Register(section, exportableSection);

            // Call
            ExportableFailureMechanismSection result = registry.Get(section);

            // Assert
            Assert.AreSame(exportableSection, result);
        }

        [Test]
        public void GivenEmptyRegistry_WhenGettingExportableFailureMechanismSection_ThenInvalidOperationExceptionThrown()
        {
            // Setup
            var registry = new ExportableFailureMechanismSectionRegistry();

            // Call
            void Call() => registry.Get(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Assert
            Assert.Throws<InvalidOperationException>(Call);
        }
    }
}