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
using System.Collections.Generic;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestWaveConditionsInputContext(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            var context = new TestWaveConditionsInputContext(input);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<WaveConditionsInput>>(context);
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(input, context.WrappedData);
        }

        private class TestWaveConditionsInputContext : WaveConditionsInputContext
        {
            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData) : base(wrappedData, new TestCalculation()) {}

            public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}