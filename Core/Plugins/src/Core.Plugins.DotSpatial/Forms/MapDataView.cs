using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial;

namespace Core.Plugins.DotSpatial.Forms
{
    public partial class MapDataView : UserControl, IView
    {
        private readonly BaseMap baseMap;
        private ICollection<string> data;

        public MapDataView()
        {
            baseMap = new BaseMap
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(baseMap);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (ICollection<string>) value;
                baseMap.Data = data;
            }
        }
    }
}