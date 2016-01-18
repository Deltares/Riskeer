using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Core.Plugins.DotSpatial
{
    /// <summary>
    /// Interaction logic for MapRibbon.xaml
    /// </summary>
    public partial class MapRibbon : IRibbonCommandHandler
    {
        public MapRibbon()
        {
            InitializeComponent();
        }

        private void ButtonOpenMapView_Click(object sender, RoutedEventArgs e)
        {
            OpenMapViewCommand.Execute();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (OpenMapViewCommand != null)
                {
                    yield return OpenMapViewCommand;
                }
            }
        }

        public ICommand OpenMapViewCommand { private get; set; }

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }
    }
}