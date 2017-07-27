﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<PipingSurfaceLine>, PipingSurfaceLine>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<PipingSurfaceLine> CreateCollection()
        {
            return new RingtoetsPipingSurfaceLineCollection();
        }

        protected override IEnumerable<PipingSurfaceLine> UniqueElements()
        {
            yield return new PipingSurfaceLine
            {
                Name = "Name A"
            };
            yield return new PipingSurfaceLine
            {
                Name = "Name B"
            };
        }

        protected override IEnumerable<PipingSurfaceLine> SingleNonUniqueElements()
        {
            const string duplicateName = "Duplicate name it is";

            yield return new PipingSurfaceLine
            {
                Name = duplicateName
            };
            yield return new PipingSurfaceLine
            {
                Name = duplicateName
            };
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<PipingSurfaceLine> itemsToAdd)
        {
            string duplicateName = itemsToAdd.First().Name;
            Assert.AreEqual($"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.",
                            exception.Message);
        }

        protected override IEnumerable<PipingSurfaceLine> MultipleNonUniqueElements()
        {
            const string duplicateNameOne = "Duplicate name it is";
            const string duplicateNameTwo = "Duplicate name again";
            yield return new PipingSurfaceLine
            {
                Name = duplicateNameOne
            };
            yield return new PipingSurfaceLine
            {
                Name = duplicateNameOne
            };
            yield return new PipingSurfaceLine
            {
                Name = duplicateNameTwo
            };
            yield return new PipingSurfaceLine
            {
                Name = duplicateNameTwo
            };
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<PipingSurfaceLine> itemsToAdd)
        {
            string duplicateNameOne = itemsToAdd.First().Name;
            string duplicateNameTwo = itemsToAdd.First(i => i.Name != duplicateNameOne).Name;
            Assert.AreEqual("Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: " +
                            $"{duplicateNameOne}, {duplicateNameTwo}.", exception.Message);
        }
    }
}