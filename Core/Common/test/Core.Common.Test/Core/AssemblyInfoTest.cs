using Core.Common.Base.Plugin;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Test.Core
{
    [TestFixture]
    public class AssemblyInfoTest
    {
        [Test]
        public void CheckForValidAssembly()
        {
            AssemblyUtils.AssemblyInfo info = AssemblyUtils.GetAssemblyInfo((typeof(ApplicationCore)).Assembly);

            Assert.AreEqual("Core.Common.Base", info.Title);
            Assert.IsNotNull(info.Version);
            Assert.IsNotNull(info.Description);
            Assert.AreEqual("Core.Common.Base", info.Product);
            Assert.IsNotNull(info.Copyright);
            Assert.AreEqual("Deltares", info.Company);
        }
    }
}