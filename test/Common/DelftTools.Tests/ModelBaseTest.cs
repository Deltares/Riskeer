using DelftTools.Tests.Core.Mocks;
using NUnit.Framework;

namespace DelftTools.Tests.Core.WorkFlow
{
    [TestFixture]
    public class ModelBaseTest
    {
        [Test]
        public void TestOutputIsEmpty()
        {
            // Create a model base model: OutputIsEmpty should be true
            var modelBase = new ModelBaseStub();
            Assert.IsTrue(modelBase.OutputIsEmpty);

            // Run the model: OutputIsEmpty should become false
            modelBase.Execute();
            Assert.IsFalse(modelBase.OutputIsEmpty);

            // Clear the output: OutputIsEmpty should become true
            modelBase.ClearOutput();
            Assert.IsTrue(modelBase.OutputIsEmpty);
        }

        [Test]
        public void TestOutputIsOutOfSync()
        {
            // Create a model base model: OutputOutOfSync should be false
            var modelBase = new ModelBaseStub();
            Assert.IsFalse(modelBase.OutputOutOfSync);

            // Mark output out of sync: OutputOutOfSync should still be false => no output yet
            modelBase.SetOutputOutOfSync();
            Assert.IsFalse(modelBase.OutputOutOfSync);

            // Run the model: OutputOutOfSync should still be false
            modelBase.Execute();
            Assert.IsFalse(modelBase.OutputOutOfSync);

            // Mark output out of sync: OutputOutOfSync should be true
            modelBase.SetOutputOutOfSync();
            Assert.IsTrue(modelBase.OutputOutOfSync);

            // Run the model again: OutputOutOfSync should become false => new output
            modelBase.Execute();
            Assert.IsFalse(modelBase.OutputOutOfSync);

            // Mark output out of sync: OutputOutOfSync should be true
            modelBase.SetOutputOutOfSync();
            Assert.IsTrue(modelBase.OutputOutOfSync);

            // Clear the output: OutputOutOfSync should become false again => no output left
            modelBase.ClearOutput();
            Assert.IsFalse(modelBase.OutputOutOfSync);
        }
    }
}
