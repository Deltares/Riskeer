﻿using System;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class AssessmentSectionStateRootContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            var context = new AssessmentSectionStateRootContext(assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<AssessmentSection>>(context);
            Assert.AreSame(assessmentSection, context.WrappedData);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionStateRootContext(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("wrappedData", exception.ParamName);
        }
    }
}