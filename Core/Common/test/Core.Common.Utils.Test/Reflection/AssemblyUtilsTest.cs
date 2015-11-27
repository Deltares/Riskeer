using System.Reflection;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Reflection
{
    [TestFixture]
    public class AssemblyUtilsTest
    {
        [Test]
        public void GetAssemblyInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyUtils.AssemblyInfo assemblyInfo = AssemblyUtils.GetAssemblyInfo(assembly);
            Assert.AreEqual("Core.Common.Utils.Test", assemblyInfo.Title);
        }
    }
}