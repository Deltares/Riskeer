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
using System.ComponentModel;
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
    public class FailureMechanismSectionListCreatorTest
    {
        [Test]
        public void Create_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionListCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanism_ReturnFailureMechanismSectionLists()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new CombinedAssemblyFailureMechanismInput(random.NextDouble(1, 2), random.NextDouble(), new[]
            {
                new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                new CombinedAssemblyFailureMechanismSection(1, 2, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            });

            // Call
            FailureMechanismSectionList[] failureMechanismSectionLists = FailureMechanismSectionListCreator.Create(new[]
            {
                failureMechanism
            }).ToArray();

            // Assert
            Assert.AreEqual(1, failureMechanismSectionLists.Length);
            FailureMechanismSectionList sectionList = failureMechanismSectionLists[0];
            Assert.AreEqual(failureMechanism.N, sectionList.FailureMechanism.LengthEffectFactor);
            Assert.AreEqual(failureMechanism.FailureMechanismContribution, sectionList.FailureMechanism.FailureProbabilityMarginFactor);

            AssertSections(failureMechanism.Sections.ToArray(), sectionList.Results.ToArray());
        }

        [Test]
        public void Create_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => FailureMechanismSectionListCreator.Create(new[]
            {
                new CombinedAssemblyFailureMechanismInput(random.NextDouble(1, 2), random.NextDouble(), new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, (FailureMechanismSectionAssemblyCategoryGroup) 99)
                })
            });

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EFmSectionCategory.Gr)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EFmSectionCategory.VIIv)]
        public void Create_ValidGroup_ReturnEFmSectionCategory(FailureMechanismSectionAssemblyCategoryGroup originalGroup, EFmSectionCategory expectedGroup)
        {
            // Setup
            var random = new Random(21);

            // Call
            IEnumerable<FailureMechanismSectionList> output = FailureMechanismSectionListCreator.Create(new[]
            {
                new CombinedAssemblyFailureMechanismInput(random.NextDouble(1, 2), random.NextDouble(), new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, originalGroup)
                })
            });

            // Assert
            Assert.AreEqual(expectedGroup, ((FmSectionWithDirectCategory) output.First().Results.First()).Category);
        }

        private static void AssertSections(CombinedAssemblyFailureMechanismSection[] originalSections, FmSectionWithCategory[] fmSectionWithCategories)
        {
            Assert.AreEqual(originalSections.Length, fmSectionWithCategories.Length);
            Assert.IsTrue(fmSectionWithCategories.All(r => r.GetType() == typeof(FmSectionWithDirectCategory)));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionStart), fmSectionWithCategories.Select(r => r.SectionStart));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionEnd), fmSectionWithCategories.Select(r => r.SectionEnd));
            CollectionAssert.AreEqual(originalSections.Select(s => ConvertCategoryGroup(s.CategoryGroup)),
                                      fmSectionWithCategories.Select(r => (FmSectionWithDirectCategory) r)
                                                             .Select(category => category.Category));
        }

        private static EFmSectionCategory ConvertCategoryGroup(FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            switch (categoryGroup)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return EFmSectionCategory.Gr;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return EFmSectionCategory.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return EFmSectionCategory.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return EFmSectionCategory.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return EFmSectionCategory.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return EFmSectionCategory.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return EFmSectionCategory.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return EFmSectionCategory.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return EFmSectionCategory.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}