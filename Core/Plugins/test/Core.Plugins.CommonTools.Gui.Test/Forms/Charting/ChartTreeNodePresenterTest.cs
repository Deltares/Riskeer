using System;
using System.Collections;
using System.Linq;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Plugins.Charting.Forms;
using Core.Plugins.Charting.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Forms.Charting
{
    [TestFixture]
    public class ChartTreeNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var nodePresenter = new ChartTreeNodePresenter();

            // Assert
            Assert.IsInstanceOf<TreeViewNodePresenterBase<IChart>>(nodePresenter);
        }

        [Test]
        public void CanRenameNode_Always_ReturnsTrue()
        {
            // Setup
            var nodePresenter = new ChartTreeNodePresenter();

            // Call
            var result = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("Some name")]
        [TestCase("")]
        public void UpdateNode_NoKnownSeriesType_PropertiesSetImageNull(string title)
        {
            // Setup
            var nodePresenter = new ChartTreeNodePresenter();
            var node = new TreeNode(null);
            var chart = new Chart
            {
                Title = title,
            };

            // Call
            nodePresenter.UpdateNode(null, node, chart);

            // Assert
            Assert.AreEqual(string.IsNullOrEmpty(title) ? Resources.ChartTreeNodePresenter_UpdateNode_Chart : title, node.Text);
            TestHelper.AssertImagesAreEqual(Resources.Chart, node.Image);
            Assert.AreSame(chart, node.Tag);
        }

        [Test]
        public void OnNodeRenamed_NoChartSeries_DoesNotThrow()
        {
            // Setup
            var nodePresenter = new ChartTreeNodePresenter();

            // Call
            TestDelegate testDelegate = () => nodePresenter.OnNodeRenamed(null, string.Empty);

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void OnNodeRenamed_NewNodeName_ChartSeriesTitleSetToNodeName()
        {
            // Setup
            var nodePresenter = new ChartTreeNodePresenter();
            var name = "<some name>";
            var newName = "<some new name>";
            var chart = new Chart
            {
                Title = name
            };

            // Call
            nodePresenter.OnNodeRenamed(chart, newName);

            // Assert
            Assert.AreEqual(newName, chart.Title);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnsSeries()
        {
            // Setup
            var mocks = new MockRepository();
            var nodePresenter = new ChartTreeNodePresenter();
            var chart = mocks.StrictMock<IChart>();
            chart.Expect(c => c.Series).Return(Enumerable.Empty<ChartSeries>());

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetChildNodeObjects(chart);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewHasSorter_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var treeView = mocks.StrictMock<ITreeView>();
            treeView.Expect(tv => tv.TreeViewNodeSorter).Return(mocks.Stub<IComparer>());

            mocks.ReplayAll();

            var nodePresenter = new ChartTreeNodePresenter { TreeView = treeView };

            // Call
            var insertionAllowed = nodePresenter.CanInsert(null, null, null);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewDoesNotHaveSorter_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var treeView = mocks.StrictMock<ITreeView>();
            treeView.Expect(tv => tv.TreeViewNodeSorter).Return(null);

            mocks.ReplayAll();

            var nodePresenter = new ChartTreeNodePresenter { TreeView = treeView };

            // Call
            var insertionAllowed = nodePresenter.CanInsert(null, null, null);

            // Assert
            Assert.IsTrue(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_ItemNotIChartSeries_ReturnsNone(DragOperations validOperation)
        {
            // Setup
            var nodePresenter = new ChartTreeNodePresenter();

            // Call
            var result = nodePresenter.CanDrop(new object(), null, null, validOperation);

            // Assert
            Assert.AreEqual(DragOperations.None, result);
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_ItemIChartSeries_ReturnsValidOperation(DragOperations validOperation)
        {
            // Setup
            var mocks = new MockRepository();
            var nodePresenter = new ChartTreeNodePresenter();
            var chartSeries = mocks.StrictMock<IChartSeries>();

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.CanDrop(chartSeries, null, null, validOperation);

            // Assert
            Assert.AreEqual(validOperation, result);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void OnDragDrop_ChartSeriesItemIChartParent_RemovesAndInserts(DragOperations operation)
        {
            // Setup
            var mocks = new MockRepository();
            var nodePresenter = new ChartTreeNodePresenter();
            var series = new TestChartSeries();
            var random = new Random(21);
            var position = random.Next();

            var parent = mocks.StrictMock<IChart>();
            var target = mocks.StrictMock<IChart>();
            parent.Expect(p => p.RemoveChartSeries(series)).Return(true);
            target.Expect(t => t.InsertChartSeries(series, position));

            mocks.ReplayAll();

            // Call
            nodePresenter.OnDragDrop(series, parent, target, operation, position);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void OnDragDrop_ChartSeriesItemObjectParent_RemovesAndInserts(DragOperations operation)
        {
            // Setup
            var mocks = new MockRepository();
            var nodePresenter = new ChartTreeNodePresenter();
            var series = new TestChartSeries();
            var random = new Random(21);
            var position = random.Next();

            var parent = new object();
            var target = mocks.StrictMock<IChart>();
            target.Expect(p => p.RemoveChartSeries(series)).Return(true);
            target.Expect(t => t.InsertChartSeries(series, position));

            mocks.ReplayAll();

            // Call
            nodePresenter.OnDragDrop(series, parent, target, operation, position);

            // Assert
            mocks.VerifyAll();
        }
    }
}