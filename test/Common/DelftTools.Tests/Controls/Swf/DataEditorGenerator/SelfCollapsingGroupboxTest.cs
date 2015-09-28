using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator
{
    [TestFixture]
    class SelfCollapsingGroupboxTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void OneChildTest()
        {
            SelfCollapsingGroupbox box = new SelfCollapsingGroupbox();

            FlowLayoutPanel panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            SelfCollapsingPanel child1 = new SelfCollapsingPanel();
            panel.Controls.Add(child1);
            box.SubscribeChild(child1);
            
            WindowsFormsTestHelper.Show(box);

            Assert.IsTrue(box.Visible);

            child1.Visible = false;

            Assert.IsFalse(box.Visible);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TwoChildrenTest()
        {
            SelfCollapsingGroupbox box = new SelfCollapsingGroupbox();

            FlowLayoutPanel panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            SelfCollapsingPanel child1 = new SelfCollapsingPanel();
            panel.Controls.Add(child1);
            box.SubscribeChild(child1);

            SelfCollapsingPanel child2 = new SelfCollapsingPanel();
            panel.Controls.Add(child2);
            box.SubscribeChild(child2);

            WindowsFormsTestHelper.Show(box);

            Assert.IsTrue(box.Visible);

            child1.Visible = false;

            Assert.IsTrue(box.Visible);

            child2.Visible = false;

            Assert.IsFalse(box.Visible);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void AlreadyInvisibleTest()
        {
            SelfCollapsingGroupbox box = new SelfCollapsingGroupbox();

            FlowLayoutPanel panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            SelfCollapsingPanel child1 = new SelfCollapsingPanel();
            child1.Visible = false;
            panel.Controls.Add(child1);
            box.SubscribeChild(child1);

            SelfCollapsingPanel child2 = new SelfCollapsingPanel();
            child2.Visible = false;
            panel.Controls.Add(child2);
            box.SubscribeChild(child2);

            WindowsFormsTestHelper.Show(box);

            Assert.IsFalse(box.Visible);

            child1.Visible = true;

            Assert.IsTrue(box.Visible);

            WindowsFormsTestHelper.CloseAll();
        }
    }
}
