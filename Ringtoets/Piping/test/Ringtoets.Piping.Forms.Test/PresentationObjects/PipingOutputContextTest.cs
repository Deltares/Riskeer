// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingOutputContextTest
    {
        [Test]
        public void Constructor_WithoutOutput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingOutputContext(null, new TestPipingSemiProbabilisticOutput());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_WithoutSemiProbabilisticOutput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingOutputContext(new TestPipingOutput(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("semiProbabilisticOutput", paramName);
        }

        [Test]
        public void Constructor_WithOutputParameters_PropertiesSet()
        {
            // Setup
            var pipingOutput = new TestPipingOutput();
            var semiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();

            // Call
            var context = new PipingOutputContext(pipingOutput, semiProbabilisticOutput);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<PipingOutput>>(context);
            Assert.AreSame(pipingOutput, context.WrappedData);
            Assert.AreSame(semiProbabilisticOutput, context.SemiProbabilisticOutput);
        }
    }
}