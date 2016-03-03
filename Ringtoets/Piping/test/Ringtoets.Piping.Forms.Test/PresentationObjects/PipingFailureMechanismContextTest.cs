﻿using System;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingFailureMechanismContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismContext<PipingFailureMechanism>>(context);
            Assert.AreSame(assessmentSection, context.Parent);
            Assert.AreSame(failureMechanism, context.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingFailureMechanismContext(null, assessmentSection);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new PipingFailureMechanismContext(failureMechanism, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }
    }
}