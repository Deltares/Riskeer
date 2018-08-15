// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model.DataTypes
{
    [TestFixture]
    public class SerializableAssessmentSectionAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assemblyResult = new SerializableAssessmentSectionAssemblyResult();

            // Assert
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyMethod);
            Assert.AreEqual((SerializableAssessmentSectionCategoryGroup) 0, assemblyResult.CategoryGroup);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSectionAssemblyResult>(
                nameof(SerializableAssessmentSectionAssemblyResult.AssemblyMethod), "assemblagemethode");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSectionAssemblyResult>(
                nameof(SerializableAssessmentSectionAssemblyResult.CategoryGroup), "categorie");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSectionAssemblyResult>(
                nameof(SerializableAssessmentSectionAssemblyResult.Status), "status");
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var category = random.NextEnumValue<SerializableAssessmentSectionCategoryGroup>();
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();

            // Call
            var assemblyResult = new SerializableAssessmentSectionAssemblyResult(assemblyMethod, category);
            // Assert
            Assert.AreEqual(category, assemblyResult.CategoryGroup);
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
        }
    }
}