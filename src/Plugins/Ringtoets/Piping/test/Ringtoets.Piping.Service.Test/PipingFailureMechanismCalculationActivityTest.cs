using DelftTools.Shell.Core.Workflow;

using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingFailureMechanismCalculationActivityTest
    {
        [Test]
        public void ParameterdConstructor_ExpectedValues()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();

            // Call
            var activity = new PipingFailureMechanismCalculationActivity(pipingFailureMechanism);

            // Assert
            Assert.IsInstanceOf<SequentialActivity>(activity);
        }
    }
}