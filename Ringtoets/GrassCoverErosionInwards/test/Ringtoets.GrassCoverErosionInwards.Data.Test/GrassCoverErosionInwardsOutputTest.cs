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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using GrassCoverErosionInwardsCloneAssert = Ringtoets.GrassCoverErosionInwards.Data.TestUtil.CloneAssert;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputTest
    {
        [Test]
        public void Constructor_OvertoppingOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsOutput(
                null,
                new TestDikeHeightOutput(double.NaN),
                new TestOvertoppingRateOutput(double.NaN));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("overtoppingOutput", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var overtoppingOutput = new TestOvertoppingOutput(double.NaN);
            var dikeHeightOutput = new TestDikeHeightOutput(double.NaN);
            var overtoppingRateOutput = new TestOvertoppingRateOutput(double.NaN);

            // Call
            var output = new GrassCoverErosionInwardsOutput(overtoppingOutput, dikeHeightOutput, overtoppingRateOutput);

            // Assert
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.IsInstanceOf<Observable>(output);

            Assert.AreSame(overtoppingOutput, output.OvertoppingOutput);
            Assert.AreSame(dikeHeightOutput, output.DikeHeightOutput);
            Assert.AreSame(overtoppingRateOutput, output.OvertoppingRateOutput);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                              new TestDikeHeightOutput(random.NextDouble()),
                                                              new TestOvertoppingRateOutput(random.NextDouble()));

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                              null,
                                                              null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }
    }
}