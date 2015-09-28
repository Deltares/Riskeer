using DelftTools.Shell.Core;
using NUnit.Framework;

namespace DelftTools.Tests.Shell.Core
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void CreateWithDefaultName()
        {
            var project = new Project();
            Assert.AreEqual("Project", project.Name);
        }

        [Test]
        public void CreateWithCustomName()
        {
            var project = new Project("Test Project");
            Assert.AreEqual(project.Name, "Test Project");
        }
    }
}