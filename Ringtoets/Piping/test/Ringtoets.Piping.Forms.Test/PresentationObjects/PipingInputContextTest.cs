﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
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
    public class PipingInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var context = new PipingInputContext(calculationItem.InputParameters, calculationItem, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<PipingInput>>(context);
            Assert.AreSame(calculationItem.InputParameters, context.WrappedData);
            Assert.AreSame(calculationItem, context.PipingCalculationItem);
            Assert.AreSame(failureMechanism, context.PipingFailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(surfaceLines, context.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(stochasticSoilModels, context.AvailableStochasticSoilModels);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new PipingInputContext(calculationItem.InputParameters, calculationItem, surfaceLines, stochasticSoilModels, failureMechanism, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");
        }

        [Test]
        public void ParameteredConstructor_CalculationItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationInput = new PipingInput(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingInputContext(calculationInput, null, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "De berekening mag niet 'null' zijn.");
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingInputContext(calculationItem.InputParameters, calculationItem, surfaceLines, stochasticSoilModels, null, assessmentSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het piping faalmechanisme mag niet 'null' zijn.");
            mocks.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingInputContext(calculationItem.InputParameters, 
                                                 calculationItem,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSectionMock);

            // Call
            context.Attach(observer);

            // Assert
            calculationItem.InputParameters.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingInputContext(calculationItem.InputParameters, 
                                                 calculationItem,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSectionMock);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            calculationItem.InputParameters.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToPipingInput_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculationItem = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingInputContext(calculationItem.InputParameters, 
                                                 calculationItem,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 failureMechanism,
                                                 assessmentSectionMock);

            calculationItem.InputParameters.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }
    }
}