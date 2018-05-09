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
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
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
            var failureMechanism1 = new FailureMechanism(random.NextDouble(1, 2), random.NextDouble());
            var failureMechanism2 = new FailureMechanism(random.NextDouble(1, 2), random.NextDouble());

            var sections = new[]
            {
                new Tuple<double, double>(0, 2),
                new Tuple<double, double>(2, 5),
                new Tuple<double, double>(5, random.NextDouble(5, 6))
            };

            var failureMechanismResults = new[]
            {
                new FailureMechanismSectionList(failureMechanism1, new []
                {
                    new FmSectionWithDirectCategory(sections[0].Item1, sections[0].Item2, random.NextEnumValue<EFmSectionCategory>()),
                    new FmSectionWithDirectCategory(sections[1].Item1, sections[1].Item2, random.NextEnumValue<EFmSectionCategory>()),
                    new FmSectionWithDirectCategory(sections[2].Item1, sections[2].Item2, random.NextEnumValue<EFmSectionCategory>()),
                }),
                new FailureMechanismSectionList(failureMechanism2, new []
                {
                    new FmSectionWithDirectCategory(sections[0].Item1, sections[0].Item2, random.NextEnumValue<EFmSectionCategory>()),
                    new FmSectionWithDirectCategory(sections[1].Item1, sections[1].Item2, random.NextEnumValue<EFmSectionCategory>()),
                    new FmSectionWithDirectCategory(sections[2].Item1, sections[2].Item2, random.NextEnumValue<EFmSectionCategory>()),
                })
            };

            var combinedResults = new[]
            {
                new FmSectionWithDirectCategory(sections[0].Item1, sections[0].Item2, random.NextEnumValue<EFmSectionCategory>()),
                new FmSectionWithDirectCategory(sections[1].Item1, sections[1].Item2, random.NextEnumValue<EFmSectionCategory>()),
                new FmSectionWithDirectCategory(sections[2].Item1, sections[2].Item2, random.NextEnumValue<EFmSectionCategory>()),
            };

            var assembly = new AssemblyResult(failureMechanismResults, combinedResults);

            // Call
            CombinedFailureMechanismSectionAssembly[] results = CombinedFailureMechanismSectionAssemblyCreator.Create(assembly).ToArray();

            // Assert
            Assert.AreEqual(3, results.Length);
            for (var i = 0; i < results.Length; i++)
            {
                Assert.AreEqual(sections[i].Item1, results[i].Section.SectionStart);
                Assert.AreEqual(sections[i].Item2, results[i].Section.SectionEnd);
                Assert.AreEqual(GetResultGroup(combinedResults[i].Category), results[i].Section.CategoryGroup);
                Assert.AreEqual(failureMechanismResults.Length, results[i].FailureMechanismResults.Count());

                for (var j = 0; j < failureMechanismResults.Length; j++)
                {
                    FailureMechanismSectionAssemblyCategoryGroup expectedGroup = GetResultGroup(((FmSectionWithDirectCategory) failureMechanismResults[j].Results[i]).Category);
                    Assert.AreEqual(expectedGroup, results[i].FailureMechanismResults.ElementAt(j));
                }
            }
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetResultGroup(EFmSectionCategory combinedResult)
        {
            switch (combinedResult)
            {
                case EFmSectionCategory.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case EFmSectionCategory.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case EFmSectionCategory.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case EFmSectionCategory.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case EFmSectionCategory.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case EFmSectionCategory.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case EFmSectionCategory.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case EFmSectionCategory.Gr:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                case EFmSectionCategory.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}