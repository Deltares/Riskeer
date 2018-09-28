// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty),
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };

            MacroStabilityInwardsStochasticSoilModel[] soilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };

            var target = new ObservableObject();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var context = new SimpleMacroStabilityInwardsContext<ObservableObject>(target, surfaceLines, soilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableObject>>(context);
            Assert.AreSame(surfaceLines, context.AvailableMacroStabilityInwardsSurfaceLines,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(soilModels, context.AvailableStochasticSoilModels,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SurfaceLinesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleMacroStabilityInwardsContext<ObservableObject>(new ObservableObject(),
                                                                                               null,
                                                                                               Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                               failureMechanism,
                                                                                               assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLines", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_StochasticSoilModelsIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleMacroStabilityInwardsContext<ObservableObject>(new ObservableObject(),
                                                                                               Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                               null,
                                                                                               new MacroStabilityInwardsFailureMechanism(),
                                                                                               assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilModels", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleMacroStabilityInwardsContext<ObservableObject>(new ObservableObject(),
                                                                                               Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                               Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                               null,
                                                                                               assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("macroStabilityInwardsFailureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleMacroStabilityInwardsContext<ObservableObject>(new ObservableObject(),
                                                                                               Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                               Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                               new MacroStabilityInwardsFailureMechanism(),
                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        private class SimpleMacroStabilityInwardsContext<T> : MacroStabilityInwardsContext<T> where T : IObservable
        {
            public SimpleMacroStabilityInwardsContext(T target, IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels, MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism, IAssessmentSection assessmentSection)
                : base(target, surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection) {}
        }

        private class ObservableObject : Observable {}
    }
}