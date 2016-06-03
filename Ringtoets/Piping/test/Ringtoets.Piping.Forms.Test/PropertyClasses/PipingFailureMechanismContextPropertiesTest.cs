using System;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingFailureMechanismContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new PipingFailureMechanismContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingFailureMechanismContext>>(properties);
        }

        [Test]
        public void Data_SetNewPipingFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties();

            // Call
            properties.Data = new PipingFailureMechanismContext(failureMechanism, new MockRepository().StrictMock<IAssessmentSection>());

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.GeneralInput.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(failureMechanism.GeneralInput.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(failureMechanism.GeneralInput.WaterVolumetricWeight, properties.WaterVolumetricWeight);

            Assert.AreEqual(failureMechanism.GeneralInput.CriticalHeaveGradient, properties.CriticalHeaveGradient);

            Assert.AreEqual(failureMechanism.GeneralInput.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(failureMechanism.GeneralInput.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(failureMechanism.GeneralInput.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(failureMechanism.GeneralInput.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(failureMechanism.GeneralInput.Gravity, properties.Gravity);
            Assert.AreEqual(failureMechanism.GeneralInput.MeanDiameter70, properties.MeanDiameter70);
            Assert.AreEqual(failureMechanism.GeneralInput.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);

            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, properties.A);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.B, properties.B);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_SetInvalidValue_ThrowsArgumentException(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties
            {
                Data = new PipingFailureMechanismContext(failureMechanism, new MockRepository().StrictMock<IAssessmentSection>())
            };

            // Call
            TestDelegate call = () => properties.A = value;

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("De waarde moet tussen 0 en 1 zijn.", exception.Message);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, properties.A);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_SetValidValue_SetsValueAndUpdatesObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var properties = new PipingFailureMechanismContextProperties
            {
                Data = new PipingFailureMechanismContext(failureMechanism, new MockRepository().StrictMock<IAssessmentSection>())
            };

            failureMechanism.Attach(observerMock);

            // Call
            properties.A = value;

            // Assert
            Assert.AreEqual(value, failureMechanism.PipingProbabilityAssessmentInput.A);
            mocks.VerifyAll();
        }
    }
}