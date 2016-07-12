using Core.Common.Base.Plugin;
using NUnit.Framework;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var pipingApplicationPlugin = new PipingApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(pipingApplicationPlugin);
        }
    }
}