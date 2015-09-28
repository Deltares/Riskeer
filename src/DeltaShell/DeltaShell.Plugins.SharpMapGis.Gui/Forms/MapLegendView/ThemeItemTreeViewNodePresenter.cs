using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using SharpMap.Api;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class ThemeItemTreeViewNodePresenter:TreeViewNodePresenterBaseForPluginGui<IThemeItem>
    {
        public ThemeItemTreeViewNodePresenter(): base(null)
        {
        }

        public ThemeItemTreeViewNodePresenter(GuiPlugin guiPlugin): base(guiPlugin)
        {
        }

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
            if (node == null) return;
            
            UpdateNode(null, node, themeItem);
        }
    }
}