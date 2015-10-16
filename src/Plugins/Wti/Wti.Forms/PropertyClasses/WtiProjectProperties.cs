using DelftTools.Shell.Gui;
using DelftTools.Utils;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WtiProject"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "WtiProjectPropertiesDisplayName")]
    public class WtiProjectProperties : ObjectProperties<WtiProject>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "WtiProjectNameDisplayName")]
        [ResourcesDescription(typeof(Resources), "WtiProjectNameDescription")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
    }
}