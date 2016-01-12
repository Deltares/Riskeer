using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Styles;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class VectorStyleTreeViewNodePresenter : TreeViewNodePresenterBase<VectorStyle>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, VectorStyle style)
        {
            if (node.Tag != style)
            {
                node.Tag = style;
            }

            if (node.Text != string.Empty)
            {
                node.Text = string.Empty;
            }

            if (node.Image != style.LegendSymbol)
            {
                node.Image = style.LegendSymbol;
            }
        }
    }
}