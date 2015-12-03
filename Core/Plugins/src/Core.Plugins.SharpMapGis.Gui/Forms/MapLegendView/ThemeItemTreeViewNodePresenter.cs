using System.ComponentModel;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class ThemeItemTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<IThemeItem>
    {
        public ThemeItemTreeViewNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, IThemeItem themeItem)
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

        protected override void OnPropertyChanged(IThemeItem themeItem, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null)
            {
                return;
            }

            UpdateNode(null, node, themeItem);
        }
    }
}