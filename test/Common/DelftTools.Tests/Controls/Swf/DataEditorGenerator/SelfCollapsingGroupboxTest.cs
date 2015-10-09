using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator
{
    [TestFixture]
    public class SelfCollapsingGroupboxTest
    {
        [Test]
        public void OneChildTest()
        {
            var box = new SelfCollapsingGroupbox();

            var panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            var child1 = new SelfCollapsingPanel();
            panel.Controls.Add(child1);
            box.SubscribeChild(child1);

            Assert.IsTrue(box.Visible);

            child1.Visible = false;
            Assert.IsFalse(box.Visible);
        }

        [Test]
        public void TwoChildrenTest()
        {
            var box = new SelfCollapsingGroupbox();

            var panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            var child1 = new SelfCollapsingPanel();
            panel.Controls.Add(child1);
            box.SubscribeChild(child1);

            var child2 = new SelfCollapsingPanel();
            panel.Controls.Add(child2);
            box.SubscribeChild(child2);

            Assert.IsTrue(box.Visible);

            child1.Visible = false;
            Assert.IsTrue(box.Visible);

            child2.Visible = false;
            Assert.IsFalse(box.Visible);
        }

        [Test]
        public void AlreadyInvisibleTest()
        {
            var box = new SelfCollapsingGroupbox();

            var panel = new FlowLayoutPanel();
            box.Controls.Add(panel);
            box.SetChildContainer(panel);

            var child1 = new SelfCollapsingPanel
            {
                Visible = false
            };

            panel.Controls.Add(child1);
            box.SubscribeChild(child1);

            var child2 = new SelfCollapsingPanel
            {
                Visible = false
            };

            panel.Controls.Add(child2);
            box.SubscribeChild(child2);

            Assert.IsFalse(box.Visible);

            child1.Visible = true;
            Assert.IsTrue(box.Visible);
        }
    }
}