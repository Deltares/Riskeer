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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class RingtoetsMacroStabilityInwardsSurfaceLineCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<RingtoetsMacroStabilityInwardsSurfaceLine>, RingtoetsMacroStabilityInwardsSurfaceLine>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<RingtoetsMacroStabilityInwardsSurfaceLine> CreateCollection()
        {
            return new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
        }

        protected override IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> UniqueElements()
        {
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name A"
            };
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name B"
            };
        }

        protected override IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> SingleNonUniqueElements()
        {
            const string duplicateName = "Duplicate name it is";

            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateName
            };
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateName
            };
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> itemsToAdd)
        {
            string duplicateName = itemsToAdd.First().Name;
            Assert.AreEqual($"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.",
                            exception.Message);
        }

        protected override IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> MultipleNonUniqueElements()
        {
            const string duplicateNameOne = "Duplicate name it is";
            const string duplicateNameTwo = "Duplicate name again";
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateNameOne
            };
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateNameOne
            };
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateNameTwo
            };
            yield return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateNameTwo
            };
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> itemsToAdd)
        {
            string duplicateNameOne = itemsToAdd.First().Name;
            string duplicateNameTwo = itemsToAdd.First(i => i.Name != duplicateNameOne).Name;
            Assert.AreEqual("Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: " +
                            $"{duplicateNameOne}, {duplicateNameTwo}.", exception.Message);
        }
    }
}