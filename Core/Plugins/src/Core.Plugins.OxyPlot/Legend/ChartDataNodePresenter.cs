using System;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Components.Charting.Data;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="PointBasedChartData"/> as a tree node.
    /// </summary>
    public abstract class ChartDataNodePresenter<T> : TreeViewNodePresenterBase<T> where T : PointBasedChartData
    {
        /// <summary>
        /// Gets the text to set on the node.
        /// </summary>
        protected abstract string Text { get; }

        /// <summary>
        /// Gets the icon to set for the node.
        /// </summary>
        protected abstract Bitmap Icon { get; }

        /// <summary>
        /// Returns the <see cref="DragOperations"/> possible on <paramref name="nodeData"/>.
        /// </summary>
        /// <param name="nodeData">The data of type <typeparamref name="T"/> to base <see cref="DragOperations"/>
        /// on.</param>
        /// <returns>The <see cref="DragOperations"/> possible on <paramref name="nodeData"/>.</returns>
        public override DragOperations CanDrag(T nodeData)
        {
            return DragOperations.Move;
        }

        /// <summary>
        /// Updates the <paramref name="node"/> with data taken from <see cref="nodeData"/>.
        /// </summary>
        /// <param name="parentNode">The parent <see cref="TreeNode"/> of the <paramref name="node"/>.</param>
        /// <param name="node">The <see cref="TreeNode"/> to update.</param>
        /// <param name="nodeData">The data of type <typeparamref name="T"/> to update the <paramref name="node"/>
        /// with.</param>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="node"/> is <c>null</c></item>
        /// <item><paramref name="nodeData"/> is <c>null</c></item>
        /// </list> 
        /// </exception>
        public override void UpdateNode(TreeNode parentNode, TreeNode node, T nodeData)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "Cannot update node without data.");
            }
            if (nodeData == null)
            {
                throw new ArgumentNullException("nodeData", "Cannot update node without data.");
            }

            node.Text = Text;
            node.Image = Icon;
            node.ShowCheckBox = true;

            var isVisible = nodeData.IsVisible;

            if (node.Checked != isVisible)
            {
                node.Checked = isVisible;
            }
        }

        /// <summary>
        /// Updates the state of the data associated with <paramref name="node"/> based on its <see cref="TreeNode.Checked"/> property.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> which had its <see cref="TreeNode.Checked"/> property updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="TreeNode.Tag"/> of <paramref name="node"/> is <c>null</c>.</exception>
        public override void OnNodeChecked(TreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "Cannot update node without data.");
            }
            var data = node.Tag as T;
            if (data == null)
            {
                throw new ArgumentException("Cannot invoke OnNodeChecked for a node without tag.");
            }
            data.IsVisible = node.Checked;

            var parentData = node.Parent == null ? null : node.Parent.Tag as IObservable;
            if (parentData != null)
            {
                parentData.NotifyObservers();
            }
        }
    }
}