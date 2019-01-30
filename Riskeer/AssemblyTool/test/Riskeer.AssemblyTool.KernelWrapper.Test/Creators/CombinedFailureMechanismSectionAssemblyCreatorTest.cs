// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void Create_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedFailureMechanismSectionAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_WithResult_ReturnCombinedFailureMechanismSectionAssemblies()
        {
            // Setup
            var random = new Random(21);

            var sections = new[]
            {
                new Tuple<double, double>(0, 2),
                new Tuple<double, double>(2, 5),
                new Tuple<double, double>(5, random.NextDouble(5, 6))
            };

            var failureMechanismResults = new[]
            {
                new FailureMechanismSectionList(string.Empty, new[]
                {
                    CreateCategory(sections[0], random),
                    CreateCategory(sections[1], random),
                    CreateCategory(sections[2], random)
                }),
                new FailureMechanismSectionList(string.Empty, new[]
                {
                    CreateCategory(sections[0], random),
                    CreateCategory(sections[1], random),
                    CreateCategory(sections[2], random)
                })
            };

            FmSectionWithDirectCategory[] combinedResults =
            {
                CreateCategory(sections[0], random),
                CreateCategory(sections[1], random),
                CreateCategory(sections[2], random)
            };

            var assembly = new AssemblyResult(failureMechanismResults, combinedResults);

            // Call
            IEnumerable<CombinedFailureMechanismSectionAssembly> results = CombinedFailureMechanismSectionAssemblyCreator.Create(assembly);

            // Assert
            CombinedFailureMechanismSectionAssemblyAssert.AssertAssembly(assembly, results);
        }

        private static FmSectionWithDirectCategory CreateCategory(Tuple<double, double> section, Random random)
        {
            return new FmSectionWithDirectCategory(section.Item1, section.Item2, random.NextEnumValue<EFmSectionCategory>());
        }
    }
}