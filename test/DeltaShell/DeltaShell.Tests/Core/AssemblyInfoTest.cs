using DelftTools.Utils.Reflection;
using DeltaShell.Core;
using NUnit.Framework;

namespace DeltaShell.Tests.Core
{
    [TestFixture]
    public class AssemblyInfoTest
    {
        [Test]
        public void CheckForValidAssembly()
        {
            AssemblyUtils.AssemblyInfo info = AssemblyUtils.GetAssemblyInfo((typeof(DeltaShellApplication)).Assembly);

            Assert.AreEqual("Delta Shell", info.Title);
            Assert.IsNotNull(info.Version);
            Assert.IsNotNull(info.Description);
            Assert.AreEqual("Delta Shell", info.Product);
            Assert.IsNotNull(info.Copyright);
            Assert.AreEqual("Deltares", info.Company);
        }
    }
}