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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_WithoutContext_ThrowsArgumentNullException()
        {
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var context = new StabilityStoneCoverWaveConditionsInputContext(
                new WaveConditionsInput(),
                new TestWaveConditionsCalculation(), 
                Enumerable.Empty<ForeshoreProfile>(),
                assessmentSection);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties(context, handler);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<StabilityStoneCoverWaveConditionsInputContext>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual("Steen (blokken en zuilen)", properties.RevetmentType);
            mockRepository.VerifyAll();
        }
    }
}