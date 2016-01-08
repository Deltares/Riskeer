using System.ComponentModel;
using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class ThemeItemTreeViewNodePresenter : TreeViewNodePresenterBase<IThemeItem>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, IThemeItem themeItem)
        {
            if (node.Text != themeItem.Label)
            {
                node.Text = themeItem.Label;
            }

            if (themeItem.Style is VectorStyle && node.Image != (themeItem.Style as VectorStyle).LegendSymbol)
            {
                node.Image = (themeItem.Style as VectorStyle).LegendSymbol;
            }
        }

        protected override void OnPropertyChanged(IThemeItem themeItem, TreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null)
            {
                return;
            }

            UpdateNode(null, node, themeItem);
        }
    }
}