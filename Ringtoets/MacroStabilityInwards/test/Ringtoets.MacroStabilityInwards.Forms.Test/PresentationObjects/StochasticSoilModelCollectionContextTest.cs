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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StochasticSoilModelCollectionContextTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var context = new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<MacroStabilityInwardsStochasticSoilModelCollection>>(context);
            Assert.AreSame(failureMechanism.StochasticSoilModels, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModelCollection();

            // Call
            TestDelegate test = () => new StochasticSoilModelCollectionContext(stochasticSoilModels, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate test = () => new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}