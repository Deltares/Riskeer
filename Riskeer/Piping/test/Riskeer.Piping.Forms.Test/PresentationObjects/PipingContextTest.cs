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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingSurfaceLine[] surfaceLines =
            {
                new PipingSurfaceLine(string.Empty),
                new PipingSurfaceLine(string.Empty)
            };

            PipingStochasticSoilModel[] soilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            };

            var target = new ObservableObject();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new SimplePipingContext<ObservableObject>(target, surfaceLines, soilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableObject>>(context);
            Assert.AreSame(surfaceLines, context.AvailablePipingSurfaceLines,
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

            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                null,
                                                                                Enumerable.Empty<PipingStochasticSoilModel>(),
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
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<PipingSurfaceLine>(),
                                                                                null,
                                                                                new PipingFailureMechanism(),
                                                                                assessmentSection);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<PipingSurfaceLine>(),
                                                                                Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                null,
                                                                                assessmentSection);

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
                                                                                Enumerable.Empty<PipingSurfaceLine>(),
                                                                                Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                new PipingFailureMechanism(),
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        private class SimplePipingContext<T> : PipingContext<T> where T : IObservable
        {
            public SimplePipingContext(T target,
                                       IEnumerable<PipingSurfaceLine> surfaceLines,
                                       IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                       PipingFailureMechanism pipingFailureMechanism,
                                       IAssessmentSection assessmentSection)
                : base(target, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }

        private class ObservableObject : Observable {}
    }
}