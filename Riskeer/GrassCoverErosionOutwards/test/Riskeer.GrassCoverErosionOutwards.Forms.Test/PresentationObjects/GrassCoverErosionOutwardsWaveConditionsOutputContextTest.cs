﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputContextTest
    {
        [Test]
        public void Constructor_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveConditionsOutputContext(output, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            var input = new GrassCoverErosionOutwardsWaveConditionsInput();

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsOutputContext(output, input);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<GrassCoverErosionOutwardsWaveConditionsOutput>>(context);
            Assert.AreSame(input, context.Input);
            Assert.AreSame(output, context.WrappedData);
        }
    }
}