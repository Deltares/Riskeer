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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsOutputContextTest
    {
        [Test]
        public void Constructor_WithoutOutput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsOutputContext(null, new TestMacroStabilityInwardsSemiProbabilisticOutput());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_WithoutSemiProbabilisticOutput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsOutputContext(new TestMacroStabilityInwardsOutput(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("semiProbabilisticOutput", paramName);
        }

        [Test]
        public void Constructor_WithOutputParameters_PropertiesSet()
        {
            // Setup
            var output = new TestMacroStabilityInwardsOutput();
            var semiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput();

            // Call
            var context = new MacroStabilityInwardsOutputContext(output, semiProbabilisticOutput);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<MacroStabilityInwardsOutput>>(context);
            Assert.AreSame(output, context.WrappedData);
            Assert.AreSame(semiProbabilisticOutput, context.SemiProbabilisticOutput);
        }
    }
}