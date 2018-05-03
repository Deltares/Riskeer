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

using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsInputContextProperties(context, () => (RoundedDouble) 1.1, handler);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionCategoryWaveConditionsInputContextProperties<WaveImpactAsphaltCoverWaveConditionsInputContext>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual("Asfalt", properties.RevetmentType);
            mockRepository.VerifyAll();
        }
    }
}