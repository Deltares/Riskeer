﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Test.Providers
{
    [TestFixture]
    public class LengthEffectProviderTest
    {
        [Test]
        public void Constructor_GetUseLengthEffectFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new LengthEffectProvider(null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getUseLengthEffectFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_GetSectionNFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new LengthEffectProvider(() => true, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getSectionNFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            bool useLengthEffect = random.NextBoolean();
            double sectionN = random.NextDouble();

            // Call
            var provider = new LengthEffectProvider(() => useLengthEffect, () => sectionN);

            // Assert
            Assert.IsInstanceOf<ILengthEffectProvider>(provider);
            Assert.AreEqual(useLengthEffect, provider.UseLengthEffect);
            Assert.AreEqual(sectionN, provider.SectionN);
        }
    }
}