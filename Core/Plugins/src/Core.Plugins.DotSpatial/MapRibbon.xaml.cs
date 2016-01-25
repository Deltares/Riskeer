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
        /// <summary>
        /// Creates an instance of <see cref="MapRibbon"/>
        /// </summary>
        public MapRibbon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the command used when open map button is clicked.
        /// </summary>
        public ICommand OpenMapViewCommand { private get; set; }

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

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems() {}

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        private void ButtonOpenMapView_Click(object sender, RoutedEventArgs e)
        {
            OpenMapViewCommand.Execute();
        }
    }
}