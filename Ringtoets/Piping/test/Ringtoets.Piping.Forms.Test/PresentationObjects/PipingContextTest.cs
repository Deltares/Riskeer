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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine[] surfaceLines =
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var target = new ObservableObject();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new SimplePipingContext<ObservableObject>(target, surfaceLines, soilModels, failureMechanism, assessmentSectionStub);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableObject>>(context);
            Assert.AreSame(surfaceLines, context.AvailablePipingSurfaceLines,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(soilModels, context.AvailableStochasticSoilModels,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSectionStub, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SurfaceLinesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                null,
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSectionStub);

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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                null,
                                                                                new PipingFailureMechanism(),
                                                                                assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilModels", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_PipingFailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                null,
                                                                                assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("pipingFailureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                new PipingFailureMechanism(),
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        private class SimplePipingContext<T> : PipingContext<T> where T : IObservable
        {
            public SimplePipingContext(T target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> stochasticSoilModels, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
                : base(target, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }

        private class ObservableObject : Observable {}
    }
}