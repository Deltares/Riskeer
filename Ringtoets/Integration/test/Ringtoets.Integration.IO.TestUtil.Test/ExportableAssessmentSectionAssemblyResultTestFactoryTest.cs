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

using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil.Test
{
    [TestFixture]
    public class ExportableAssessmentSectionAssemblyResultTestFactoryTest
    {
        [Test]
        public void CreateResult_Always_ReturnsExportableAssessmentSectionAssemblyResult()
        {
            // Call
            ExportableAssessmentSectionAssemblyResult result =
                ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult();

            // Assert
            Assert.AreEqual(ExportableAssemblyMethod.WBI2C1, result.AssemblyMethod);
            Assert.AreEqual(AssessmentSectionAssemblyCategoryGroup.C, result.AssemblyCategory);
        }
    }
}