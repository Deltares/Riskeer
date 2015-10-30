using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WtiProject"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "WtiProjectProperties_DisplayName")]
    public class WtiProjectProperties : ObjectProperties<WtiProject>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "WtiProject_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WtiProject_Name_Description")]
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